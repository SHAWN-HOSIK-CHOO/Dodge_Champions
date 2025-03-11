using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EOSWrapper;

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


    void Start()
    {
        _login.onLogin += OnLogin;
        _login.onConnect += OnConnect;
        _lobbyControl.OnJoinLobby += OnJoinLobby;
        _lobbyControl.gameObject.SetActive(true);
        _consoleController.gameObject.SetActive(true);
        _lobbyControl.gameObject.SetActive(false);
        _transition.gameObject.SetActive(false);
        _consoleController.ShowInputField(true);
        _coroutineHandler.BeginCoroutine(() => { return GoLobbyScene(); });

    }

    void OnConnect()
    {
        _login.gameObject.SetActive(false);
        _lobbyControl.gameObject.SetActive(true);
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Connect Success");
        _consoleController.AddText("Enjoy Game");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
    }
    void OnLogin()
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Login Success");
        _consoleController.AddText($"Hello <color=yellow>{FreeNet._instance._localUser._localUserInfo.Value.DisplayName}</color>");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
    }
    void OnJoinLobby(EOS_Lobby lobby)
    {
        var simulator = _consoleController.GetComponent<ConsoleSimulator>();
        simulator.BeginTracking();
        _consoleController.AddText("Join Lobby");
        simulator.EndTracking();
        simulator.Simulate(0.05f);
        _coroutineHandler.BeginCoroutine(() => { return GoLobbyScene(); });
    }

    IEnumerator GoLobbyScene()
    {
        var asyncOperation = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;
        _transition.gameObject.SetActive(true);
        _transition.color = new Color(0, 0, 0, 0);
        yield return _transition.DOFade(1f, 1f).SetEase(Ease.InOutFlash).WaitForCompletion();
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);
        yield return _transition.DOFade(0f, 1f).SetEase(Ease.InOutFlash).WaitForCompletion();
        _transition.gameObject.SetActive(false);
    }



    private void OnDestroy()
    {
        _login.onLogin -= OnLogin;
        _login.onConnect -= OnConnect;
        _lobbyControl.OnJoinLobby -= OnJoinLobby;
    }

}
