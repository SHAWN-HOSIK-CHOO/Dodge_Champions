using System;
using GameUI;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
   public class PlayerSpawner : NetworkBehaviour
   {
      public GameObject pfPlayer;    
      public Transform  spawnPoint1; 
      public Transform  spawnPoint2; 

      private bool _isSpawned = false;
      
      private void SpawnPlayers()
      {
         _isSpawned = true;
         
         if (IsServer) 
         {
            SpawnPlayer(spawnPoint1.position, spawnPoint1.rotation, 0); 
            SpawnPlayer(spawnPoint2.position, spawnPoint2.rotation, 1); 
            
            GameManager.Instance.StartRoundServerRPC();
         }
      }
      
      private void SpawnPlayer(Vector3 position, Quaternion rotation, int playerIndex)
      {
         // 플레이어 프리팹 인스턴스화
         GameObject playerInstance = Instantiate(pfPlayer, position, rotation);

         // 네트워크 오브젝트로 스폰
         NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
         if (networkObject != null)
         {
            // 특정 클라이언트에 소유권 부여
            ulong clientId = NetworkManager.Singleton.ConnectedClientsList[playerIndex].ClientId;
            networkObject.SpawnWithOwnership(clientId);
         }
      }

      private void Update()
      {
         if (!_isSpawned &&Input.GetKeyDown(KeyCode.Z))
         {
            SpawnPlayers();
            //UIManager.Instance.Initialize();
         }
      }
   }
}