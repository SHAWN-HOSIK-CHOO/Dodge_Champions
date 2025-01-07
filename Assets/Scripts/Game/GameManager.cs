using System.Collections;
using UnityEngine;
using Unity.Netcode;
using GameUI;
using UnityEngine.Playables;

namespace Game
{
    public class GameManager : NetworkBehaviour
    {
        private static GameManager _instance = null;
        public static  GameManager Instance => _instance == null ? null : _instance;

        public GameObject cinemachineCamera;
        public GameObject mainCamera;
        public GameObject cutSceneCamera;

        public bool isGameReadyToStart = false;

        public GameObject localPlayerBall;
        public GameObject enemyPlayerBall;
        public Transform  localPlayerBallSpawnPosition;

        public Transform[] spawnTransforms = new Transform[2];

        public  PlayableDirector cutSceneDirector = null;
        private Coroutine        _cutSceneCoroutine = null;

        public GameObject headsCoin = null;
        public GameObject tailsCoin = null;

        private bool _isHeads = false;

        public GameObject uiManager;

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

        public int localClientID;
        public int enemyClientID;

        public GameObject localPlayer = null;
        public GameObject enemyPlayer = null;

        public void StartRound()
        {
            uiManager.SetActive(false);
            cutSceneCamera.SetActive(true);
            
            int randomNumber = Random.Range(0, 2);
            _isHeads = randomNumber == 0;
            
            if (_cutSceneCoroutine != null)
            {
                StopCoroutine(CoSetGameOrder());
            }

            _cutSceneCoroutine = StartCoroutine(CoSetGameOrder());
        }
        
        private IEnumerator CoSetGameOrder()
        {
            cutSceneDirector.Play();

            while (cutSceneDirector.state == PlayState.Playing)
            {
                yield return null;
            }
            
            uiManager.SetActive(true);
            cutSceneCamera.SetActive(false);
            UIManager.Instance.StartGameCountDown(5f);
        }
        
        public void Callback_Timeline_SetActiveAppropriateCoin()
        {
            if (_isHeads)
            {
                headsCoin.SetActive(true);
                tailsCoin.SetActive(false);
            }
            else
            {
                tailsCoin.SetActive(true);
                headsCoin.SetActive(false);
            }
        }
    }
}
