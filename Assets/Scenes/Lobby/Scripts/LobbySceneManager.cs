using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
    EOSNet _eosNet;
    localUser _localUser;
    void Start()
    {
        _eosNet = SingletonMonoBehaviour<EOSNet>._instance;
        _localUser = SingletonMonoBehaviour<localUser>._instance;
    }
}
