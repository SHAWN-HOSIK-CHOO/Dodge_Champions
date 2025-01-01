using System;
using System.Collections;
using System.Globalization;
using Game;
using GameInput;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UI;
using Tests;
using UnityEngine.Serialization;

namespace CharacterAttributes
{
   public class CharacterManager : NetworkBehaviour
   { 
       private bool       _isAllClientsConnected = false;
       
       [Header("Ball spawn positions, normally under right hand")]
       public  GameObject ballSpawnPosition;
       
       [Space(5)]
       [Header("Hit detection raycast settings")]
       public Transform capsuleStart;         // 캡슐의 시작점
       public Transform capsuleEnd;           // 캡슐의 끝점
       public float     capsuleRadius = 0.5f; // 캡슐 반경
       public LayerMask ballLayer;            // 공에 해당하는 레이어

       [Space(5)]
       [Header("I-Frame raycast settings")]
       public Transform iFrameStart;
       public float     iFrameSphereRadius = 0.6f;
       public LayerMask iFrameLayer;
       
       [Space(5)]
       [Header("Hit Effects (When ball hit)")]
       public        GameObject[] pfHitEffects                = new GameObject[3];
       public static int          SCurrentComboStack          = 0;
       private       Coroutine    _currentHitDisplayCoroutine = null;
       
       [Space(5)]
       [Header("Combo")]
       public  float     comboValidateTimeInSeconds = 5f;
       private Coroutine _comboTimerCoroutine       = null;
       
       [Space(5)]
       [Tooltip("I-Frame (Just Dodge)")]
       private Coroutine _justDodgeCoroutine;
       private Coroutine  _justDodgeSuccessEffectCoroutine;
       public  GameObject justDodgeEffect;

       private CharacterSkillLauncher _characterSkillLauncher;
       
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
               
               //Tests
                // if((int)OwnerClientId == 1)
                //  PracticeMachineManager.Instance.StartPractice(this.gameObject);
           }
           else
           {
               GameManager.Instance.enemyClientID = (int)OwnerClientId;
               GameManager.Instance.enemyPlayer   = this.gameObject;
               this.gameObject.layer              = LayerMask.NameToLayer("EnemyPlayer");
           }

           _characterSkillLauncher = this.GetComponent<CharacterSkillLauncher>();
       }
       
       private void Update()
       {
           if (_characterSkillLauncher.isSkillActivated)
           {
               DetectIFrame();
           }
           
           DetectBallHit();
       }

       private void DetectIFrame()
       {
           Vector3 sphereCenter = iFrameStart.position;

           Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, iFrameSphereRadius, iFrameLayer);
           
           if (IsOwner)
           {
               foreach (Collider coll in hitColliders)
               {
                   if (coll.CompareTag("Fake") && coll != null)
                   {
                       Debug.Log("Just Dodge 성공!");
                       UIManager.Instance.dodgeText.SetActive(true);
                       NotifyJustDodgeSuccessServerRPC(coll.transform.position);
                       Destroy(coll.gameObject);
                       break;
                   }
               }
           }
           else
           {
               foreach (Collider coll in hitColliders)
               {
                   if (coll.CompareTag("Real") && coll != null)
                   {
                       Destroy(coll.gameObject);
                       break;
                   }
               }
           }
       }

       [ServerRpc]
       private void NotifyJustDodgeSuccessServerRPC(Vector3 effectPosition)
       {
           NotifyJustDodgeSuccessClientRPC(effectPosition);
       }

       [ClientRpc]
       private void NotifyJustDodgeSuccessClientRPC(Vector3 effectPosition)
       {
           TriggerJustDodgeEffects(effectPosition);
       }
       
       private void TriggerJustDodgeEffects(Vector3 collPosition)
       {
           if (_justDodgeSuccessEffectCoroutine != null)
           {
               StopCoroutine(_justDodgeSuccessEffectCoroutine);
           }

           _justDodgeSuccessEffectCoroutine = StartCoroutine(CoDisplayHitEffectForNSec(collPosition, 0.5f, true));
       }
       
       private void DetectBallHit()
       {
           if (_characterSkillLauncher.isSkillActivated)
           {
               // I-Frame 활성화 중이므로 Hit 무시
               return;
           }
           
           Vector3 start = capsuleStart.position;
           Vector3 end   = capsuleEnd.position;

           Collider[] hitColliders = Physics.OverlapCapsule(start, end, capsuleRadius, ballLayer);

           if (IsOwner)
           {
               foreach (Collider coll in hitColliders)
               {
                   if (coll.CompareTag("Fake") && coll != null)
                   {
                       NotifyHitServerRPC(coll.transform.position);

                       Destroy(coll.gameObject);
                       break;
                   }
               }
           }
           else if (!IsOwner)
           {
               foreach (Collider coll in hitColliders)
               {
                   if (coll.CompareTag("Real") && coll != null)
                   {
                       Destroy(coll.gameObject);
                       break;
                   }
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
           
           if (iFrameStart != null)
           {
               Gizmos.color = Color.magenta;
               Vector3 sphereCenter = iFrameStart.position;
               Gizmos.DrawWireSphere(sphereCenter, iFrameSphereRadius);
           }
       }
       
       [ServerRpc(RequireOwnership = false)]
       public void NotifyHitServerRPC(Vector3 hitPosition)
       {
           NotifyHitClientRPC(hitPosition);
       }

       [ClientRpc]
       private void NotifyHitClientRPC(Vector3 hitPosition)
       {
           Debug.Log("Player" + OwnerClientId + " is Hit");

           if (SCurrentComboStack >= 2)
           {
               SCurrentComboStack = 2; // 최대 콤보를 초과하면 유지
           }

           if (_currentHitDisplayCoroutine != null)
           {
               StopCoroutine(_currentHitDisplayCoroutine);
           }

           if (_comboTimerCoroutine != null)
           {
               StopCoroutine(_comboTimerCoroutine);
           }

           // 콤보 증가 전에 이펙트를 실행
           _currentHitDisplayCoroutine = StartCoroutine(CoDisplayHitEffectForNSec(hitPosition, 0.5f));

           // 콤보를 증가시키고 타이머 시작
           SCurrentComboStack   = Mathf.Clamp(SCurrentComboStack + 1, 0, 2);
           _comboTimerCoroutine = StartCoroutine(CoComboValidateTimer(comboValidateTimeInSeconds));
       }

       IEnumerator CoComboValidateTimer(float comboValidateTime)
       {
           float elapsedTime = 0f;

           while (elapsedTime < comboValidateTime)
           {
               elapsedTime += Time.deltaTime;
               yield return null;
           }

           // 콤보 초기화
           SCurrentComboStack = 0;
       }
       
       IEnumerator CoDisplayHitEffectForNSec(Vector3 hitPosition,float sec = 0.5f, bool isJustDodge = false)
       {
           if (isJustDodge)
           {
               GameObject effect = Instantiate(justDodgeEffect, hitPosition, Quaternion.identity);
               yield return new WaitForSeconds(sec);
               Destroy(effect.gameObject);
               //Debug
               UIManager.Instance.dodgeText.SetActive(false);
           }
           else
           {
               GameObject effect = Instantiate(pfHitEffects[SCurrentComboStack], hitPosition, Quaternion.identity);
               yield return new WaitForSeconds(sec);
               Destroy(effect.gameObject);
           }
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
