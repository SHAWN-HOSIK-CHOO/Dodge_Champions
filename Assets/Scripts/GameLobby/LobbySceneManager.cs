using Game;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameLobby
{
    public class LobbySceneManager : MonoBehaviour
    {
        public GameObject pfNetworkManager;

        public Button startNetworkManager;

        [Header("Debug")]
        public Button startHostDb;
        public Button startClientDb;
        public TMP_Text joinText;
        public TMP_InputField joinField;
        public Button createRelay;
        public Button joinRelay;

        [Header("Confirm Button")]
        public Button confirmButton;

        [Header("Start Button")]
        public Button startGameButton;

        async void Start()
        {

#if UNITY_EDITOR
            Application.runInBackground = true;
#endif

            await SceneManagerWrapper.LoadSceneAsync("LobbyUI", LoadSceneMode.Additive);

            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            startNetworkManager.onClick.AddListener(InstantiateNetworkManager);
            confirmButton.onClick.AddListener(Callback_Btn_ConfirmSelection);
            startGameButton.onClick.AddListener(Callback_Btn_StartGame);


            if (NetworkManager.Singleton != null && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                startClientDb.gameObject.SetActive(false);
                startHostDb.gameObject.SetActive(false);
            }

        }


        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= Callback_onClientsConnected;
            }
            confirmButton.onClick.RemoveAllListeners();
            startGameButton.onClick.RemoveAllListeners();
        }

        private void Callback_Btn_ConfirmSelection()
        {
            //PlayerSelectionManager.Instance.SetPlayerSelection(selectedBallIndex, selectedSkillIndex);

            //여기서 멀티모드 선언
            GameMode.Instance.CurrentGameMode = EGameMode.MULTIPLAER;
        }

        public void Callback_Btn_StartGame()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Stage", LoadSceneMode.Single);
            }
        }

        private IEnumerator CoRestartNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                Debug.Log("Shutting down existing NetworkManager...");

                NetworkManager.Singleton.OnClientConnectedCallback -= Callback_onClientsConnected;
                // 네트워크 종료
                NetworkManager.Singleton.Shutdown();
                Destroy(NetworkManager.Singleton.gameObject);

                // 네트워크 정리 대기
                yield return new WaitForSeconds(1f);
            }

            // 새로운 NetworkManager 생성
            Instantiate(pfNetworkManager);
            NetworkManager.Singleton.OnClientConnectedCallback += Callback_onClientsConnected;
            Debug.Log("NetworkManager restarted successfully.");
#if UNITY_EDITOR
            NetworkManager.Singleton.RunInBackground = true;
#endif
        }

        private void InstantiateNetworkManager()
        {
            StartCoroutine(CoRestartNetworkManager());
        }

        private void OnApplicationQuit()
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
        //DEBUG
        public void ExitApplication()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
        }

        public void Debug_StartHost()
        {
            if (NetworkManager.Singleton != null)
            {
                (NetworkManager.Singleton as NgoManager).StartHost();
            }
            else
            {
                Debug.LogError("NetworkManager is null");
            }
        }

        public void Debug_StartClient()
        {
            if (NetworkManager.Singleton != null)
            {
                (NetworkManager.Singleton as NgoManager).StartClient();
            }
            else
            {
                Debug.LogError("NetworkManager is null");
            }
        }

        //TODO: Delete All Legacy
        public async void Callback_btn_CreateRelay()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinText.text = joinCode;
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        public async void Callback_btn_JoinRelay()
        {
            string userInput = joinField.text;

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(userInput);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }

        private void Callback_onClientsConnected(ulong clientID)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                startClientDb.gameObject.SetActive(false);
                startHostDb.gameObject.SetActive(false);
            }
        }
    }
}
