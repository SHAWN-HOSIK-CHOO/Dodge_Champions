using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PingPong : NetworkBehaviour
{
    string MessageName = "PingPongMessage";

    [SerializeField]
    private float _jitterRange;
    [SerializeField]
    private bool _useVirtualRtt;
    [SerializeField]
    private float _virtualRtt;


    public event Action OnRttChanged;

    private Dictionary<ulong, double> _smoothedRTT;
    private const float Alpha = 0.125f;
    public override void OnNetworkSpawn()
    {
        _smoothedRTT = new Dictionary<ulong, double>();

        if (FreeNet._instance._ngoManager._useEpicOnlineTransport)
        {
            FreeNet._instance._ngoManager.GetComponent<EOSNetcodeTransport>()._pingpong = this;
        }
        NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, ReceiveMessage);
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisConnected;
    }
    public void SetVirtualRtt(bool useVirtualRtt, float virtualRtt = 0)
    {
        _useVirtualRtt = useVirtualRtt;
        _virtualRtt = virtualRtt;
    }
    public void SetJitterRanage(float jitterRange)
    {
        _jitterRange = jitterRange;
    }
    private void OnClientConnected(ulong clientId)
    {
        _smoothedRTT.Add(clientId, 0);
    }
    private void OnClientDisConnected(ulong clientId)
    {
        _smoothedRTT.Remove(clientId);
    }
    private void FixedUpdate()
    {
        if (IsSpawned && IsServer)
        {
            SendPing();
        }
    }
    public override void OnNetworkDespawn()
    {
        if (NetworkManager.CustomMessagingManager != null)
        {
            NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(MessageName);
        }
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisConnected;
    }
    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        double sendTime;
        double rtt;
        messagePayload.ReadValueSafe(out sendTime);
        if (IsServer)
        {
            if (_smoothedRTT.TryGetValue(senderId, out var smoothedRTT))
            {
                rtt = (NetworkManager.RealTimeProvider.RealTimeSinceStartup * 1000.0f - sendTime);
                smoothedRTT = (1 - Alpha) * smoothedRTT + Alpha * rtt;
                _smoothedRTT[senderId] = smoothedRTT;
            }
        }
        else
        {
            messagePayload.ReadValueSafe(out rtt);
            _smoothedRTT[NetworkManager.ServerClientId] = rtt;
            OnRttChanged?.Invoke();
            SendPong(senderId, sendTime);
        }
    }
    private void SendPong(ulong clientId, double receivedTime)
    {
        var writer = new FastBufferWriter(sizeof(double), Allocator.Temp);
        var customMessagingManager = NetworkManager.CustomMessagingManager;
        using (writer)
        {
            writer.WriteValueSafe(receivedTime);
            customMessagingManager.SendNamedMessage(MessageName, clientId, writer);
        }
    }
    private void SendPing()
    {
        foreach (var clientID in _smoothedRTT.Keys.ToArray())
        {
            if (_smoothedRTT.TryGetValue(clientID, out double rtt))
            {
                double sendTime = NetworkManager.RealTimeProvider.RealTimeSinceStartup * 1000;
                var writer = new FastBufferWriter(sizeof(double) * 2, Allocator.Temp);
                using (writer)
                {
                    writer.WriteValueSafe(sendTime);
                    writer.WriteValueSafe(rtt);
                    NetworkManager.CustomMessagingManager.SendNamedMessage(MessageName, clientID, writer);
                }
            }
        }
    }
    public bool GetRtt(ulong clientId, out double rtt)
    {
        rtt = 0;
        var jitter = UnityEngine.Random.Range(-_jitterRange, _jitterRange);
        if (_useVirtualRtt)
        {
            rtt = _virtualRtt + jitter;
            return true;
        }
        else if (_smoothedRTT.TryGetValue(clientId, out rtt))
        {
            rtt += jitter;
            return true;
        }
        return false;
    }
}
