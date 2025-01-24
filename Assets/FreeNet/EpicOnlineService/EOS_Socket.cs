
using Epic.OnlineServices;
using Epic.OnlineServices.P2P;
using System.Collections.Generic;
using System;
using UnityEngine;
using static EOSNet;

public class EOS_Socket
{
    public ProductUserId _localPUID { get; private set; }
    public SocketId _socketID { get; private set; }
    DoubleKeyDict<EOSNet.Role, string, Connection> _Connections;
    public class Connection
    {
        public enum State
        {
            Connected,
            Disconnected,
        }
        public State _State { get; private set; }
        public EOSNet.Role _remoteRole { get; private set; }
        public ProductUserId _remotePUID { get; private set; }
        public event Action<Connection> _onEnqueuePacket;
        public event Action<Connection> _onConnectionStateChanged;
        private Queue<EOSNet.EOS_Packet> _IncomingPackets;
        public void SetState(State state)
        {
            if (_State != state)
            {
                _State = state;
                _onConnectionStateChanged?.Invoke(this);
            }
        }
        public void Release()
        {
            SetState(State.Disconnected);
            _onEnqueuePacket = null;
            _onConnectionStateChanged = null;
        }
        public bool DeqeuePacket(out EOSNet.EOS_Packet packet)
        {
            return _IncomingPackets.TryDequeue(out packet);
        }
        public void EnqueuePacket(EOSNet.EOS_Packet packet)
        {
            _IncomingPackets.Enqueue(packet);
            _onEnqueuePacket?.Invoke(this);
        }

