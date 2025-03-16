
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmptySceneManager : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Z))
        {
            NetworkManager.Singleton.SceneManager.OnLoad += OnLoad;
            NetworkManager.Singleton.StartHost();

        }
    }

    void OnLoad(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        FreeNet._instance._ngoManager.SceneManager.OnLoad -= OnLoad;
       // _coroutineHandler.BeginCoroutine(() => { return EndLoadLobby(clientId, sceneName, asyncOperation); });
    }
}
