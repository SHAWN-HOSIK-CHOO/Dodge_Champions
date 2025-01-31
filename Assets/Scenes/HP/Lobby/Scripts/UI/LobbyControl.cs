using Epic.OnlineServices;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    TransitionUI _transitionUI;
    BasicUI _basicUI;

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
        yield return SingletonMonoBehaviour<TransitionUI>.WaitInitialize();
        _transitionUI = TransitionUI._instance;
        _basicUI = _transitionUI.GetRootUI().GetComponentInChildren<BasicUI>();


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
        var transition = new BasicTransition("JoinLobby", _basicUI._waitInfo);
        _transitionUI.AddTransition(transition);
        _basicUI._waitInfoDetail.text = "Joining Lobby...";
        _freeNet._singleLobbyManager.CreateLobby(_createLobbyUI.GetLobbymemberNum(),
                _createLobbyUI.GetLobbyType(), _createLobbyUI.GetLobbyInfo());
    }
    void FindPublicLobby()
    {
        var transition = new BasicTransition("FindLobby", _basicUI._waitInfo);
        _transitionUI.AddTransition(transition);
        _basicUI._waitInfoDetail.text = "Finding Lobby...";
        _freeNet._singleLobbyManager.FindPublicLobby(10,onComplete: (Result result, List<FoundLobby> list) =>
        {
            if (result == Result.Success)
            {
                _lobbyListControl.SetLobbyList(list);
            }
            _basicUI._waitInfoDetail.text =  $"{result}";
            _transitionUI.MakeTransitionEnd("FindLobby");
        });
    }
    void FindLobbyByCode()
    {
        string code = _findLobbyByCodeUI.GetCode();
        _basicUI._waitInfoDetail.text = "Finding Lobby...";
        var transition = new BasicTransition("FindLobbyByCode", _basicUI._waitInfo);
        _transitionUI.AddTransition(transition);
        _freeNet._singleLobbyManager.FindLobbyByCode(10,code,(Result result ,List<FoundLobby> list)=>
        {
            if (result == Result.Success)
            {
                _lobbyListControl.SetLobbyList(list);
            }
            _basicUI._waitInfoDetail.text = $"{result}";
            _transitionUI.MakeTransitionEnd("FindLobbyByCode");
        });
    }
    void JoinFoundLobby(LobbyInfoUI lobby)
    {
        var transition = new BasicTransition("JoinLobby", _basicUI._waitInfo);
        _transitionUI.AddTransition(transition);
        _basicUI._waitInfoDetail.text = "Joining Lobby...";
        lobby._foundLobby.JoinLobby();
    }
    void OnJoinLobby(Result result,EOS_Lobby lobby)
    {
        _lobbyListControl.ReleasecurrentFoundLobbies();
        if (result == Result.Success)
        {
            _basicUI._waitInfoDetail.text = $"Success...";
            _onJoined?.Invoke(lobby);
        }
        else
        {
            _basicUI._waitInfoDetail.text = $"Fail... {result}";
        }
        _transitionUI.MakeTransitionEnd("JoinLobby");
    }
    void OnLeaveLobby(Result result,EOS_Lobby lobby)
    {
        _lobbyListControl.ReleasecurrentFoundLobbies();
        if (result == Result.Success)
        {
            _basicUI._waitInfoDetail.text = $"Success...";
            _onLeaved?.Invoke(lobby);
        }
        else
        {
            _basicUI._waitInfoDetail.text = $"Fail... {result}";
        }
        _transitionUI.MakeTransitionEnd("LeaveLobby");
    }
    void LeaveLobby()
    {
        _basicUI._waitInfoDetail.text = $"Leave Lobby...";
        var transition = new BasicTransition("LeaveLobby", _basicUI._waitInfo);
        _transitionUI.AddTransition(transition);
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
