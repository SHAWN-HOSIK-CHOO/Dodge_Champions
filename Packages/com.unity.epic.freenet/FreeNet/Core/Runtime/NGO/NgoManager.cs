#define CUSTUMNETCODEFIX
using Epic.OnlineServices.P2P;
using System;
using Unity.Netcode;
using UnityEngine;
using static NetworkSpawner;

public class NgoManager : NetworkManager
{
    /* 
     * - NGO�� ���� ��Ű���� ���� CUSTUMNETCODEFIX �� Define �Ͽ� ��� �ڵ带 ������
     * 
     * NetworkTickSystem Tick
     * Ŭ���̾�Ʈ�� Tick ��ũ �ӵ��� �����ϱ� ���� ���� �ð����� �ѹ��Ҷ�
     * ���̰� ���� ��� + ���� �ð��� ���ŷ� ���ư� ��쿡 �ѹ� �ǵ��� ����.     
     * ���� ���� ��Ȳ���� Tick Reset�� �߻��Ҷ� Tick�� ������ ���� �ݺ��ϰų� �ߺ� Ȥ�� Skip ȣ�� ���� ������ ��. 
     * 
     * Ngo Transform $ Interporation
     * ���� Tick�� ĳ���Ͽ� �ռ� Tick�� ��쿡�� Transform�� Update�ϰ� ���� -> �ѹ鿡 ���� �ùٸ��� ��ó���� ���� ��..
     * ������ ������ ���������� ������ ���� �ֽ��� ������ ������ �����ϰ� ���� -> �ùٸ� �߰� ��θ� �ùķ��̼� ���� ����..
     * Ŭ���̾�Ʈ �������� �̵� ������ �ʿ��� -> transform �ۿ� ������ ��� �����ϰ� ���� ����..
     *
     * BuildProfile with PlayMode
     * ��Ÿ�� �������� ����Ǿ� PlayMode�� ���ε� �� ���� ��� ����ȭ�� �� ���� ����
     * �����Ϳ� ���� �������� �и��Ͽ� �ڵ带 �߰� �ۼ���.
     *
     * Optimization
     * �� ������ Send�� ���� ���� ��� ó���� -> �ʿ��ϴٸ� ť�� ���� ���� ������ �ʿ���
     * ���� �ֱ� ���� ��Ŷ�� Receive �� -> �ִ� TickInterval ��ŭ�� ��Ŷ ó�� ���� �߻�
     * Urgent ��Ŷ�� ������ �ٷ� ó���Ҽ� �ְ� �Ͽ��� -> To DO : �߸��� ����ΰ�? ���״� ����?  
     * 
     * To Do With NetCode...
     * 
     * Ŭ���̾�Ʈ�� ���� �� �Է� �����丮�� ���� ��.
     * ������ Ŭ���̾�Ʈ�� ��û �����丮�� ���� ��.
     * 
     * Ŭ���̾�Ʈ�� ��û �� ���� ������ �����ϰ�
     * ���� ƽ�� ����ʿ� ���� Ŭ���̾�Ʈ ��û�� ó���ϰ� ����� Ŭ���̾�Ʈ���� ��ȯ�ϸ� 
     * Ŭ���̾�Ʈ�� ���� ���� ����� ���Ͽ� ������ �Ѵ�.
     * 
     */

    FreeNet _freeNet;
    EOSNetcodeTransport _EOSNetcodeTransport;

    [SerializeField]
    public double _localBufferSec;

    [SerializeField]
    public double _serverBufferSec;
    [SerializeField]
    public bool _useEpicOnlineTransport;

    public byte _channel => 0;
    public byte _urgentChannel => 1;

    [SerializeField]
    NetworkSpawner _networkSpawnerPref;

    public NetworkSpawner _networkSpawner;
    public Action _onSpawnerSpawned;
    public event Action _onNgoManagerReady;
    public event Action _onTick;

    public void Init(FreeNet freeNet)
    {
        _freeNet = freeNet;
        _EOSNetcodeTransport = GetComponent<EOSNetcodeTransport>();
    }
    public void SetNetworkValue()
    {
        if(NetworkTimeSystem != null)
        {
            NetworkTimeSystem.LocalBufferSec = _localBufferSec;
            NetworkTimeSystem.ServerBufferSec = _serverBufferSec;
        }

        if(MessageManager!= null)
        {
            MessageManager.NonFragmentedMessageMaxSize = P2PInterface.MaxPacketSize;
        }
    }
    public bool StartServer(EOSWrapper.ETC.PUID localPUID, string socketName)
    {
        var result = false;
        if (_useEpicOnlineTransport)
        {
            result = _EOSNetcodeTransport.InitializeEOSServer(_freeNet._eosCore, localPUID, socketName, _channel,_urgentChannel) && StartServer();
        }
        else
        {
            result = base.StartServer();
        }
        
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
            _networkSpawner = Instantiate(_networkSpawnerPref);
            _networkSpawner.GetComponent<NetworkObject>().Spawn(false);
            MessageManager.NonFragmentedMessageMaxSize = P2PInterface.MaxPacketSize;
        }
        return result;
    }
    public bool StartClient(EOSWrapper.ETC.PUID localPUID, EOSWrapper.ETC.PUID remotePUID, string socketName)
    {
        var result = false;
        if (_useEpicOnlineTransport)
        {
            result = _EOSNetcodeTransport.InitializeEOSClient(_freeNet._eosCore, localPUID, remotePUID, socketName, _channel, _urgentChannel) && StartClient();
        }
        else
        {
            result =  base.StartClient();
        }
        if (result)
        {
            SetNetworkValue();
            _onSpawnerSpawned += OnSpawnedSpawner;
        }
        return result;
    }
    public bool StartHost(EOSWrapper.ETC.PUID localPUID, string socketName)
    {
        var result = false;
        if (_useEpicOnlineTransport)
        {
            result = _EOSNetcodeTransport.InitializeEOSServer(_freeNet._eosCore, localPUID, socketName, _channel, _urgentChannel) &&
            _EOSNetcodeTransport.InitializeEOSClient(_freeNet._eosCore, localPUID, localPUID, socketName, _channel, _urgentChannel) &&
            StartHost() && _EOSNetcodeTransport.StartClient();
        }
        else
        {
            result = base.StartHost();
        }
        if(result)
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
        if (IsServer)
        {
            var obj = _networkSpawner.Spawn(new SpawnParams()
            {
                prefabListName = "NetPrefabs",
                prefabName = "PingPong",
                destroyWithScene = false
            });
        }
        base.NetworkTickSystem.Tick += _onTick;
        _onNgoManagerReady?.Invoke();
    }
}