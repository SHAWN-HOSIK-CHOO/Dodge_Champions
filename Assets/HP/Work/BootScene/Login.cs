using Epic.OnlineServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace BootScene
{
    public class Login : MonoBehaviour
    {
        [SerializeField]
        List<EpicLogin> _epicLogin;
        [SerializeField]
        Resolution _resolution;

        Dictionary<EpicLogin, bool> _onLogin;
        Dictionary<EpicLogin, bool> _onConnect;
        void Start()
        {
            _onLogin = new Dictionary<EpicLogin, bool>();
            _onConnect = new Dictionary<EpicLogin, bool>();

            foreach (var login in _epicLogin)
            {
                _onLogin[login] = false;
                login.onLogin += (Result result) =>
                {
                    bool success = result == Result.Success;
                    _onLogin[login] = success;
                    if (!success) return;
                    if (!_onConnect.TryGetValue(login, out success))
                    {
                        success = false;
                        _onConnect[login] = success;
                    }
                    if (success)
                    {
                        OnAuthComplete();
                    }
                };
                _onConnect[login] = false;
                login.onConnect += (Result result) =>
                {
                    bool success = result == Result.Success;
                    _onConnect[login] = success;
                    if (!success) return;
                    if (!_onLogin.TryGetValue(login, out success))
                    {
                        success = false;
                        _onLogin[login] = success;
                    }
                    if (success)
                    {
                        OnAuthComplete();
                    }
                };
            }
        }

        void OnAuthComplete()
        {
            if(!_resolution.ApplySavedResolution())
            {
                _resolution.ChangeResolution(1280,720, true);
            }
            SceneManagerWrapper.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        }
    }
}
