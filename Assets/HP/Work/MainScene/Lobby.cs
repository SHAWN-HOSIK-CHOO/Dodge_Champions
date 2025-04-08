using Epic.OnlineServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static LobbyManager;

namespace MainScene
{
    public class Lobby : MonoBehaviour
    {
        [SerializeField]
        LobbyManager _LobbyManager;

        [SerializeField]
        UIPageView _lobbyPageView;


        [SerializeField]
        UIPageView _modePageView;

        [SerializeField]
        UISelectHandler _UISelectHandler;

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
        UIImgButton _CreateRoom;

        [SerializeField]
        GameObject _CreateRoomUI;

        [SerializeField]
        UIImgButton _FindRoom;

        [SerializeField]
        GameObject _FindRoomUI;

        [SerializeField]
        UIImgButton _CreateRoomConfirm;

        [SerializeField]
        UIImgButton _CreateRoomCancle;


        private void Start()
        {
            _CreateRoomCancle.OnPointerClickAction += OnCreateRoomCancleClick;
            _CreateRoomConfirm.OnPointerClickAction += OnCreateRoomConfirmClick;
            _LobbyLogo.OnPointerClickAction += OnLobbyLogoClick;
            _closeLobby.OnPointerClickAction += OnCloseLobby;
            _CreateRoom.OnPointerClickAction += OnCreateRoomClick;
            _FindRoom.OnPointerClickAction += OnFindRoomClick;
        }

        void OnCreateRoomConfirmClick(BaseEventData data)
        {

            var mode = _modePageView._currentContent.GetComponent<ModeElement>();
            var security = _privateToggle.IsOn ? LobbySecurityType.Protected : LobbySecurityType.Public;
            string roomName = _roomNameInput.text;
            string roomCode = _roomCodeInput.text;


            //_LobbyManager.CreateLobby(mode._text, roomName, 16, security,
            //    roomCode, (Result result, EOS_Lobby lobby) =>
            //    {
            //        //if (result != Result.Success)
            //        //{
            //        //    Instantiate(_systemAnouncePref).GetComponent<SystemAnounce>().Show(result.ToString());
            //        //}
            //        //else
            //        //{
            //        //    OnJoinLobby?.Invoke(lobby);
            //        //}
            //    });
        }
        void OnCreateRoomCancleClick(BaseEventData data)
        {

        }



        void OnFindRoomClick(BaseEventData data)
        {
            _FindRoomUI.gameObject.SetActive(true);
            _CreateRoomUI.gameObject.SetActive(false);
        }

        void OnCreateRoomClick(BaseEventData data)
        {
            _FindRoomUI.gameObject.SetActive(false);
            _CreateRoomUI.gameObject.SetActive(true);
        }


        void OnLobbyLogoClick(BaseEventData data)
        {
            _LobbyLogo.gameObject.SetActive(true);
        }

        void OnCloseLobby(BaseEventData data)
        {
            _LobbyUI.SetActive(false);
        }
    }
}