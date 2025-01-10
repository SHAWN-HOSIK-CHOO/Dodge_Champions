using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLobby
{
    public class GameStarter : MonoBehaviour
    {
        public void StartGame()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                // 서버에서 StageScene으로 이동
                NetworkManager.Singleton.SceneManager.LoadScene("Stage", LoadSceneMode.Single);
            }
        }
        
        public void StartHost()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartHost();
                Debug.Log("Host started");
            }
            else
            {
                Debug.LogError("NetworkManager is not set up!");
            }
        }

        public void StartClient()
        {
            Debug.Log("StartClient button clicked");
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartClient();
                Debug.Log("Client started");
            }
            else
            {
                Debug.LogError("NetworkManager is not set up!");
            }
        }
    }
}
