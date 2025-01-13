using System;
using System.Collections;
using CharacterAttributes;
using GameInput;
using GameLobby;
using UnityEngine;
using Unity.Netcode;
using GameUI;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

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

        public int  currentAttackPlayerID   = -1;
        public bool isLocalPlayerAttackTurn = false;

        private BallSkillDataBase _ballSkillDataBase;

        public override void OnNetworkSpawn()
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

            _ballSkillDataBase = this.GetComponent<BallSkillDataBase>();
            localPlayerBall =
                _ballSkillDataBase.pfPlayerBalls[PlayerSelectionManager.Instance.GetLocalPlayerSelection().BallIndex];
            
            enemyPlayerBall = _ballSkillDataBase.pfEnemyBalls[PlayerSelectionManager.Instance.GetEnemySelection().BallIndex];
            
            //TODO: 스킬도 설정
        }

        public int localClientID;
        public int enemyClientID;

        public GameObject localPlayer = null;
        public GameObject enemyPlayer = null;

        [ServerRpc(RequireOwnership = false)]
        public void SwapTurnServerRPC()
        {
            SwapTurnClientRPC();
            //Debug.Log("Called from : " + localClientID + " by : " + OwnerClientId);
        }

        [ClientRpc]
        private void SwapTurnClientRPC()
        {
            //Debug.Log("Executed from : " + localClientID);
            currentAttackPlayerID   = currentAttackPlayerID == 0 ? 1 : 0;
            isLocalPlayerAttackTurn = localClientID == currentAttackPlayerID;
            
            if (isLocalPlayerAttackTurn)
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(true);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(false);
                InputManager.Instance.canThrowBall = true;
            }
            else
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(false);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(true);
                InputManager.Instance.canThrowBall = false;
            }
        }
        
        [ServerRpc]
        public void StartRoundServerRPC()
        {
            //Debug.Log($"Called on Host: {IsHost}, Server: {IsServer}, Client: {IsClient}");
            
            uiManager.SetActive(false);
            cutSceneCamera.SetActive(true);
            
            int randomNumber = Random.Range(0, 2);
            
            Debug.Log("Random Number : " + randomNumber);
            
            StartRoundClientRPC(randomNumber);
        }

        [ClientRpc]
        private void StartRoundClientRPC(int randomNumber)
        {
            _isHeads = randomNumber == 0;

            currentAttackPlayerID = _isHeads ? 0 : 1;

            isLocalPlayerAttackTurn = localClientID == currentAttackPlayerID;
            
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
            if (isLocalPlayerAttackTurn)
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(true);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(false);
                InputManager.Instance.canThrowBall = true;
            }
            else
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(false);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(true);
                InputManager.Instance.canThrowBall = false;
            }
            UIManager.Instance.StartGameCountDown(5f);
        }
        
        public void Callback_Timeline_SetActiveAppropriateCoin()
        {
            if (_isHeads)
            {
                Debug.Log("Activated Heads Coin");
                headsCoin.SetActive(true);
                tailsCoin.SetActive(false);
            }
            else
            {
                Debug.Log("Activated Tails Coin");
                tailsCoin.SetActive(true);
                headsCoin.SetActive(false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndGameServerRPC(int winnerID)
        {
            EndGameClientRPC(winnerID);
        }

        [ClientRpc]
        private void EndGameClientRPC(int winnerID)
        {
            UIManager.Instance.GameOverUI(winnerID);
            isGameReadyToStart = false;
        }
    }
}
