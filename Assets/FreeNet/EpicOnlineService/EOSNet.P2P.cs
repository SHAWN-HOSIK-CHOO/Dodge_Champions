

using Epic.OnlineServices;
using Epic.OnlineServices.P2P;
using System;
using UnityEngine.UIElements;

public partial class EOSNet : SingletonMonoBehaviour<EOSNet>
{
    // LocalPUID socketID
    DoubleKeyDict<string, string, EOS_Socket> _sockets;
    SocketId _cashedsocketID;
    ProductUserId _cashedProductUserID;
    public struct EOS_Packet
    {
        public string _socketName;
        public byte _channel;
        public ArraySegment<byte> _data;
    }
    public enum Role
    {
        localHost,
        localClient,
        RemotePeer,
    }
    public EOSNet()
    {
        _sockets = new DoubleKeyDict<string, string, EOS_Socket> ();
    }

    public EOS_Socket CreateSocket(string localpuid, string socketid)
    {
        if(_sockets.TryGetValue(localpuid,socketid,out var oldSock))
        {
            return oldSock;
        }
        else
        {
            var socket = new EOS_Socket(localpuid, socketid);
            if (_sockets.TryAdd(localpuid.ToString(), socketid, socket))
            {
                socket.AddCB();
                return socket;
            }
            return null;
        }
    }
    public void RemoveSocket(EOS_Socket socket)
    {
        socket.StopAllConnect();
        socket.RemoveCB();
        _sockets.Remove(socket._localPUID.ToString(), socket._socketID.SocketName);
    }
    public bool GetSocket(string localpuid, string socketid, out EOS_Socket socket)
    {
        return _sockets.TryGetValue(localpuid, socketid, out socket);
    }
    public void SendPeer(EOS_Socket socket, ProductUserId remotePUID, byte channel, ArraySegment<byte> data , PacketReliability reliability)
    {
        var packet = new EOS_Packet()
        {
            _socketName = socket._socketID.SocketName,
            _channel = channel,
            _data = data
        };
        EOSWrapper.P2PControl.SendPacket(_IP2P, new SendPacketOptions()
        {
            LocalUserId = socket._localPUID,
            RemoteUserId = remotePUID,
            SocketId = socket._socketID,
            Channel = channel,
            Data = data,
            Reliability = reliability,
            AllowDelayedDelivery = true,
            DisableAutoAcceptConnection = true
        });
    }
    public void SendLocal(EOS_Socket socket,Role role, byte channel, ArraySegment<byte> data)
    {
        var packet = new EOS_Packet()
        {
            _socketName = socket._socketID.SocketName,
            _channel = channel,
            _data = data.ToArray()
        };
        if (socket.GetConnection(role, socket._localPUID.ToString(),out var connection))
        {
            connection.EnqueuePacket(packet);
        }
    }
    private void ReceivePacket()
    {
        foreach (var key in _sockets.GetKeys1())
        {
            var localID = ProductUserId.FromString(key);
            if (EOSWrapper.P2PControl.GetNextReceivedPacketSize(_IP2P, localID, out uint nextBytes, null))
            {
                if (EOSWrapper.P2PControl.ReceiveNextPacket(_IP2P, localID, ref _cashedProductUserID, ref _cashedsocketID, nextBytes, out ArraySegment<byte> dataSegment, out byte channel))
                {
                    var newpacket = new EOS_Packet()
                    {
                        _socketName = _cashedsocketID.SocketName,
                        _channel = channel,
                        _data = dataSegment
                    };
                    string remotePUID = _cashedProductUserID.ToString();
                    if(_sockets.TryGetValue(key, _cashedsocketID.SocketName,out var socket))
                    {
                        if(socket.GetConnection(Role.RemotePeer, remotePUID, out var connection))
                        {
                            connection.EnqueuePacket(newpacket);
                        }
                    }
                }
            }
        }
    }
}