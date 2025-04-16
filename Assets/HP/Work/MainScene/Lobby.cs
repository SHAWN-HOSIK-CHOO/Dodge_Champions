using Epic.OnlineServices;
using HP;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static HP.LobbyManager;

namespace MainScene
{
    public class Lobby : MonoBehaviour
    {
        [SerializeField]
        Canvas _SystemMsgCanvas;

        [SerializeField]
        SystemAnounce _systemAnouncePref;

        [SerializeField]
        LobbyElement _LobbyElementPref;

        [SerializeField]
        ModeElement _ModeElementPref;

        [SerializeField]
        UIPageView _lobbyPageView;

        [SerializeField]
        GameObject _LobbyPading;

        [SerializeField]
        GameObject _privateCode;
        [SerializeField]
        TMP_Text _privateCodeInfo;
        [SerializeField]
        UIImgButton _privateCodeConfirm;
        [SerializeField]
        UIImgButton _privateCodeCancle;
        [SerializeField]
        UITmpInputField _privateCodeinputField;


        [SerializeField]
        UIPageView _modePageView;

        [SerializeField]
        UITmpInputField _roomNameInput;

        [SerializeField]
        UITmpInputField _roomCodeInput;

        [SerializeField]
        UIImgToggle _privateToggle;

        [SerializeField]
        UIImgButton _LobbyLogo;

        [SerializeField]
        UIImgButton _closeLobby;

        [SerializeField]
        GameObject _LobbyUI;

        [SerializeField]
        UIImgButton _CreateRoomButton;

        [SerializeField]
        GameObject _CreateRoomUI;

        [SerializeField]
        UIImgButton _FindRoomButton;

        [SerializeField]
        GameObject _FindRoomUI;

        [SerializeField]
        UIImgButton _findRoomConfirm;

        [SerializeField]
        UIImgButton _findRoomCancle;

        [SerializeField]
        UIImgButton _CreateRoomConfirm;

        [SerializeField]
        UIImgButton _CreateRoomCancle;

        private void Start()
        {
            _privateCodeConfirm.OnPointerClickAction += OnprivateCodeConfirmClick;
            _privateCodeCancle.OnPointerClickAction += OnprivateCodeCancleClick;

            _findRoomCancle.OnPointerClickAction += OnfindRoomCancleClick;
            _findRoomConfirm.OnPointerClickAction += OnFindRoomConfirmClick;
            _CreateRoomCancle.OnPointerClickAction += OnCreateRoomCancleClick;
            _CreateRoomConfirm.OnPointerClickAction += OnCreateRoomConfirmClick;

            _LobbyLogo.OnPointerClickAction += OnLobbyLogoClick;
            _closeLobby.OnPointerClickAction += OnCloseLobby;
            _CreateRoomButton.OnPointerClickAction += OnCreateRoomButtonClick;
            _FindRoomButton.OnPointerClickAction += OnFindRoomButtonClick;
        }

        private void OnprivateCodeCancleClick(BaseEventData data)
        {
            _FindRoomButton.Activate();
            _privateCodeInfo.text = "";
            _privateCode.SetActive(false);
        }

        private void OnprivateCodeConfirmClick(BaseEventData data)
        {
            var lobby = _lobbyPageView._currentContent.GetComponent<LobbyElement>();
            if (lobby._lobbySearch._attribute.GetLobbyCode(out var privateCode) && privateCode == _privateCodeinputField.text)
            {
                lobby._lobbySearch.JoinLobby((Result result, EOS_Lobby lobby) =>
                {
                    _FindRoomButton.Activate();
                    if (result != Result.Success)
                    {
                        Instantiate(_systemAnouncePref, _SystemMsgCanvas.transform).GetComponent<SystemAnounce>().Show(result.ToString());
                    }
                    else
                    {
                        _privateCodeInfo.text = "";
                        _privateCode.SetActive(false);
                        HP.NetworkManager.Instance.JoinLobby(lobby);
                    }
                });
            }
            else
            {
                _privateCodeInfo.text = "코드가 틀렸습니다";
            }
        }