        public Connection(EOSNet.Role role, string remotepuid = null)
        {
            _IncomingPackets = new Queue<EOSNet.EOS_Packet>();
            _remoteRole = role;
            _remotePUID = ProductUserId.FromString(remotepuid);
            _State = State.Disconnected;
        }
    }
    #region callbacks
    public event Action<Connection> _onMakeConnection;
    public event Action<OnRemoteConnectionClosedInfo> _onClosed;
    event Action<OnIncomingConnectionRequestInfo> _onRequest;
    event Action<OnPeerConnectionEstablishedInfo> _onEstablished;
    public event Action<OnPeerConnectionInterruptedInfo> _onInterrupted;
    #endregion
    #region EOScallbacks
    ulong _onRequestHandle;
    ulong _onEstablishedHandle;
    ulong _onInterruptedHandle;
    ulong _onClosedHandle;
    #endregion
    public EOS_Socket(string localpuid, string socketid)
    {
        _localPUID = ProductUserId.FromString(localpuid);
        _socketID = new SocketId() { SocketName = socketid };
        _Connections = new DoubleKeyDict<Role, string, Connection>();
    }
    void AddRequestCB()
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        _onRequest += OnRequest;
        if(_onRequestHandle == 0)
        {
            var Requestoptions = new AddNotifyPeerConnectionRequestOptions()
            {
                LocalUserId = _localPUID,
                SocketId = _socketID
            };
            _onRequestHandle = eosNet._IP2P.AddNotifyPeerConnectionRequest(ref Requestoptions, Role.RemotePeer, (ref OnIncomingConnectionRequestInfo info) =>
            {
                _onRequest?.Invoke(info);
            });
        }
    }
    void AddClosedCB()
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        _onClosed += OnClosed;
        if (_onClosedHandle == 0)
        {
            var ClosedOptions = new AddNotifyPeerConnectionClosedOptions()
            {
                LocalUserId = _localPUID,
                SocketId = _socketID
            };
            _onClosedHandle = eosNet._IP2P.AddNotifyPeerConnectionClosed(ref ClosedOptions, Role.RemotePeer, (ref OnRemoteConnectionClosedInfo info) =>
            {
                _onClosed?.Invoke(info);
            });
        }
    }
    void AddEstablishedCB()
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        _onEstablished += OnEstablished;
        if(_onEstablishedHandle == 0)
        {
            var options = new AddNotifyPeerConnectionEstablishedOptions()
            {
                LocalUserId = _localPUID,
                SocketId = _socketID
            };
            _onEstablishedHandle = eosNet._IP2P.AddNotifyPeerConnectionEstablished(ref options, Role.RemotePeer, (ref OnPeerConnectionEstablishedInfo info) =>
            {
                _onEstablished?.Invoke(info);
            });
        }
    }
    void AddInterruptedCB()
    {
        _onInterrupted += OnInterrupted;
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        if(_onInterruptedHandle==0)
        {
            var options = new AddNotifyPeerConnectionInterruptedOptions()
            {
                LocalUserId = _localPUID,
                SocketId = _socketID
            };
            _onInterruptedHandle = eosNet._IP2P.AddNotifyPeerConnectionInterrupted(ref options, Role.RemotePeer, (ref OnPeerConnectionInterruptedInfo info) =>
            {
                _onInterrupted?.Invoke(info);
            });
        }
    }
    public void RemoveCB()
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        eosNet._IP2P.RemoveNotifyPeerConnectionRequest(_onRequestHandle);
        eosNet._IP2P.RemoveNotifyPeerConnectionInterrupted(_onInterruptedHandle);
        eosNet._IP2P.RemoveNotifyPeerConnectionEstablished(_onEstablishedHandle);
        eosNet._IP2P.RemoveNotifyPeerConnectionClosed(_onClosedHandle);

        _onRequest = null;
        _onClosed = null;
        _onEstablished = null;
        _onInterrupted = null;
    }
    public void AddCB()
    {
        AddRequestCB();
        AddClosedCB();
        AddEstablishedCB();
        AddInterruptedCB();
    }
    public void OnRequest(OnIncomingConnectionRequestInfo info)
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        Role remoterole = (Role)info.ClientData;
        if (remoterole == Role.RemotePeer)
        {
            if (!EOSWrapper.P2PControl.AcceptConnection(eosNet._IP2P, _localPUID, info.RemoteUserId, _socketID))
            {
                Debug.LogError($"���� ����");
            }
        }
        else
        {
            _onEstablished?.Invoke(new OnPeerConnectionEstablishedInfo()
            {
                ClientData = remoterole,
                LocalUserId = _localPUID,
                RemoteUserId = info.RemoteUserId,
                SocketId = _socketID,
            });

            if (remoterole == Role.localClient)
            {
                _onRequest?.Invoke(new OnIncomingConnectionRequestInfo()
                {
                    ClientData = Role.localHost,
                    LocalUserId = _localPUID,
                    RemoteUserId = info.RemoteUserId,
                    SocketId = _socketID,
                });
            }
        }
    }
    public void OnClosed(OnRemoteConnectionClosedInfo info)
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        EOSNet.Role remoterole = (EOSNet.Role)info.ClientData;
        if (_Connections.TryGetValue(remoterole, info.RemoteUserId.ToString(), out var connection))
        {
            connection.Release();
            _Connections.Remove(remoterole, info.RemoteUserId.ToString());

            if (remoterole == EOSNet.Role.localClient)
            {
                StopConnect(EOSNet.Role.localHost, info.RemoteUserId.ToString());
            }
            else if (remoterole == EOSNet.Role.localHost)
            {
                StopConnect(EOSNet.Role.localClient, info.RemoteUserId.ToString());
            }
        }
    }
    public void OnEstablished(OnPeerConnectionEstablishedInfo info)
    {
        EOSNet.Role remoterole = (EOSNet.Role)info.ClientData;
        var connection = new Connection(remoterole, info.RemoteUserId.ToString());
        _Connections.TryAdd(remoterole, connection._remotePUID.ToString(), connection);
        _onMakeConnection?.Invoke(connection);
        connection.SetState(Connection.State.Connected);
    }
    public void OnInterrupted(OnPeerConnectionInterruptedInfo info)
    {

    }
    public bool GetConnection(EOSNet.Role role, string remotePUID,out Connection Outconnection)
    {
       return _Connections.TryGetValue(role, remotePUID, out Outconnection);
    }
    public void StartConnect(ProductUserId remotePUID)
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        if( _localPUID.ToString()== remotePUID.ToString())
        {
            _onRequest?.Invoke(new OnIncomingConnectionRequestInfo()
            {
                ClientData = Role.localClient,
                LocalUserId = _localPUID,
                RemoteUserId = remotePUID,
                SocketId = _socketID,
            });
        }
        else
        {
            if (!EOSWrapper.P2PControl.AcceptConnection(eosNet._IP2P, _localPUID, remotePUID, _socketID))
            {
                Debug.LogError($"���� ����");
            }
        }
    }
    public void StopConnect(EOSNet.Role role, string remotePUID)
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        if(GetConnection(role,remotePUID,out var connection))
        {
            if (connection._remoteRole == Role.RemotePeer)
            {
                if (!EOSWrapper.P2PControl.CloseConnection(eosNet._IP2P, _localPUID, connection._remotePUID, _socketID))
                {
                    Debug.LogError($"���� ���� ����");
                }
            }
            else
            {
                _onClosed?.Invoke(new OnRemoteConnectionClosedInfo()
                {
                    ClientData = connection._remoteRole,
                    LocalUserId = _localPUID,
                    RemoteUserId = connection._remotePUID,
                    SocketId = _socketID,
                });
            }
        }
    }
    public void StopAllConnect()
    {
        var eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        foreach(var kvp in _Connections.GetEnumerator())
        {
            StopConnect(kvp.Key1, kvp.Key2);
        }
    }

}