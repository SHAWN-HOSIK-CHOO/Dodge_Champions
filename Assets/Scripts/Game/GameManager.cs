using System;
using System.Collections;
using CharacterAttributes;
using GameInput;
using GameLobby;
using UnityEngine;
using Unity.Netcode;
using GameUI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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
        }

        private float     _turnTime             = 0f;
        private Coroutine _timeCheckerCoroutine = null;
        
        [ClientRpc]
        private void SwapTurnClientRPC()
        {
            currentAttackPlayerID   = currentAttackPlayerID == localClientID ? enemyClientID : localClientID;
            isLocalPlayerAttackTurn = localClientID == currentAttackPlayerID;

            _turnTime = 0f;
            
            if (isLocalPlayerAttackTurn)
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(true);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(false);
                UIManager.Instance.coolDownImage.gameObject.SetActive(true);
                _turnTime = localPlayer.GetComponent<CharacterManager>().turnTimeInSeconds;
                localPlayer.GetComponent<CharacterManager>().hitApproved = true;
                enemyPlayer.GetComponent<CharacterManager>().hitApproved = false;
                InputManager.Instance.canThrowBall = true;
            }
            else
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(false);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(true);
                UIManager.Instance.coolDownImage.gameObject.SetActive(false);
                _turnTime = enemyPlayer.GetComponent<CharacterManager>().turnTimeInSeconds;
                InputManager.Instance.canThrowBall = false;
                localPlayer.GetComponent<CharacterManager>().hitApproved = false;
                enemyPlayer.GetComponent<CharacterManager>().hitApproved = true;
            }

            if (_timeCheckerCoroutine != null)
            {
                StopCoroutine(_timeCheckerCoroutine);
            }
            _timeCheckerCoroutine = StartCoroutine(CoTurnTimeChecker(_turnTime));
        }

        private IEnumerator CoTurnTimeChecker(float times)
        {
            float elapsedTime = 0f;
            UIManager.Instance.turnCoolDownImage.fillAmount = 0f;
            
            while (elapsedTime <= times)
            {
                elapsedTime += Time.deltaTime;
                float ratio = elapsedTime / times;

                UIManager.Instance.turnCoolDownImage.fillAmount = ratio;
                
                yield return null;
            }

            UIManager.Instance.turnCoolDownImage.fillAmount = 1f;
            
            _timeCheckerCoroutine = null;
            
            if(IsOwner)
                SwapTurnServerRPC();
        }
        
        [ServerRpc]
        public void StartRoundServerRPC()
        {
            uiManager.SetActive(false);
            cutSceneCamera.SetActive(true);
            
            int randomNumber = Random.Range(0, 2);
            _isHeads = randomNumber == 0;

            currentAttackPlayerID = _isHeads ? localClientID : enemyClientID;
            
            StartRoundClientRPC(currentAttackPlayerID, randomNumber);
        }

        [ClientRpc]
        private void StartRoundClientRPC(int attackPlayerID, int randomNumber)
        {
            _isHeads = randomNumber == 0;
            
            currentAttackPlayerID   = attackPlayerID;
            
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
                localPlayer.GetComponent<CharacterManager>().hitApproved = true;
            }
            else
            {
                UIManager.Instance.statesUIImages[0].gameObject.SetActive(false);
                UIManager.Instance.statesUIImages[1].gameObject.SetActive(true);
                InputManager.Instance.canThrowBall = false;
                localPlayer.GetComponent<CharacterManager>().hitApproved = false;
            }
            
            UIManager.Instance.StartGameCountDown(5f);
            UIManager.Instance.coolDownImage.gameObject.SetActive(isLocalPlayerAttackTurn);
            
        }

        //TODO: 이거 반드시 해결 이런식으로 사용 X
        public void JustForTheBeginningCoroutine()
        {
            if (isLocalPlayerAttackTurn)
            {
                _turnTime = localPlayer.GetComponent<CharacterManager>().turnTimeInSeconds;
            }
            else
            {
                _turnTime = enemyPlayer.GetComponent<CharacterManager>().turnTimeInSeconds;
            }
            
            _timeCheckerCoroutine = StartCoroutine(CoTurnTimeChecker(_turnTime));
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
        public void EndGameServerRPC(int loserID)
        {
            EndGameClientRPC(loserID);
        }

        [ClientRpc]
        private void EndGameClientRPC(int loserID)
        {
            Debug.Log("End Game RPC Called");
            UIManager.Instance.GameOverUI(loserID);
            isGameReadyToStart = false;
            StartCoroutine(CoReturnLobbyAfterNSeconds(2.5f));
        }

        IEnumerator CoReturnLobbyAfterNSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            PlayerSelectionManager.DestroyAllSingletonsAndEnd();
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
    }
}
