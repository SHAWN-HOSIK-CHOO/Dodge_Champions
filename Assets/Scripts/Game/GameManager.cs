using CharacterAttributes;
using GameInput;
using GameLobby;
using GameUI;
using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    public class GameManager : NetworkBehaviour
    {
        private static GameManager _instance = null;
        public static GameManager Instance => _instance == null ? null : _instance;

        public GameObject cinemachineCamera;
        public GameObject mainCamera;
        public GameObject cutSceneCamera;

        public bool isGameReadyToStart = false;

        public GameObject localPlayerBall;
        public GameObject enemyPlayerBall;

        public PlayableDirector cutSceneDirector = null;
        private Coroutine _cutSceneCoroutine = null;

        public GameObject headsCoin = null;
        public GameObject tailsCoin = null;

        private bool _isHeads = false;

        public GameObject uiManager;

        public int timePerRound = 40;

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

            localPlayerBall =
                PlayerSelectionManager.Instance.characterReferences[PlayerSelectionManager.Instance.GetLocalPlayerSelection()]
                                  .pfBall;
            localPlayerBall.gameObject.layer = LayerMask.NameToLayer("RealPf");
            localPlayerBall.gameObject.tag = "Real";

            enemyPlayerBall = PlayerSelectionManager.Instance
                                                     .characterReferences[PlayerSelectionManager.Instance.GetEnemySelection()].pfBallForEnemy;
            enemyPlayerBall.gameObject.layer = LayerMask.NameToLayer("FakePf");
            enemyPlayerBall.gameObject.tag = "Fake";

            //TODO: 스킬도 설정
        }

        public int localClientID;
        public int enemyClientID;

        public GameObject localPlayer = null;
        public GameObject enemyPlayer = null;

        [ServerRpc(RequireOwnership = false)]
        public void RestartRoundServerRPC(int roundTime = 30, int standByTime = 10)
        {
            RestartRoundClientRPC(roundTime, standByTime);
        }

        private Coroutine _timeCheckerCoroutine = null;

        [ClientRpc]
        private void RestartRoundClientRPC(int roundTime = 30, int standByTime = 10)
        {
            {
                localPlayer.GetComponent<CharacterManager>().hitApproved = true;
                enemyPlayer.GetComponent<CharacterManager>().hitApproved = true;
                InputManager.Instance.canThrowBall = true;

                if (enemyPlayer.GetComponent<CharacterBallLauncher>().instantiatedBall != null)
                {
                    enemyPlayer.GetComponent<CharacterBallLauncher>().DestroyInstantiatedBall();
                }

                if (localPlayer.GetComponent<CharacterBallLauncher>().instantiatedBall != null)
                {
                    localPlayer.GetComponent<CharacterBallLauncher>().DestroyInstantiatedBall();
                }
            }

            if (_timeCheckerCoroutine != null)
            {
                StopCoroutine(_timeCheckerCoroutine);
            }
            _timeCheckerCoroutine = StartCoroutine(CoCountBeforeRoundStart((float)roundTime, (float)standByTime));
        }

        private IEnumerator CoCountBeforeRoundStart(float times, float standByTime = 10.0f)
        {
            float elapsedStandByTime = 0f;
            isGameReadyToStart = false;
            InputManager.Instance.playerInput.enabled = false;

            foreach (var action in InputManager.Instance.playerInput.actions)
            {
                action.Disable();
            }

            if (localPlayer != null && enemyPlayer != null)
            {
                {
                    localPlayer.GetComponent<CharacterMovement>().SetAnimationIdle();
                    enemyPlayer.GetComponent<CharacterMovement>().SetAnimationIdle();
                    localPlayer.GetComponent<CharacterController>().Move(Vector3.zero);
                    enemyPlayer.GetComponent<CharacterController>().Move(Vector3.zero);
                }
            }

            UIManager.Instance.StartGameCountDown(standByTime);
            while (elapsedStandByTime <= standByTime)
            {
                elapsedStandByTime += Time.deltaTime;
                yield return null;
            }

            isGameReadyToStart = true;

            foreach (var action in InputManager.Instance.playerInput.actions)
            {
                action.Enable();
            }

            InputManager.Instance.playerInput.enabled = true;

            float leftTime = times;

            UIManager.Instance.roundTimer.text = leftTime.ToString(CultureInfo.InvariantCulture);

            while (leftTime >= 0)
            {
                leftTime -= Time.deltaTime;

                UIManager.Instance.roundTimer.text = Mathf.Clamp(Mathf.FloorToInt(leftTime), 0, times).ToString(CultureInfo.InvariantCulture);

                yield return null;
            }

            UIManager.Instance.roundTimer.text = times.ToString(CultureInfo.InvariantCulture);

            _timeCheckerCoroutine = null;

            if (IsServer)
                RestartRoundServerRPC(timePerRound);
        }

        [ServerRpc]
        public void StartGameServerRPC()
        {
            cutSceneCamera.SetActive(true);

            StartGameClientRPC();
        }

        [ClientRpc]
        private void StartGameClientRPC()
        {
            if (_cutSceneCoroutine != null)
            {
                StopCoroutine(CoIntroAnimationPlay());
            }

            _cutSceneCoroutine = StartCoroutine(CoIntroAnimationPlay());
        }

        private IEnumerator CoIntroAnimationPlay()
        {
            UIManager.Instance.SetActiveAllObjects(false);
            cutSceneDirector.Play();

            while (cutSceneDirector.state == PlayState.Playing)
            {
                yield return null;
            }

            if (IsServer)
            {
                RestartRoundServerRPC(timePerRound, 5);
            }

            cutSceneCamera.SetActive(false);
            UIManager.Instance.SetActiveAllObjects(true);
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
