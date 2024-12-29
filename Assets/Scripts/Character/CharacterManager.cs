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
       
       public Transform capsuleStart;         // 캡슐의 시작점
       public Transform capsuleEnd;           // 캡슐의 끝점
       public float     capsuleRadius = 0.5f; // 캡슐 반경
       public LayerMask ballLayer;            // 공에 해당하는 레이어
       
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

       private void Update()
       {
           if (!IsOwner)
           {
               return;
           }
           
           DetectBallHit();
       }

       private void DetectBallHit()
       {
           Vector3 start = capsuleStart.position;
           Vector3 end   = capsuleEnd.position;

           Collider[] hitColliders = Physics.OverlapCapsule(start, end, capsuleRadius, ballLayer);

           foreach (Collider coll in hitColliders)
           {
               if (coll.CompareTag("Fake"))
               {
                   NotifyHitServerRPC();
                   Destroy(coll.gameObject);
                   break; // 첫 번째 충돌 처리 후 종료 (필요 시 제거 가능)
               }
           }
       }

       private void OnDrawGizmos()
       {
           if (capsuleStart != null && capsuleEnd != null)
           {
               Gizmos.color = Color.red;

               Vector3 start = capsuleStart.position;
               Vector3 end   = capsuleEnd.position;

               // 캡슐의 중심축 벡터 계산
               Vector3 capsuleAxis = end - start;
               float   distance    = capsuleAxis.magnitude;

               // 캡슐의 중심 위치
               Vector3 center = (start + end) * 0.5f;

               // 원통 부분
               Quaternion rotation       = Quaternion.FromToRotation(Vector3.up, capsuleAxis.normalized);
               Matrix4x4  originalMatrix = Gizmos.matrix;

               Gizmos.matrix = Matrix4x4.TRS(center, rotation, new Vector3(capsuleRadius * 2, distance * 0.5f, capsuleRadius * 2));
               Gizmos.DrawWireCube(Vector3.zero, new Vector3(1, 1, 1)); // 원통 부분

               // 원 부분 (캡슐의 끝부분)
               Gizmos.matrix = originalMatrix;
               Gizmos.DrawWireSphere(start, capsuleRadius);
               Gizmos.DrawWireSphere(end,   capsuleRadius);
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
