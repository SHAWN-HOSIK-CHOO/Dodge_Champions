using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    LobbyControl _lobbyControl;

    FreeNet _freeNet;
    TransitionUI _transitionUI;
    BasicUI _basicUI;
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _freeNet = FreeNet._instance;
        yield return SingletonMonoBehaviour<TransitionUI>.WaitInitialize();
        _transitionUI = TransitionUI._instance;
        _basicUI = _transitionUI.GetRootUI().GetComponentInChildren<BasicUI>();

        _lobbyControl._onJoined += OnJoined;
        _lobbyControl._onLeaved += OnLeaved;

        _basicUI._waitInfoDetail.text = "Load LobbyControl Success";
        _transitionUI.MakeTransitionEnd("LoadLobby");
    }
    void OnLeaved(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _freeNet._NGOManager.Shutdown();
        _lobbyControl.gameObject.SetActive(true);
        Debug.Log("·Îºñ ¿¬°á ²÷±è");
    }
    void OnJoined(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _lobbyControl.gameObject.SetActive(false);
        Debug.Log("¿¬°á ½ÃÀÛ");
        if (lobby.GetLobbyCode(out var attr))
        {
            string code = attr.Data.Value.Value.AsUtf8;
            _freeNet._NGOManager.OnClientStopped -= NGODisConnected;
            _freeNet._NGOManager.OnClientStarted -= NGOConnected;
            _freeNet._NGOManager.OnClientStopped += NGODisConnected;
            _freeNet._NGOManager.OnClientStarted += NGOConnected;
            _basicUI._waitInfoDetail.text = "NGO Client Connect...";
            var transition = new BasicTransition("NGOClientConnect", _basicUI._waitInfo);
            _transitionUI.AddTransition(transition); 
            if (lobby._lobbyOwner.ToString() == lobby._localPUID.ToString())
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartHost(lobby._localPUID.ToString(), code);
            }
            else
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartClient(lobby._localPUID.ToString(), lobby._lobbyOwner.ToString(), code);
            }
        }
    }
    void NGODisConnected(bool b)
    {

    }
    void NGOConnected()
    {
        _basicUI._waitInfoDetail.text =  "NGO Connect Success";
        _transitionUI.MakeTransitionEnd("NGOClientConnect");

        var transition = new BasicTransition("LoadGame", _basicUI._waitInfo);
        _transitionUI.AddTransition(transition);
        _basicUI._waitInfoDetail.text = $"Load Game...";
        SceneManager.LoadScene("Game");
    }
    private void OnDestroy()
    {
        _lobbyControl._onJoined -= OnJoined;
        _freeNet._NGOManager.OnClientStopped -= NGODisConnected;
        _freeNet._NGOManager.OnClientStarted -= NGOConnected;
    }
}
