using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static LobbyManager;
using Epic.OnlineServices;

public class LobbyControl : MonoBehaviour
{
    [SerializeField]
    LobbyManager _lobbyManager;
    [SerializeField]
    Button _createRoomButton;
    [SerializeField]
    Button _findRoomButton;
    [SerializeField]
    CreateControl _createControl;
    [SerializeField]
    FindJoinControl _findJoinControl;
    [SerializeField]
    PrivateCodeControl _privateCodeControl;
    [SerializeField]
    SystemAnounce _systemAnouncePref;

    public event Action<EOS_Lobby> OnJoinLobby;

    private void OnDestroy()
    {
        _createRoomButton.onClick.RemoveAllListeners();
        _findRoomButton.onClick.RemoveAllListeners();
        _findJoinControl._onJoinRequest -= RequestJoin;
    }
    void Start()
    {
        _createRoomButton.onClick.AddListener(onClickCreateButton);
        _findRoomButton.onClick.AddListener(onClickfindRoomButton);

        _createControl._submitButton.onClick.AddListener(onClickSubmitButton);
        _createControl._cancelButton.onClick.AddListener(onClickCancelButton);

        _findJoinControl._onJoinRequest += RequestJoin;
        _privateCodeControl._onJoinRequest += JoinWithPrivateCode;
    }

    private void OnEnable()
    {
        _createControl.gameObject.SetActive(false);
        _findJoinControl.gameObject.SetActive(false);
        _privateCodeControl.gameObject.SetActive(false);
    }
    


    public void JoinWithPrivateCode(string code, LobbyInfoElement element)
    {
        if (element._lobbySearch._attribute.GetLobbyCode(out var privateCode) && privateCode == code)
        {
            _privateCodeControl.gameObject.SetActive(false);
            Join(element);
        }
        else
        {
            Instantiate(_systemAnouncePref).GetComponent<SystemAnounce>().Show("코드가 틀렸습니다.");
        }
    }
    void RequestJoin(LobbyInfoElement element)
    { 
        if(element._lobbySearch._attribute.GetLobbySecurity(out var security)&& security == LobbySecurityType.Protected)
        {
            _privateCodeControl.ActivateInpuField(element);
        }
        else
        {
            Join(element);
        }
    }
    void Join(LobbyInfoElement element)
    {
        element._lobbySearch.JoinLobby((Result result, EOS_Lobby lobby) =>
        {
            if (result != Result.Success)
            {
                Instantiate(_systemAnouncePref).GetComponent<SystemAnounce>().Show(result.ToString());
            }
            else
            {
                OnJoinLobby?.Invoke(lobby);
            }
        });
    }
    void onClickSubmitButton()
    {
        var security = _createControl._isPrivate.isOn ? LobbySecurityType.Protected : LobbySecurityType.Public ;
        _lobbyManager.CreateLobby(_createControl.curSelectedMode._text.text, _createControl._roomNameInputField.text, 16, security,
            _createControl._roomCodeInputField.text, (Result result, EOS_Lobby lobby) =>
            {
                if(result != Result.Success)
                {
                    Instantiate(_systemAnouncePref).GetComponent<SystemAnounce>().Show(result.ToString());
                }
                else
                {
                    OnJoinLobby?.Invoke(lobby);
                }
            });
    }
    void onClickCancelButton()
    {
        _createControl.gameObject.SetActive(false);
    }
    void onClickCreateButton()
    {
        _createControl.gameObject.SetActive(true);
        _findJoinControl.gameObject.SetActive(false);
    }
    void onClickfindRoomButton()
    {
        _createControl.gameObject.SetActive(false);
        _findJoinControl.gameObject.SetActive(true);
        _findJoinControl.RemoveAllElement();
        _lobbyManager.FindLobby(10, (Result result, List<EOS_LobbySearchResult> list) =>
        {
            if(result != Result.Success)
            {
                Instantiate(_systemAnouncePref).GetComponent<SystemAnounce>().Show(result.ToString());
            }
            else
            {
                foreach(var item in list)
                {
                    _findJoinControl.AddElement(item);
                }
            }
        });

    }
}
