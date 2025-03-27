#define CUSTUMNETCODEFIX
using Epic.OnlineServices.P2P;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NetworkSpawner;

public class NgoManager : NetworkManager
{
    /* 
     * NetworkTickSystem Tick
     * Ŭ���̾�Ʈ�� Tick ��ũ �ӵ��� �����ϱ� ���� ���� �ð����� �ѹ��ϰ� ���� ->�����ð��� �ǵ��ư��� �ʴ´ٸ� �ѹ����� ����
     * Ŭ��� ������ Tick�� �����ϵ��� ����� -> ȣ��Ʈ Ŭ���̾�Ʈ �𵨿��� ����ٸ� ƽ�� ���� �ʿ䰡 ���� �������� Tick�� �ٲ㵵 ���� ������
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

    [SerializeField]
    private List<string> _networkScene;
    public void Init(FreeNet freeNet)
    {
        _freeNet = freeNet;
        _EOSNetcodeTransport = GetComponent<EOSNetcodeTransport>();
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
        var result = base.StartClient();
        if (result)
        {
            SetNetworkValue();
            SceneManager.VerifySceneBeforeLoading = NetworkSceneValidation;
            _onSpawnerSpawned += OnSpawnedSpawner;
        }
        return result;
    }
    public bool StartClient(EOSWrapper.ETC.PUID localPUID, EOSWrapper.ETC.PUID remotePUID, string socketName)
    {
        var result = _EOSNetcodeTransport.InitializeEOSClient(_freeNet._eosCore, localPUID, remotePUID, socketName, _channel, _urgentChannel) && base.StartClient();
        if (result)
        {
            SetNetworkValue();
            SceneManager.VerifySceneBeforeLoading = NetworkSceneValidation;
            _onSpawnerSpawned += OnSpawnedSpawner;
        }
        return result;
    }
    private bool NetworkSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
    {
        int index = _networkScene.FindIndex(x => x == sceneName);
        if (index != -1)
        {
            return true;
        }
        return false;
    }
    public new bool StartHost()
    {
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
        if (IsServer)
        {
            var obj = _networkSpawner.Spawn(new SpawnParams()
            {
                prefabListName = "NetPrefabs",
                prefabName = "PingPong",
                destroyWithScene = false
            });
        }
        _onNgoManagerReady?.Invoke();
    }

}