using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using GameInput;
using System.Collections;
using UnityEngine;
public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    LobbyControl _LobbyControl;

    WaitingTransitionUI _waitingTransitionUI;

    FreeNet _freeNet;
    IEnumerator Start()
    {
        yield return SingletonMonoBehaviour<FreeNet>.WaitInitialize();
        _freeNet = FreeNet._instance;
        yield return SingletonMonoBehaviour<WaitingTransitionUI>.WaitInitialize();
        _waitingTransitionUI = WaitingTransitionUI._instance;

        _LobbyControl._onJoined += OnJoined;
        _LobbyControl._onLeaved += OnLeaved;

        _waitingTransitionUI.UpdateWaitInfoDetail("Load LobbyControl Success");
        _waitingTransitionUI._transitionUI.MakeTransitionEnd("Load Lobby");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            _LobbyControl.gameObject.SetActive(!_LobbyControl.gameObject.activeSelf);
        }
    }
    void OnLeaved(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _freeNet._NGOManager.Shutdown();
        _LobbyControl.gameObject.SetActive(true);
        Debug.Log("·Îºñ ¿¬°á ²÷±è");
    }
    void OnJoined(EOS_SingleLobbyManager.EOS_Lobby lobby)
    {
        _LobbyControl.gameObject.SetActive(false);
        Debug.Log("¿¬°á ½ÃÀÛ");
        if (lobby._attribute.TryGetValue("LobbyCode", out var attr))
        {
            string code = attr.Data.Value.Value.AsUtf8;

            _freeNet._NGOManager.OnClientStopped -= NGODisConnected;
            _freeNet._NGOManager.OnClientStarted -= NGOConnected;
            _freeNet._NGOManager.OnClientStopped += NGODisConnected;
            _freeNet._NGOManager.OnClientStarted += NGOConnected;

            _waitingTransitionUI.UpdateWaitInfoDetail("NGO Client Connect...");
            _waitingTransitionUI._transitionUI.AddNullTransition("NGOClientConnect");
            if (lobby._lobbyOwner.ToString() == lobby._localPUID.ToString())
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartHost(lobby._localPUID.ToString(), code);
            }
            else
            {
                _freeNet.GetComponent<EOSNetcodeTransport>().StartClient(lobby._lobbyID.ToString(), lobby._lobbyOwner.ToString(), code);
            }


        }
    }
    void NGODisConnected(bool b)
    {
        // b represent hostmode



    }
    void NGOConnected()
    {
        _waitingTransitionUI.UpdateWaitInfoDetail("NGO Connect Success");
        _waitingTransitionUI._transitionUI.MakeTransitionEnd("NGOClientConnect");
    }
    private void OnDestroy()
    {
        _LobbyControl._onJoined -= OnJoined;
        _freeNet._NGOManager.OnClientStopped -= NGODisConnected;
        _freeNet._NGOManager.OnClientStarted -= NGOConnected;
    }
}
