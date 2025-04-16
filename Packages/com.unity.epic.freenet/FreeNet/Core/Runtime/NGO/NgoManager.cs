using Epic.OnlineServices.P2P;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NgoManager : NetworkManager
{
    FreeNet _freeNet;
    [SerializeField]
    public double _localBufferSec;
    [SerializeField]
    public double _serverBufferSec;
    public byte _channel => 0;
    public byte _urgentChannel => 1;
    public bool _ngoReady { get; private set; }

    [SerializeField]
    NetworkSpawner _networkSpawnerPref;
    [SerializeField]
    EOSNetcodeTransport _EOSNetcodeTransport;
    public NetworkSpawner _networkSpawner;
    public Action _onSpawnerSpawned;
    public event Action _onNgoManagerReady;

    public static NgoManager Instance => (NetworkManager.Singleton as NgoManager);
    public void Init(FreeNet freeNet)
    {
        _freeNet = freeNet;
    }
    public void SetNetworkValue()
    {
        if (NetworkTimeSystem != null)
        {
            NetworkTimeSystem.LocalBufferSec = _localBufferSec;
            NetworkTimeSystem.ServerBufferSec = _serverBufferSec;
        }

        if (MessageManager != null)
        {
            MessageManager.NonFragmentedMessageMaxSize = P2PInterface.MaxPacketSize;
        }
    }
    public new bool StartServer()
    {
        _ngoReady = false;
        var result = base.StartServer();
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
            _networkSpawner = Instantiate(_networkSpawnerPref);
            _networkSpawner.GetComponent<NetworkObject>().Spawn(false);
        }
        return result;
    }
    public bool StartServer(EOSWrapper.ETC.PUID localPUID, string socketName)
    {
        _ngoReady = false;
        var result = _EOSNetcodeTransport.InitializeEOSServer(_freeNet._eosCore, localPUID, socketName, _channel, _urgentChannel) && base.StartServer();
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
            _networkSpawner = Instantiate(_networkSpawnerPref);
            _networkSpawner.GetComponent<NetworkObject>().Spawn(false);
        }
        return result;
    }
    public new bool StartClient()
    {
        _ngoReady = false;
        var result = base.StartClient();
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
        }
        return result;
    }
    public bool StartClient(EOSWrapper.ETC.PUID localPUID, EOSWrapper.ETC.PUID remotePUID, string socketName)
    {
        _ngoReady = false;
        var result = _EOSNetcodeTransport.InitializeEOSClient(_freeNet._eosCore, localPUID, remotePUID, socketName, _channel, _urgentChannel) && base.StartClient();
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
        }
        return result;
    }
    public new bool StartHost()
    {
        _ngoReady = false;
        var result = base.StartHost();
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
            _networkSpawner = Instantiate(_networkSpawnerPref);
            _networkSpawner.GetComponent<NetworkObject>().Spawn(false);
        }
        return result;
    }
    public bool StartHost(EOSWrapper.ETC.PUID localPUID, string socketName)
    {
        var result = _EOSNetcodeTransport.InitializeEOSServer(_freeNet._eosCore, localPUID, socketName, _channel, _urgentChannel) &&
            _EOSNetcodeTransport.InitializeEOSClient(_freeNet._eosCore, localPUID, localPUID, socketName, _channel, _urgentChannel) &&
            StartHost() && _EOSNetcodeTransport.StartClient();
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
            _networkSpawner = Instantiate(_networkSpawnerPref);
            _networkSpawner.GetComponent<NetworkObject>().Spawn(false);
        }
        return result;
    }
    public void SendUrgentPacket(ulong clientID)
    {
        var transportID = ConnectionManager.ClientIdToTransportId(clientID);
        _EOSNetcodeTransport.SendUrgentPacket(transportID);
    }
    void OnSpawnedSpawner()
    {
        _onSpawnerSpawned -= OnSpawnedSpawner;
        _ngoReady = true;
        _onNgoManagerReady?.Invoke();
    }
}