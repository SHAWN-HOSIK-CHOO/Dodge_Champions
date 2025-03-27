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
     * 클라이언트가 Tick 싱크 속도를 조정하기 위해 이전 시간으로 롤백하고 있음 ->서버시간이 되돌아가지 않는다면 롤백하지 않음
     * 클라와 서버의 Tick이 동일하도록 설계됨 -> 호스트 클라이언트 모델에서 벗어난다면 틱이 같을 필요가 없고 동적으로 Tick을 바꿔도 되지 않을까
     * 
     * Ngo Transform $ Interporation
     * 이전 Tick을 캐싱하여 앞선 Tick인 경우에만 Transform을 Update하고 있음 -> 롤백에 대해 올바르게 대처하지 못할 것..
     * 보간시 이전의 보간정보를 버리고 가장 최신의 정보만 가지고 보간하고 있음 -> 올바른 중간 경로를 시뮬레이션 하지 않음..
     * 클라이언트 예측에는 이동 정보가 필요함 -> transform 밖에 정보가 없어서 지원하고 있지 않음..
     *
     * BuildProfile with PlayMode
     * 런타임 기준으로 설계되어 PlayMode시 씬로드 및 빌드 목록 동기화가 잘 되지 않음
     * 에디터와 빌드 기준으로 분리하여 코드를 추가 작성함.
     *
     * Optimization
     * 매 프레임 Send를 제한 없이 모두 처리함 -> 필요하다면 큐잉 등의 부하 관리가 필요함
     * 고정 주기 마다 패킷을 Receive 함 -> 최대 TickInterval 만큼의 패킷 처리 지연 발생
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