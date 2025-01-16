using System;
using GameInput;
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
        }

        public GameObject cinemachineCameraSinglePlayer;
        public GameObject mainCamera;
        public GameObject localPlayer;
        public GameObject pfEnemyNpc;

        public  Transform  enemySpawnTransform = null;
        public GameObject enemyNpc;

        public GameObject pfCurrentBall;
        
        public bool isPlayerTurn = false;

        public bool isGameReadyToStart = false;

        [Header("UI")] 
        public Image playerFillImage;
        public Image attackImage;
        public Image defenseImage;

        public void Initialize(GameObject player)
        {
            localPlayer = player;
            StartGame();
        }

        public void StartGame()
        {
            DisplayCutScene(0);
            SpawnEnemy();
            isGameReadyToStart                 = true;
            isPlayerTurn                       = true;
            InputManager.Instance.canThrowBall = true;
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
            isPlayerTurn = !isPlayerTurn;

            if (isPlayerTurn)
            {
                attackImage.gameObject.SetActive(true);
                defenseImage.gameObject.SetActive(false);
                InputManager.Instance.canThrowBall = true;
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
