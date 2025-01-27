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
    WaitingTransitionUI _waitingTransitionUI;
    [SerializeField]
    CreateLobbyUI _createLobbyUI;
    [SerializeField]
    LobbyListControl _lobbyListControl;
    [SerializeField]
    FindLobbyByCodeUI _findLobbyByCodeUI;
    FreeNet _freeNet;

    public event Action<EOS_Lobby> _onJoined;

    private void Awake()
    {
        _createLobbyUI.onClickCreateButton += CreateLobby;
        _findLobbyByCodeUI._onfindButtonClicked += FindLobbyByCode;
        _lobbyListControl._onfindButtonClicked += FindPublicLobby;
        _lobbyListControl._onJoinClicked += JoinFoundLobby;
    }
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _freeNet = FreeNet._instance;
        _freeNet._singleLobbyManager._onJoinLobby += OnJoinLobby;
    }
    
    void CreateLobby()
    {
        _waitingTransitionUI.UpdateWaitInfo("Creating Lobby...");
        _waitingTransitionUI.StartWaitingUICoroutine();
        _freeNet._singleLobbyManager.CreateLobby(_createLobbyUI.GetLobbymemberNum(),
                _createLobbyUI.GetLobbyType(), _createLobbyUI.GetLobbyInfo());
    }
    
    void FindPublicLobby()
    {
        _waitingTransitionUI.UpdateWaitInfo("Finding Lobby...");
        _waitingTransitionUI.StartWaitingUICoroutine();
        _freeNet._singleLobbyManager.FindPublicLobby(10,onComplete: (Result result, List<FoundLobby> list) =>
        {
            if (result == Result.Success)
            {
                _lobbyListControl.SetLobbyList(list);
            }
            _waitingTransitionUI.UpdateWaitInfo($"{result}");
            _waitingTransitionUI.StopWaitingUICoroutine();
        });
    }
    void FindLobbyByCode()
    {
        string code = _findLobbyByCodeUI.GetCode();
        _waitingTransitionUI.UpdateWaitInfo("Finding Lobby...");
        _waitingTransitionUI.StartWaitingUICoroutine();


        _freeNet._singleLobbyManager.FindLobbyByCode(10,code,(Result result ,List<FoundLobby> list)=>
        {
            if (result == Result.Success)
            {
                _lobbyListControl.SetLobbyList(list);
            }
            _waitingTransitionUI.UpdateWaitInfo($"{result}");
            _waitingTransitionUI.StopWaitingUICoroutine();
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
            _waitingTransitionUI.UpdateWaitInfo($"Success...");
            _onJoined?.Invoke(lobby);
        }
        else
        {
            _waitingTransitionUI.UpdateWaitInfo($"Fail... {result}");
        }
        _waitingTransitionUI.StopWaitingUICoroutine();
    }
    private void OnDestroy()
    {
        _freeNet._singleLobbyManager._onJoinLobby -= OnJoinLobby;

        _createLobbyUI.onClickCreateButton -= CreateLobby;
        _findLobbyByCodeUI._onfindButtonClicked -= FindLobbyByCode;
        _lobbyListControl._onJoinClicked -= JoinFoundLobby;
    }
}
