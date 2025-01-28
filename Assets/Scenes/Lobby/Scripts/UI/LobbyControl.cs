using Epic.OnlineServices;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EOS_SingleLobbyManager;

public class LobbyControl : MonoBehaviour
{
    [SerializeField]
    CreateLobbyUI _createLobbyUI;
    [SerializeField]
    LobbyListControl _lobbyListControl;
    [SerializeField]
    FindLobbyByCodeUI _findLobbyByCodeUI;
 
    FreeNet _freeNet;
    WaitingTransitionUI _waitingTransitionUI;

    public event Action<EOS_Lobby> _onJoined;
    public event Action<EOS_Lobby> _onLeaved;

    private void Awake()
    {
        _createLobbyUI.onClickCreateButton += CreateLobby;
        _findLobbyByCodeUI._onfindButtonClicked += FindLobbyByCode;
        _lobbyListControl._onfindButtonClicked += FindPublicLobby;
        _lobbyListControl._onJoinButtonClicked += JoinFoundLobby;
        _lobbyListControl._onleaveButtonClicked += LeaveLobby;
    }
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _freeNet = FreeNet._instance;
        yield return SingletonMonoBehaviour<WaitingTransitionUI>.WaitInitialize();
        _waitingTransitionUI = WaitingTransitionUI._instance;

        _freeNet._singleLobbyManager._onJoinLobby += OnJoinLobby;
        _freeNet._singleLobbyManager._onLeaveLobby += OnLeaveLobby;
    }

    private void OnEnable()
    {
        _createLobbyUI.gameObject.SetActive(false);
        _lobbyListControl.gameObject.SetActive(true);
        _findLobbyByCodeUI.gameObject.SetActive(true);
    }
    void CreateLobby()
    {
        _waitingTransitionUI._transitionUI.AddNullTransition("CreateLobby");
        _waitingTransitionUI.UpdateWaitInfoDetail("Creating Lobby...");
        _freeNet._singleLobbyManager.CreateLobby(_createLobbyUI.GetLobbymemberNum(),
                _createLobbyUI.GetLobbyType(), _createLobbyUI.GetLobbyInfo());
    }
    void FindPublicLobby()
    {
        _waitingTransitionUI.UpdateWaitInfoDetail("Finding Lobby...");
        _waitingTransitionUI._transitionUI.AddNullTransition("FindingLobby");
        _freeNet._singleLobbyManager.FindPublicLobby(10,onComplete: (Result result, List<FoundLobby> list) =>
        {
            if (result == Result.Success)
            {
                _lobbyListControl.SetLobbyList(list);
            }
            _waitingTransitionUI.UpdateWaitInfoDetail($"{result}");
            _waitingTransitionUI._transitionUI.MakeTransitionEnd("FindingLobby");
        });
    }
    void FindLobbyByCode()
    {
        string code = _findLobbyByCodeUI.GetCode();
        _waitingTransitionUI.UpdateWaitInfoDetail("Finding Lobby...");
        _waitingTransitionUI._transitionUI.AddNullTransition("FindingLobbyByCode");
        _freeNet._singleLobbyManager.FindLobbyByCode(10,code,(Result result ,List<FoundLobby> list)=>
        {
            if (result == Result.Success)
            {
                _lobbyListControl.SetLobbyList(list);
            }
            _waitingTransitionUI.UpdateWaitInfoDetail($"{result}");
            _waitingTransitionUI._transitionUI.MakeTransitionEnd("FindingLobbyByCode");
        });
    }
    void JoinFoundLobby(LobbyInfoUI lobby)
    {
        lobby._foundLobby.JoinLobby();
    }
    void OnJoinLobby(Result result,EOS_Lobby lobby)
    {
        _lobbyListControl.ReleasecurrentFoundLobbies();
        if (result == Result.Success)
        {
            _waitingTransitionUI.UpdateWaitInfoDetail($"Success...");
            _onJoined?.Invoke(lobby);
        }
        else
        {
            _waitingTransitionUI.UpdateWaitInfoDetail($"Fail... {result}");
        }
        _waitingTransitionUI._transitionUI.MakeTransitionEnd("CreateLobby");
    }
    void OnLeaveLobby(Result result,EOS_Lobby lobby)
    {
        _lobbyListControl.ReleasecurrentFoundLobbies();
        if (result == Result.Success)
        {
            _waitingTransitionUI.UpdateWaitInfoDetail($"Success...");
            _onLeaved?.Invoke(lobby);
        }
        else
        {
            _waitingTransitionUI.UpdateWaitInfoDetail($"Fail... {result}");
        }
        _waitingTransitionUI._transitionUI.MakeTransitionEnd("LeaveLobby");
    }
    void LeaveLobby()
    {
        _waitingTransitionUI.UpdateWaitInfoDetail($"Leave Lobby...");
        _waitingTransitionUI._transitionUI.AddNullTransition("LeaveLobby");
        _freeNet._singleLobbyManager.LeaveLobby();
    }
    
    private void OnDestroy()
    {
        _freeNet._singleLobbyManager._onJoinLobby -= OnJoinLobby;

        _createLobbyUI.onClickCreateButton -= CreateLobby;
        _findLobbyByCodeUI._onfindButtonClicked -= FindLobbyByCode;
        _lobbyListControl._onJoinButtonClicked -= JoinFoundLobby;
        _lobbyListControl._onleaveButtonClicked -= LeaveLobby;
    }
}
