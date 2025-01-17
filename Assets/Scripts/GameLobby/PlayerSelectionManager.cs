using Game;
using GameInput;
using GameUI;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Core;  // Unity Services 초기화 관련 API

using UnityEngine.SceneManagement;

namespace GameLobby
{
    public struct PlayerSelectionData : INetworkSerializable
    {
        public int BallIndex;
        public int SkillIndex;

        // 기본 생성자
        public PlayerSelectionData(int ballIndex, int skillIndex)
        {
            BallIndex  = ballIndex;
            SkillIndex = skillIndex;
        }

        // INetworkSerializable 구현
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref BallIndex);
            serializer.SerializeValue(ref SkillIndex);
        }
    }
   public class PlayerSelectionManager : NetworkBehaviour
    {
        private NetworkVariable<PlayerSelectionData> player0Selection = new NetworkVariable<PlayerSelectionData>();
        private NetworkVariable<PlayerSelectionData> player1Selection = new NetworkVariable<PlayerSelectionData>();

        public static PlayerSelectionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                player0Selection.OnValueChanged += OnPlayer0SelectionChanged;
                player1Selection.OnValueChanged += OnPlayer1SelectionChanged;
            }
        }
        
        public static void DestroyAllSingletonsAndEnd()
        {
            // 마우스 커서 상태 초기화
            Cursor.lockState = CursorLockMode.None;
            NetworkManager.Singleton.Shutdown();
            
            Destroy(InputManager.Instance.gameObject);
            Destroy(UIManager.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene("Lobby");
        }
        
        public override void OnDestroy()
        {
            if (player0Selection != null)
                player0Selection.OnValueChanged -= OnPlayer0SelectionChanged;

            if (player1Selection != null)
                player1Selection.OnValueChanged -= OnPlayer1SelectionChanged;
            
            base.OnDestroy();
        }

        // 플레이어가 선택한 데이터를 서버로 전송
        public void SetPlayerSelection(int ballIndex, int skillIndex)
        {
            if (IsClient)
            {
                SubmitPlayerSelectionServerRpc(ballIndex, skillIndex);
            }
        }

        // 서버에서 플레이어 데이터를 업데이트
        [ServerRpc(RequireOwnership = false)]
        private void SubmitPlayerSelectionServerRpc(int ballIndex, int skillIndex, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            
            Debug.Log("Received request from client : " + clientId);

            PlayerSelectionData data = new PlayerSelectionData
                                       {
                                           BallIndex  = ballIndex,
                                           SkillIndex = skillIndex
                                       };

            // ClientId에 따라 올바른 NetworkVariable에 데이터 저장
            if (clientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                player0Selection.Value = data;
            }
            else if (clientId == NetworkManager.Singleton.ConnectedClientsList[1].ClientId)
            {
                player1Selection.Value = data;
            }
            else
            {
                Debug.LogError($"Unexpected ClientId: {clientId}. Unable to assign selection data.");
            }
        }

        // 선택 데이터 변경 콜백
        private void OnPlayer0SelectionChanged(PlayerSelectionData oldValue, PlayerSelectionData newValue)
        {
            Debug.Log($"Player 0 Selection Updated: Ball={newValue.BallIndex}, Skill={newValue.SkillIndex}");
        }

        private void OnPlayer1SelectionChanged(PlayerSelectionData oldValue, PlayerSelectionData newValue)
        {
            Debug.Log($"Player 1 Selection Updated: Ball={newValue.BallIndex}, Skill={newValue.SkillIndex}");
        }

        // 클라이언트 입장에서 데이터를 가져오기
        public PlayerSelectionData GetLocalPlayerSelection()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (localClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                return player0Selection.Value;
            }
            else
            {
                return player1Selection.Value;
            }
        }

        public PlayerSelectionData GetEnemySelection()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (localClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                return player1Selection.Value;
            }
            else
            {
                return player0Selection.Value;
            }
        }
    }
}
