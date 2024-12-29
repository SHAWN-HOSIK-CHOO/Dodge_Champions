using System;
using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UI;

namespace Character
{
   public class CharacterManager : NetworkBehaviour
   { 
       private bool       _isAllClientsConnected = false;
       public  GameObject ballSpawnPosition;
       
       public override void OnNetworkSpawn()
       {
           if (IsServer) 
           {
               NetworkManager.OnClientConnectedCallback += OnClientConnected;
           }

           if (IsOwner)
           {
               GameManager.Instance.localClientID                = (int)OwnerClientId;
               GameManager.Instance.localPlayer                  = this.gameObject;
               GameManager.Instance.localPlayerBallSpawnPosition = ballSpawnPosition.transform;
               this.gameObject.layer                             = LayerMask.NameToLayer("LocalPlayer");
               InputManager.Instance.InitCallWhenLocalPlayerSpawned();
           }
           else
           {
               GameManager.Instance.enemyClientID = (int)OwnerClientId;
               GameManager.Instance.enemyPlayer   = this.gameObject;
               this.gameObject.layer              = LayerMask.NameToLayer("EnemyPlayer");
           }
       }

       private void OnTriggerEnter(Collider other)
       {
           if (other.CompareTag("Fake"))
           {
               Debug.Log(OwnerClientId + " 's prefab is hit");
           }
       }

       [ServerRpc(RequireOwnership = false)]
       public void NotifyHitServerRPC()
       {
           NotifyHitClientRPC();
       }

       [ClientRpc]
       private void NotifyHitClientRPC()
       {
           Debug.Log("Player" + OwnerClientId + " is Hit");
       }

       public bool IsAllClientsConnected()
       {
           return _isAllClientsConnected;
       }
        
       private void OnClientConnected(ulong clientId)
       {
           int connectedClients = NetworkManager.ConnectedClientsList.Count;
            
           if (connectedClients >= 2)
           {
               SetReadyFlagServerRpc(true); 
           }
       }

       [ServerRpc(RequireOwnership = false)]
       private void SetReadyFlagServerRpc(bool readyFlag)
       {
           _isAllClientsConnected = readyFlag;
           Debug.Log($"All clients ready: {_isAllClientsConnected}");
            
           UpdateReadyFlagClientRpc(_isAllClientsConnected);
       }

       [ClientRpc]
       private void UpdateReadyFlagClientRpc(bool readyFlag)
       {
           _isAllClientsConnected = readyFlag;
           Debug.Log($"isReady updated on client: {_isAllClientsConnected}");

           if (_isAllClientsConnected)
           {
               UIManager.Instance.StartGameCountDown(5f);
           }
       }
   }
}