        void OnFindRoomConfirmClick(BaseEventData data)
        {
            if (_lobbyPageView._currentContent == null) return;
            var lobby = _lobbyPageView._currentContent.GetComponent<LobbyElement>();
            _FindRoomButton.DeActivate();

            if (lobby._lobbySearch._attribute.GetLobbySecurity(out var security))
            {
                if (security == LobbySecurityType.Protected)
                {
                    _privateCode.SetActive(true);
                    _privateCodeInfo.text = "";
                }
                else
                {
                    lobby._lobbySearch.JoinLobby((Result result, EOS_Lobby lobby) =>
                    {
                        _FindRoomButton.Activate();
                        if (result != Result.Success)
                        {
                            Instantiate(_systemAnouncePref, _SystemMsgCanvas.transform).GetComponent<SystemAnounce>().Show(result.ToString());
                        }
                        else
                        {
                            HP.NetworkManager.Instance.JoinLobby(lobby);
                        }
                    });
                }
            }
        }

        void OnfindRoomCancleClick(BaseEventData data)
        {
            _LobbyUI.SetActive(false);
        }
        void OnCreateRoomConfirmClick(BaseEventData data)
        {
            if (_modePageView._currentContent == null) return;
            var mode = _modePageView._currentContent.GetComponent<ModeElement>();
            var security = _privateToggle.IsOn ? LobbySecurityType.Protected : LobbySecurityType.Public;
            string roomName = _roomNameInput.text;
            string roomCode = _roomCodeInput.text;
            _CreateRoomButton.DeActivate();
            HP.LobbyManager.Instance.CreateLobby(mode._mode.text, roomName, 16, security,
                roomCode, (Result result, EOS_Lobby lobby) =>
                {
                    _CreateRoomButton.Activate();
                    if (result != Result.Success)
                    {
                        Instantiate(_systemAnouncePref, _SystemMsgCanvas.transform).GetComponent<SystemAnounce>().Show(result.ToString());
                    }
                    else
                    {
                        HP.NetworkManager.Instance.JoinLobby(lobby);
                    }
                });
        }
        void OnCreateRoomCancleClick(BaseEventData data)
        {
            _LobbyUI.SetActive(false);
        }
        void OnFindRoomButtonClick(BaseEventData data)
        {
            _LobbyPading.gameObject.SetActive(false);
            _FindRoomUI.gameObject.SetActive(true);
            _CreateRoomUI.gameObject.SetActive(false);

            _FindRoomButton.DeActivate();
            _lobbyPageView.DestroyAllElement();
            HP.LobbyManager.Instance.FindLobby(10, (Result result, List<EOS_LobbySearchResult> list) =>
            {
                _FindRoomButton.Activate();
                if (result != Result.Success)
                {
                    Instantiate(_systemAnouncePref, _SystemMsgCanvas.transform).GetComponent<SystemAnounce>().Show(result.ToString());
                }
                else
                {
                    foreach (var item in list)
                    {
                        var element = Instantiate(_LobbyElementPref, _lobbyPageView.transform);
                        element.Init(item);
                        _lobbyPageView.AddContent(element.GetComponent<UIImgToggle>());
                    }
                }
            });
        }
        void OnCreateRoomButtonClick(BaseEventData data)
        {
            _LobbyPading.gameObject.SetActive(false);
            _FindRoomUI.gameObject.SetActive(false);
            _CreateRoomUI.gameObject.SetActive(true);

            _modePageView.DestroyAllElement();

            for (int i = 0; i < 3; i++)
            {
                var element = Instantiate(_ModeElementPref, _modePageView.transform);
                element._mode.text = $"Default{i}";
                _modePageView.AddContent(element.GetComponent<UIImgToggle>());
            }
        }
        void OnLobbyLogoClick(BaseEventData data)
        {
            _LobbyUI.gameObject.SetActive(true);
            _LobbyPading.gameObject.SetActive(true);
            _FindRoomUI.gameObject.SetActive(false);
            _CreateRoomUI.gameObject.SetActive(false);
        }
        void OnCloseLobby(BaseEventData data)
        {
            _LobbyUI.SetActive(false);
        }
    }
}