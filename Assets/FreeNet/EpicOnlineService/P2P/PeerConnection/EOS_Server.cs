using Epic.OnlineServices;
using Epic.OnlineServices.P2P;
using System;
using System.Collections.Generic;
using System.Linq;
using static EOS_Socket;

public class EOS_Server : EOS_Peer
{
    public string _localPUID { get; private set; }
    private Dictionary<int, string> _ConnectIDmapping;
    private Dictionary<string, int> _PUIDmapping;
    public Queue<(int, ArraySegment<byte>)> _incomingPacket;
    public event Action<(int, EOS_Core.EOS_Packet)> _onReceivedPacket;
    public event Action<(int, EOS_Socket.Connection)> _onConnectionStateChanged;

    public delegate int GetNewConnectID(EOS_Core.Role role);
    GetNewConnectID _GetNewConnectIDDelegate;
    public void Init(string localPUID, string socketid, byte channel, GetNewConnectID getNewConnectIDDelegate)
    {
        _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
        _state = state.stop;
        _localPUID = localPUID;
        _GetNewConnectIDDelegate = getNewConnectIDDelegate;
        _socket = _eosNet.CreateSocket(localPUID, socketid);
        _socket._onMakeConnection -= OnMakeConnectionCB;
        _socket._onClosed -= OnClosedCB;
        _socket._onMakeConnection += OnMakeConnectionCB;
        _socket._onClosed += OnClosedCB;
        _channel = channel;
        _eosNet = SingletonMonoBehaviour<EOS_Core>._instance;
        _ConnectIDmapping = new Dictionary<int, string>();
        _PUIDmapping = new Dictionary<string, int>();
        _incomingPacket = new Queue<(int, ArraySegment<byte>)>();
    }
    public void RemoveMapping(string puid)
    {
        if (GetConnectID(puid, out var id))
        {
            _ConnectIDmapping.Remove(id);
        }
        _PUIDmapping.Remove(puid);
    }
    public void RemoveMapping(int id)
    {
        if (GetPUID(id, out var puid))
        {
            _PUIDmapping.Remove(puid);
        }
        _ConnectIDmapping.Remove(id);
    }
    public void RemoveMapping()
    {
        _ConnectIDmapping.Clear();
        _PUIDmapping.Clear();
    }
    public bool GetConnection(int id, out EOS_Socket.Connection connection)
    {
        connection = null;
        if (GetPUID(id, out var puid))
        {
            if (_localPUID == puid)
            {
                return _socket.GetConnection(EOS_Core.Role.localClient, puid, out connection);
            }
            else
            {
                return _socket.GetConnection(EOS_Core.Role.RemotePeer, puid, out connection);
            }
        }
        return false;
    }
    public bool GetConnectID(string puid, out int id)
    {
        return _PUIDmapping.TryGetValue(puid, out id);
    }
    public bool GetPUID(int id, out string puid)
    {
        return _ConnectIDmapping.TryGetValue(id, out puid);
    }
    public IEnumerable<(int, string)> GetEnumerable()
    {
        foreach (var kp in _ConnectIDmapping.ToList())
        {
            yield return (kp.Key, kp.Value);
        }
    }
    public int AddMapping(int newID,string remotePUID)
    {
        if (_PUIDmapping.TryGetValue(remotePUID, out int id))
        {
            return id;
        }
        else
        {
            _ConnectIDmapping.Add(newID, remotePUID);
            _PUIDmapping.Add(remotePUID, newID);
            return newID;
        }
    }
    public void OnServerEnqueuePacket(EOS_Socket.Connection connection)
    {
        if (connection.DeqeuePacket(out var packet))
        {
            if (GetConnectID(connection._remotePUID.ToString(), out int id))
            {
                _incomingPacket.Enqueue((id, packet._data));
                _onReceivedPacket?.Invoke((id,packet));
            }
        }
    }
    public void SendToClient(byte channelId, ArraySegment<byte> segment, int connectionId, PacketReliability reliability)
    {
        if (GetPUID(connectionId, out var puid))
        {
            if(puid == _localPUID)
            {
                _eosNet.SendLocal(_socket, EOS_Core.Role.localClient, _channel, segment);

            }
            else
            {
                _eosNet.SendPeer(_socket, ProductUserId.FromString(puid), _channel, segment, reliability);
            }
        }
    }
    public override void OnMakeConnectionCB(Connection info)
    {
        var remoterole = (EOS_Core.Role)info._remoteRole;
        if (remoterole != EOS_Core.Role.localHost)
        {
            AddMapping(_GetNewConnectIDDelegate(remoterole), info._remotePUID.ToString());
            if (_socket.GetConnection(remoterole, info._remotePUID.ToString(), out var connection))
            {
                connection._onEnqueuePacket += OnServerEnqueuePacket;
                connection._onConnectionStateChanged += OnConnectionStateChangedCB;
            }
        }
    }
    void OnConnectionStateChangedCB(Connection connection)
    {
        if (GetConnectID(connection._remotePUID.ToString(), out int id))
        {
            _onConnectionStateChanged?.Invoke((id, connection));
        }
    }
    public override void OnClosedCB(OnRemoteConnectionClosedInfo info)
    {
        var remoterole = (EOS_Core.Role)info.ClientData;
        if (remoterole != EOS_Core.Role.localHost)
        {
            if (GetConnectID(info.RemoteUserId.ToString(), out var id))
            {
                RemoveMapping(info.RemoteUserId.ToString());
                if (_socket.GetConnection(remoterole, info.RemoteUserId.ToString(), out var connection))
                {
                    connection._onEnqueuePacket -= OnServerEnqueuePacket;
                    connection._onConnectionStateChanged -= OnConnectionStateChangedCB;
                }
            }
        }
    }
    public bool StopConnection(string puid)
    {
        if (_localPUID == puid)
        {
            _socket.StopConnect(EOS_Core.Role.localClient, puid);
        }
        else
        {
            _socket.StopConnect(EOS_Core.Role.RemotePeer, puid);
        }
        RemoveMapping(puid);
        return true;
    }
    public bool StopConnection(int connectionId)
    {
        if (GetPUID(connectionId, out var puid))
        {
            if (_localPUID == puid)
            {
                _socket.StopConnect(EOS_Core.Role.localClient, puid);
            }
            else
            {
                _socket.StopConnect(EOS_Core.Role.RemotePeer, puid);
            }
            RemoveMapping(connectionId);
        }
        return true;
    }
    public override bool StopConnection()
    {
        foreach (var kvp in GetEnumerable())
        {
            StopConnection(kvp.Item2);
        }
        return true;
    }
    public override bool StartConnection()
    {
        ChangeState(state.start);
        return true;
    }
    public override void Shutdown()
    {
        StopConnection();
        if (_socket != null)
        {
            _socket._onMakeConnection -= OnMakeConnectionCB;
            _socket._onClosed -= OnClosedCB;
        }
    }
}
