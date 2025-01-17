using System;
using CharacterAttributes;
using GameInput;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace SinglePlayer
{
    public class SinglePlayerGM : MonoBehaviour
    {
        private static SinglePlayerGM _instance = null;
        public static  SinglePlayerGM Instance => _instance == null ? null : _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
            
            Initialize();
        }

        public GameObject cinemachineCameraSinglePlayer;
        public GameObject mainCamera;
        public GameObject localPlayer;
        public GameObject pfEnemyNpc;

        public GameObject pfSinglePlayerSpawner = null;
        public Transform  playerSpawnTransform  = null;
        public Transform  enemySpawnTransform   = null;
        public GameObject enemyNpc;

        public GameObject pfCurrentBall;
        
        public bool isPlayerTurn = false;

        private bool _isGameReadyToStart;

        public bool IsGameReadyToStart
        {
            get => _isGameReadyToStart;
            set
            {
                if (_isGameReadyToStart != value) // 값이 변경될 때만 수행
                {
                    _isGameReadyToStart                                      = value;
                    isPlayerTurn                                             = true;
                    InputManager.Instance.canThrowBall                       = true;
                    localPlayer.GetComponent<CharacterManager>().hitApproved = true;
                    attackImage.gameObject.SetActive(true);
                    defenseImage.gameObject.SetActive(false);
                }
            }
        }


        [Header("UI")] 
        public Image playerFillImage;
        public Image attackImage;
        public Image defenseImage;

        private SinglePlayerSpawner _singlePlayerSpawner;
        private GameObject          _singlePlayerSpawnerGameObject = null;

        private void Start()
        {
            StartGame();
        }

        public void Initialize()
        {
            InitializeDefaultProperties();
            InitializeSinglePlayerSpawner();
        }

        private void InitializeDefaultProperties()
        {
            IsGameReadyToStart                 = false;
            isPlayerTurn                       = false;
            InputManager.Instance.canThrowBall = false;
        }
        private void InitializeSinglePlayerSpawner()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("NetworkManager is missing");
            }
            
            _singlePlayerSpawnerGameObject = Instantiate(pfSinglePlayerSpawner, Vector3.zero,Quaternion.identity );
            NetworkObject networkObject = _singlePlayerSpawnerGameObject.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("Single Player Spawner 's Network object is null");
            }

            if (NetworkManager.Singleton.IsHost)
            {
                networkObject.Spawn();

                _singlePlayerSpawner = _singlePlayerSpawnerGameObject.GetComponent<SinglePlayerSpawner>();
                localPlayer          = _singlePlayerSpawner.SpawnLocalPlayer(playerSpawnTransform);
            }
            else
            {
                Debug.LogError("Cannot spawn net objects on a non-host client");
            }
        }
        
        public void StartGame()
        {
            DisplayCutScene(0);
            SpawnEnemy();
            IsGameReadyToStart = true;
        }

        public void EndGame(bool didPlayerWin)
        {
            if (didPlayerWin)
            {
                Debug.Log("Player Won");
            }
            else
            {
                Debug.Log("Player Lost");
            }
        }

        public void SwitchTurn()
        {
            Debug.Log($"Requested turn swap to -> {!isPlayerTurn}");
            
            isPlayerTurn = !isPlayerTurn;

            if (isPlayerTurn)
            {
                attackImage.gameObject.SetActive(true);
                defenseImage.gameObject.SetActive(false);
                localPlayer.GetComponent<CharacterManager>().hitApproved = true;
                InputManager.Instance.canThrowBall                       = true;
            }
            else
            {
                attackImage.gameObject.SetActive(false);
                defenseImage.gameObject.SetActive(true);
                localPlayer.GetComponent<CharacterManager>().hitApproved = false;
                InputManager.Instance.canThrowBall                       = false;
            }
            
            PreSwitchTurnActions();
        }

        public void SpawnEnemy()
        {
            enemyNpc = Instantiate(pfEnemyNpc, enemySpawnTransform.position, enemySpawnTransform.rotation);
        }

        private void DisplayCutScene(int index)
        {
            if (index == 0)
            {
                Debug.Log("적 등장");
            }
            else if(index == 1)
            {
                Debug.Log("적 퇴장");
            }
            
        }

        private void PreSwitchTurnActions()
        {
            // ui 변경, input 통제 ...
        }

        public void SetPlayerFill(float fillRatio)
        {
            playerFillImage.fillAmount = fillRatio;
        }
    }
}
