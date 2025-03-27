using Game;
using GameInput;
using GameUI;
using PlayableCharacter;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLobby
{
    public class PlayerSelectionManager : NetworkBehaviour
    {
        private NetworkVariable<int> _player0SelectedCharacter = new NetworkVariable<int>(0);
        private NetworkVariable<int> _player1SelectedCharacter = new NetworkVariable<int>(0);

        public static PlayerSelectionManager Instance { get; private set; }

        public CharacterSO[] characterReferences = new CharacterSO[6];

        public CharacterSO[] confirmedCharacterSOs = new CharacterSO[2];

        public bool[] hasPlayersConfirmed = new bool[2] { false, false };

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
                _player0SelectedCharacter.OnValueChanged += OnPlayer0SelectionChanged;
                _player1SelectedCharacter.OnValueChanged += OnPlayer1SelectionChanged;

                for (int i = 0; i < 2; i++)
                {
                    confirmedCharacterSOs[i] = characterReferences[0];
                }
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
            if (_player0SelectedCharacter != null)
                _player0SelectedCharacter.OnValueChanged -= OnPlayer0SelectionChanged;

            if (_player1SelectedCharacter != null)
                _player1SelectedCharacter.OnValueChanged -= OnPlayer1SelectionChanged;

            base.OnDestroy();
        }

        public void SetPlayerSelection(int index)
        {
            if (IsClient)
            {
                SubmitPlayerSelectionServerRpc(index);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SubmitPlayerSelectionServerRpc(int characterIndex, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            Debug.Log("Received request from client : " + clientId);

            if (clientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                _player0SelectedCharacter.Value = characterIndex;
                hasPlayersConfirmed[0] = true;
            }
            else if (clientId == NetworkManager.Singleton.ConnectedClientsList[1].ClientId)
            {
                _player1SelectedCharacter.Value = characterIndex;
                hasPlayersConfirmed[1] = true;
            }
            else
            {
                Debug.LogError($"Unexpected ClientId: {clientId}. Unable to assign selection data.");
            }

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                if (hasPlayersConfirmed[0] && hasPlayersConfirmed[1])
                    NetworkManager.Singleton.SceneManager.LoadScene("Stage", LoadSceneMode.Single);
            }
        }

        private void OnPlayer0SelectionChanged(int oldValue, int newValue)
        {
            Debug.Log($"Player 0 Selection Updated: {newValue}");
            confirmedCharacterSOs[0] = characterReferences[newValue];
        }

        private void OnPlayer1SelectionChanged(int oldValue, int newValue)
        {
            Debug.Log($"Player 1 Selection Updated: {newValue}");
            confirmedCharacterSOs[1] = characterReferences[newValue];
        }

        public int GetLocalPlayerSelection()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (localClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                return _player0SelectedCharacter.Value;
            }
            else
            {
                return _player1SelectedCharacter.Value;
            }
        }

        public int GetEnemySelection()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (localClientId == NetworkManager.Singleton.ConnectedClientsList[0].ClientId)
            {
                return _player1SelectedCharacter.Value;
            }
            else
            {
                return _player0SelectedCharacter.Value;
            }
        }

        public int GetSelectionByIndex(int index)
        {
            if (index == 0)
            {
                return _player0SelectedCharacter.Value;
            }
            else if (index == 1)
            {
                return _player1SelectedCharacter.Value;
            }
            else
            {
                Debug.LogError("Invalid player index");
                return 0;
            }
        }
    }
}
