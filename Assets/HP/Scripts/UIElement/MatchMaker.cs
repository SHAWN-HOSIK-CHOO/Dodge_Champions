using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchMaker : MonoBehaviour
{
    [SerializeField]
    LobbyControl _lobbyControl;
    [SerializeField]
    Login _login;
    [SerializeField]
    ConsoleController _consoleController;
    [SerializeField]
    CoroutineHandler _coroutineHandler;
    [SerializeField]
    Image _transition;


    bool _loginSuccess;
    bool _connectSuccess;
    void Start()
    {

        _login.onLogin += OnLogin;
        _login.onConnect += OnConnect;
        _lobbyControl.OnJoinLobby += OnJoinLobby;

        _coroutineHandler.BeginCoroutine(() => { return WaitAuthComplete(); });
    }
    private void OnEnable()
    {
        _lobbyControl.gameObject.SetActive(false);
        _lobbyControl.gameObject.SetActive(false);
        _transition.gameObject.SetActive(false);
        _consoleController.ShowInputField(true);
        _consoleController.gameObject.SetActive(true);
    }

    IEnumerator WaitAuthComplete()
    {
        while (!(_loginSuccess && _connectSuccess))
        {
            yield return null;
        }
        _login.gameObject.SetActive(false);
        _lobbyControl.gameObject.SetActive(true);

        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Login & Connect Success");
        _consoleController.AddText("Enjoy Game");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
    }

    void OnConnect()
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Connect Success");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        _connectSuccess = true;
    }
    void OnLogin()
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Login Success");
        _consoleController.AddText($"Hello <color=yellow>{FreeNet._instance._localUser._localUserInfo.Value.DisplayName}</color>");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        _loginSuccess = true;
    }
    void OnJoinLobby(EOS_Lobby lobby)
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Load Lobby Start...");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        _coroutineHandler.BeginCoroutine(() => { return BeginLoadLobby(lobby); });
    }
    IEnumerator BeginLoadLobby(EOS_Lobby lobby)
    {
        _transition.gameObject.SetActive(true);
        _transition.color = new Color(0, 0, 0, 0);
        yield return _transition.DOFade(1f, 1f).SetEase(Ease.InOutFlash).WaitForCompletion();

        if (LobbyAttributeExtenstion.GetLobbySocket(lobby._attribute, out var socket))
        {
            FreeNet._instance._ngoManager._onNgoManagerReady += OnNgoManagerReady;
            if (lobby._lobbyOwner == FreeNet._instance._localUser._localPUID)
            {
                FreeNet._instance._ngoManager.StartHost(FreeNet._instance._localUser._localPUID, socket);
            }
            else
            {
                FreeNet._instance._ngoManager.StartClient(FreeNet._instance._localUser._localPUID, lobby._lobbyOwner, socket);
                FreeNet._instance._ngoManager.SceneManager.OnLoad += OnLoad;
            }
        }
    }

    void OnNgoManagerReady()
    {
        FreeNet._instance._ngoManager._onNgoManagerReady -= OnNgoManagerReady;
        if (FreeNet._instance._ngoManager.IsServer)
        {
            FreeNet._instance._ngoManager.SceneManager.OnLoad += OnLoad;
            FreeNet._instance._ngoManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
    }

    void OnLoad(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        FreeNet._instance._ngoManager.SceneManager.OnLoad -= OnLoad;
        _coroutineHandler.BeginCoroutine(() => { return EndLoadLobby(clientId, sceneName, asyncOperation); });
    }

    IEnumerator EndLoadLobby(ulong clientId, string sceneName, AsyncOperation asyncOperation)
    {
        asyncOperation.allowSceneActivation = false;
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText($"[SceneEvent] Name: {sceneName}, CliendID: {clientId}");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        yield return _transition.DOFade(0f, 1f).SetEase(Ease.InOutFlash).WaitForCompletion();
        _transition.gameObject.SetActive(false);
        asyncOperation.allowSceneActivation = true;
    }
    private void OnDestroy()
    {
        _login.onLogin -= OnLogin;
        _login.onConnect -= OnConnect;
        _lobbyControl.OnJoinLobby -= OnJoinLobby;
    }

}
