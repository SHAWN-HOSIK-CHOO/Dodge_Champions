#define CUSTUMNETCODEFIX
using Epic.OnlineServices.P2P;
using System;
using Unity.Netcode;
using UnityEngine;
using static NetworkSpawner;

public class NgoManager : NetworkManager
{
    /* 
     * �Ʒ��� NGO�� �� �� ������ ������
     * �̿� ��� �ڵ� ������ ������ ����. �ڼ��Ѱ� cirl + q �� CUSTUMNETCODEFIX Ž���� �ּ� Ȯ��
     * 
     * -NGO Time System Issue
     * NGO �� �ǵ����� Ŭ���̾�Ʈ ������ �����Ͽ� ���������� Ŭ���̾�Ʈ�� ���� ���¸� ���󰡵��� �Ѵ�.
     * �̸� ���� Ŭ���̾�Ʈ�� ���� �����ð� ���� ���� LocalTime, ���� ServerTime�� ���� 
     * Ŭ���̾�Ʈ�� ����ȭ ��Ŷ���� ������ Tick�� �͸������� �޾Ƶ��̵��� ����Ǿ�����.
     * �� �������� Ŭ���̾�Ʈ�� Tick�� �ѹ�� �� ������, Network Tick Loop �� ��ŵ�ǰų� �ߺ� ����ǰų� ������ ���� �ݺ��ϴ� ��Ȳ�� �߻��� �� ����.
     * 
     * - NGO Transform System Issue
     * ���� NGO�� ���� Tick�� ĳ���Ͽ� �׺��� �ռ� Tick�� ��쿡�� Transform�� Update �ϴ� ����� ���ϰ� ����
     * 
     * �������� �ʴ´ٸ� Transform�� �޴� ��� ������Ʈ �Ѵ�.
     * Transform ��Ŷ�� �ս� ���ɼ��� ����     
     * 
     * - Interporation
     * Last Sync Tranfrom �׸��� ������ ���� ���� Last Transform ���̿��� ������ ����.
     * ���� ��Ʈ��ũ�� Burst(������ ������ �Ѳ����� ���� ���) �ɶ� �ùٸ� �߰� ��θ� �ùķ��̼� ���� �ʴ´�.
     * Burst�� �����ϰ��� ServerTime ���� 2ƽ �� ������ �ð����� ���� ���� ����ϴµ�
     * �̶� �ε巯���� �����ϰ��� �ѹ� �� 2�� �����Ѵ�. (default 0.1f) 
     * Trnasform ������ ������� ������ ��ȭ�� ������ �� �������� �ܻ���� �� �� ���� 
     * -> Ŭ���̾�Ʈ ������ ���ؼ��� RPC�� ����� ������ Move ������ �ʿ�
     *
     * - optimization
     * ���������� �˾Ƽ� �� ��Ʈ ��ŷ�ϰ� ����.
     * �� ������ ���� ��� ��Ŷ�� Send, ���� �ֱ� ���� Receive �� �ϰ� ���� 
     * TODO : Transport Layer���� ť���Ͽ� Send ��Ʈ��ũ ���� ������ �ؾ� �� ��,
     * TODO? : Urgent ��Ŷ�� ���� ��� Receive�� �����ϰ� �� (ipnut �����Ͻ� �ּ�ȭ)
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