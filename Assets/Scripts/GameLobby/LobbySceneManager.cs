using System;
using System.Collections;
using Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEditor;
using Random = UnityEngine.Random;


namespace GameLobby
{
    public class LobbySceneManager : MonoBehaviour
    {
        public GameObject pfNetworkManager;
        
        public Button         startNetworkManager;

        public Color[] clearColors = new Color[10];
        
        [Header("Phase 0")]
        public Button         hostButton;
        public Button         joinButton;
        public TMP_Text       joinCodeText;
        public TMP_InputField joinCodeInputField;
        
        [Header("Phase 1")]
        public TMP_Dropdown ballDropdown;  // TextMeshPro Dropdown for ball selection
        public TMP_Dropdown skillDropdown; // TextMeshPro Dropdown for skill selection
        public Button       confirmButton; // Button to confirm selection

        [Header("Phase 2")] 
        public Button startGameButton;

        [Header("Dancer")] 
        public Animator dancerAnimator;
        private static readonly int    Start1 = Animator.StringToHash("Start");
        
        [Header("Debug")]
        public Button startHostDb;
        public Button startClientDb;
        public Button exitApplicationDb;
        public Button singlePlayButtonDb;


        async void Start()
        {
            int randomColorIndex = Random.Range(0, clearColors.Length);
            
            if(Camera.main != null)
                Camera.main.backgroundColor = clearColors[randomColorIndex];
            
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            
            startNetworkManager.onClick.AddListener(InstantiateNetworkManager);
            hostButton.onClick.AddListener(Callback_btn_CreateRelay);
            joinButton.onClick.AddListener(() => Callback_btn_JoinRelay(joinCodeInputField.text));
            confirmButton.onClick.AddListener(Callback_btn_ConfirmSelection);
            startGameButton.onClick.AddListener(Callback_Btn_StartGame);
            
            InitializeDropdownOptions();

            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientConnectedCallback += Callback_onClientsConnected;

            dancerAnimator.SetBool(Start1,true);
            
            SetPhase0Objects(true);
            SetPhase1Objects(false);
            SetPhase2Objects(false);
        }
        
        private void Callback_onClientsConnected(ulong clientID)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                SetPhase0Objects(false);
                SetPhase1Objects(true);
                SetPhase2Objects(false);
                
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
            hostButton.onClick.RemoveAllListeners();
            joinButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
            startGameButton.onClick.RemoveAllListeners();
        }

        private void InitializeDropdownOptions()
        {
            // Example options for BallDropdown
            ballDropdown.ClearOptions();
            ballDropdown.AddOptions(new System.Collections.Generic.List<string>
                                    {
                                        "BasicBall","AutoBall","DodgeBall","RevengeBall","InfiniteBall", "HardBall"
                                    });

            // Example options for SkillDropdown
            skillDropdown.ClearOptions();
            skillDropdown.AddOptions(new System.Collections.Generic.List<string>
                                     {
                                         "Dash"
                                     });
        }

        private void Callback_btn_ConfirmSelection()
        {
            int selectedBallIndex  = ballDropdown.value;
            int selectedSkillIndex = skillDropdown.value;
            
            PlayerSelectionManager.Instance.SetPlayerSelection(selectedBallIndex, selectedSkillIndex);

            //Debug.Log($"Player selected: BallIndex={selectedBallIndex}, SkillIndex={selectedSkillIndex}");
            SetPhase0Objects(false);
            SetPhase1Objects(false);
            SetPhase2Objects(true);

            //여기서 멀티모드 선언
            GameMode.Instance.CurrentGameMode = EGameMode.MULTIPLAER;
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
        }

        private void InstantiateNetworkManager()
        {
            StartCoroutine(CoRestartNetworkManager());
        }

        
        public void Callback_Btn_StartGame()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Stage", LoadSceneMode.Single);
            }
        }

        async void Callback_btn_CreateRelay()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string     joinCode   = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.text = "Code: " + joinCode;
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation,"dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }

        async void Callback_btn_JoinRelay(string code)
        {
            var joinAllocation  = await RelayService.Instance.JoinAllocationAsync(code);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation,"dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
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
        
        private void SetPhase0Objects(bool flag)
        {
            hostButton.gameObject.SetActive(flag);
            joinButton.gameObject.SetActive(flag);
            joinCodeText.gameObject.SetActive(flag);
            joinCodeInputField.gameObject.SetActive(flag);
        }

        private void SetPhase1Objects(bool flag)
        {
            ballDropdown.gameObject.SetActive(flag);
            skillDropdown.gameObject.SetActive(flag);
            confirmButton.gameObject.SetActive(flag);
        }

        private void SetPhase2Objects(bool flag)
        {
            if (flag == false)
            {
                startGameButton.gameObject.SetActive(false);
            }
            else
            {
                if (NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
                {
                    startGameButton.gameObject.SetActive(true);
                }
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
                NetworkManager.Singleton.StartHost();
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
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                Debug.LogError("NetworkManager is null");
            }
        }

        public void Debug_ToSinglePlayer()
        {
            GameMode.Instance.CurrentGameMode = EGameMode.SINGLEPLAYER;
            SceneManager.LoadScene("SinglePlayStage");
        }
    }
}
