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
    }
}
