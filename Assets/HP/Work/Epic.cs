using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.UserInfo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace MainScene
{
    public class Epic : MonoBehaviour
    {
        [SerializeField]
        GameObject _EpicUI;

        [SerializeField]
        UIImgButton _EpicLogo;

        [SerializeField]
        UIImgButton _closeEpic;

        [SerializeField]
        UIImgButton _logOut;

        [SerializeField]
        UIElement _userName;

        [SerializeField]
        UIElement _userCountry;

        [SerializeField]
        UIElement _NatType;

        [SerializeField]
        UIElement _RelayControl;

        private void Start()
        {
            _EpicLogo.OnPointerClickAction += OnEpicLogoClick;
            _userName.OnActivateAction += OnUserNameActivate;
            _userCountry.OnActivateAction += OnUserCountryActivate;
            _NatType.OnActivateAction += OnNatTypeActivate;
            _RelayControl.OnActivateAction += OnRelayControlActivate;

            _closeEpic.OnPointerClickAction += OnCloseEpic;
            _logOut.OnPointerClickAction += OnLogOut;
        }

        void OnEpicLogoClick(BaseEventData data)
        {
            _EpicUI.gameObject.SetActive(true);
        }

        void OnUserNameActivate()
        {
            var textUI = _userName.GetComponent<TMP_Text>();
            UserInfoData? _localUserInfo = FreeNet._instance._localUser._localUserInfo;
            if (_localUserInfo.HasValue)
            {
                textUI.text = _localUserInfo.Value.DisplayName;
            }
        }
        void OnUserCountryActivate()
        {
            var textUI = _userCountry.GetComponent<TMP_Text>();
            UserInfoData? _localUserInfo = FreeNet._instance._localUser._localUserInfo;
            if (_localUserInfo.HasValue)
            {
                textUI.text = _localUserInfo.Value.Country;
            }
        }
        void OnNatTypeActivate()
        {
            var textUI = _NatType.GetComponent<TMP_Text>();
            EOSWrapper.P2PControl.QueryNATType(FreeNet._instance._eosCore._IP2P, (ref OnQueryNATTypeCompleteInfo info) =>
            {
                if(info.ResultCode == Result.Success)
                {
                    textUI.text = info.NATType.ToString();
                }
            });
        }
        void OnRelayControlActivate()
        {
            var textUI = _RelayControl.GetComponent<TMP_Text>();
            if(EOSWrapper.P2PControl.GetRelayControl(FreeNet._instance._eosCore._IP2P, out RelayControl control))
            {
                textUI.text = control.ToString();
            }
        }

        void OnCloseEpic(BaseEventData data)
        {
            _EpicUI.SetActive(false);
        }

        void OnLogOut(BaseEventData data)
        {
            var options = new DeletePersistentAuthOptions();
            FreeNet._instance._eosCore._IAuth.DeletePersistentAuth(ref options, null, null);
            EOSWrapper.LoginControl.LogOut(FreeNet._instance._eosCore._IAuth, FreeNet._instance._localUser._localEAID._EAID,(ref LogoutCallbackInfo info) =>
            {
                SceneManagerWrapper.LoadScene("BootScene",LoadSceneMode.Single);
            });
        }
    }
}