using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;


namespace GameLobby
{
    public class GameStarter : MonoBehaviour
    {
        public Button         hostButton;
        public Button         joinButton;
        public TMP_Text       joinCodeText;
        public TMP_InputField joinCodeInputField;

        async void Start()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            hostButton.onClick.AddListener(CreateRelay);
            joinButton.onClick.AddListener(() => JoinRelay(joinCodeInputField.text));
        }

        public void StartGame()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Stage", LoadSceneMode.Single);
            }
        }

        async void CreateRelay()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string     joinCode   = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.text = "Code: " + joinCode;

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation,"dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }

        async void JoinRelay(string code)
        {
            var joinAllocation  = await RelayService.Instance.JoinAllocationAsync(code);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation,"dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
    }
}
