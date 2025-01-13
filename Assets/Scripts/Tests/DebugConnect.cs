using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

namespace Tests
{
    public class DebugConnect : MonoBehaviour
    {
        public Button startHost;
        public Button startClient;

        private void Start()
        {
            startHost.onClick.AddListener(StartHost);
            startClient.onClick.AddListener(StartClient);
        }

        private void StartHost()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                Debug.LogError("NetworkManager is null");
            }
        }

        private void StartClient()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                Debug.LogError("NetworkManager is null");
            }
        }
    }
}
