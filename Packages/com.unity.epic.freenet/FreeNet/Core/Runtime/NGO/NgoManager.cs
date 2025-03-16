#define CUSTUMNETCODEFIX
using Epic.OnlineServices.P2P;
using System;
using Unity.Netcode;
using UnityEngine;
using static NetworkSpawner;

public class NgoManager : NetworkManager
{
    /* 
     * - NGO를 로컬 패키지로 뺀뒤 CUSTUMNETCODEFIX 를 Define 하여 몇몇 코드를 수정함
     * 
     * NetworkTickSystem Tick
     * 클라이언트가 Tick 싱크 속도를 조정하기 위해 이전 시간으로 롤백할때
     * 차이가 심한 경우 + 서버 시간이 과거로 돌아간 경우에 롤백 되도록 변경.     
     * 위와 같은 상황에서 Tick Reset이 발생할때 Tick이 과거의 값을 반복하거나 중복 혹은 Skip 호출 됨을 주의할 것. 
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
     * Urgent 패킷을 보내어 바로 처리할수 있게 하였음 -> To DO : 잘못된 사용인가? 버그는 없나?  
     * 
     * To Do With NetCode...
     * 
     * 클라이언트는 상태 및 입력 히스토리를 만들 것.
     * 서버는 클라이언트의 요청 히스토리를 만들 것.
     * 
     * 클라이언트는 요청 후 예측 수행을 진행하고
     * 서버 틱이 진행됨에 따라 클라이언트 요청을 처리하고 결과를 클라이언트에게 반환하면 
     * 클라이언트는 예측 수행 결과와 비교하여 보정을 한다.
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