using Ball;
using Game;
using GameInput;
using SinglePlayer;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CharacterAttributes
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Ball spawn positions, normally under right hand")]
        public GameObject ballSpawnPosition;

        [Space(5)]
        [Header("Hit detection raycast settings")]
        public Transform capsuleStart;         // 캡슐의 시작점
        public Transform capsuleEnd;           // 캡슐의 끝점
        public float capsuleRadius = 0.5f; // 캡슐 반경
        public LayerMask ballLayer;            // 공에 해당하는 레이어

        [Space(5)]
        [Header("I-Frame raycast settings")]
        public Transform iFrameStart;
        public float iFrameSphereRadius = 0.6f;
        public LayerMask iFrameLayer;

        [Space(5)]
        [Header("Hit Effects (When ball hit)")]
        public GameObject[] pfHitEffects = new GameObject[7];
        private Coroutine _currentHitDisplayCoroutine = null;

        [Space(5)]
        [Tooltip("I-Frame (Just Dodge)")]
        private Coroutine _justDodgeCoroutine;
        private Coroutine _justDodgeSuccessEffectCoroutine;
        public GameObject justDodgeEffect;

        private CharacterSkillLauncher _characterSkillLauncher;

        [Header("Throw Counts")]
        public int maxThrowCount = 5;
        private int _currentThrowCount = 0;

        private Coroutine _pushCoroutine = null;

        public override void OnNetworkSpawn()
        {
            if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
            {
                if (IsOwner)
                {
                    Debug.Log("Called by " + (int)OwnerClientId);
                    GameManager.Instance.localClientID = (int)OwnerClientId;
                    GameManager.Instance.localPlayer = this.gameObject;
                    this.gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
                    InputManager.Instance.InitCallWhenLocalPlayerSpawned(this.gameObject);
                    //TODO: Set turn time, Throw time(input manager) ,Skill cooldown도 설정(skillLauncher 통해서),
                }
                else
                {
                    GameManager.Instance.enemyClientID = (int)OwnerClientId;
                    GameManager.Instance.enemyPlayer = this.gameObject;
                    this.gameObject.layer = LayerMask.NameToLayer("EnemyPlayer");
                    //TODO: Set turn time 공에 따라....
                }
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                this.gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
                InputManager.Instance.InitCallWhenLocalPlayerSpawned(this.gameObject);
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
            this.GetComponent<CharacterStatus>().IncreaseDodgeSuccessCount();
            Debug.Log(OwnerClientId + " Dodge Success : " + GetComponent<CharacterStatus>().justDodgeSuccessCounts);

            if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
            {
                //만약 내가 공을 던진 사람이고 상대방이 Just Dodge로 피한 사람이고, 상대 턴이 아니라면(내 턴이라면)
                if (!IsOwner )
                {
                    //턴을 넘겨준다
                    //TODO: 턴제 삭제로 인해 EndCurrentRoundServerRPC가 아닌 보상 매카니즘이 이식되어야 한다.
                    //GameManager.Instance.EndCurrentRoundServerRPC();
                }
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                
            }

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
            Vector3 end = capsuleEnd.position;

            Collider[] hitColliders = Physics.OverlapCapsule(start, end, capsuleRadius, ballLayer);

            if (IsOwner)
            {
                foreach (Collider coll in hitColliders)
                {
                    if (coll.CompareTag("Fake") && coll != null)
                    {
                        int damage = coll.GetComponent<BallScript>().ballDamage;
                        int hitEffectIndex = coll.GetComponent<BallScript>().hitEffectIndex;
                        bool isInfinite = coll.GetComponent<BallScript>().isInfinite;

                        NotifyHitServerRPC(coll.transform.position, damage, hitEffectIndex, isInfinite);

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
                        //Debug.Log("Ball hit detect from none owner");
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
                Vector3 end = capsuleEnd.position;

                // 캡슐의 중심축 벡터 계산
                Vector3 capsuleAxis = end - start;
                float distance = capsuleAxis.magnitude;

                // 캡슐의 중심 위치
                Vector3 center = (start + end) * 0.5f;

                // 원통 부분
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, capsuleAxis.normalized);
                Matrix4x4 originalMatrix = Gizmos.matrix;

                Gizmos.matrix = Matrix4x4.TRS(center, rotation, new Vector3(capsuleRadius * 2, distance * 0.5f, capsuleRadius * 2));
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(1, 1, 1)); // 원통 부분

                // 원 부분 (캡슐의 끝부분)
                Gizmos.matrix = originalMatrix;
                Gizmos.DrawWireSphere(start, capsuleRadius);
                Gizmos.DrawWireSphere(end, capsuleRadius);
            }

            if (iFrameStart != null)
            {
                Gizmos.color = Color.magenta;
                Vector3 sphereCenter = iFrameStart.position;
                Gizmos.DrawWireSphere(sphereCenter, iFrameSphereRadius);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void NotifyHitServerRPC(Vector3 hitPosition, int damage, int effectIndex = 0, bool isInfinite = false)
        {
            if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
            {
                NotifyHitClientRPC(hitPosition, damage, effectIndex, isInfinite);
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                NotifyHitSinglePlayer(hitPosition, damage, effectIndex, isInfinite);
            }
        }

        public bool hitApproved = false;

        [ClientRpc]
        private void NotifyHitClientRPC(Vector3 hitPosition, int damage, int effectIndex = 0, bool isInfinite = false)
        {
            Debug.Log("Player" + OwnerClientId + " is Hit");
            //체력 조절

            this.GetComponent<CharacterStatus>().HandleHit(damage);
            this.GetComponent<CharacterStatus>().IncreaseHitCount();

            if (!IsOwner && isInfinite)
            {
                hitApproved = true;
                //InputManager.Instance.canThrowBall = true;
            }

            if (!IsOwner && !isInfinite) // 즉, 내가 공을 맞았고, 상대가 던진 사람이라면 그리고 무한볼이 아니라면
            {
                //먼저 상대의 throw count를 올린다.
                IncreaseThrowCount();
                //만약 상대의 맥스 카운트에 도달하지 않았다면
                if (_currentThrowCount < maxThrowCount)
                {
                    //한번 더 던질 수 있게 한다,
                    hitApproved = true;
                    //InputManager.Instance.canThrowBall = true;
                    //Debug.Log("Current Throw Count / Max : "+_currentThrowCount + " / " + maxThrowCount);
                }
                else
                {
                    //그게 아니라면 턴을 넘겨받는다.
                    //TODO: 턴제 삭제로 추가 작업 필요
                    hitApproved = false;
                    //GameManager.Instance.EndCurrentRoundServerRPC();
                }
            }

            if (_currentHitDisplayCoroutine != null)
            {
                StopCoroutine(_currentHitDisplayCoroutine);
            }

            _currentHitDisplayCoroutine = StartCoroutine(CoDisplayHitEffectForNSec(hitPosition, 0.5f, false, effectIndex));

            //TODO: 코드 다시 작성하기, 현재 effectIndex로 공 종류 구분 중
            if (effectIndex == 6)
            {
                //상대를 미는 공이라면
                if (_pushCoroutine != null)
                {
                    StopCoroutine(_pushCoroutine);
                }

                float pushStrength = 30f;
                float duration = 0.1f;

                // 새 밀림 효과 시작
                _pushCoroutine = StartCoroutine(CoPush(hitPosition, pushStrength, duration));
            }
        }

        private void NotifyHitSinglePlayer(Vector3 hitPosition, int damage, int effectIndex = 0, bool isInfinite = false)
        {
            this.GetComponent<CharacterStatus>().HandleHit(damage);
            this.GetComponent<CharacterStatus>().IncreaseHitCount();

            if (_currentHitDisplayCoroutine != null)
            {
                StopCoroutine(_currentHitDisplayCoroutine);
            }

            _currentHitDisplayCoroutine = StartCoroutine(CoDisplayHitEffectForNSec(hitPosition, 0.5f, false, effectIndex));

            //TODO: 코드 다시 작성하기, 현재 effectIndex로 공 종류 구분 중
            if (effectIndex == 6)
            {
                //상대를 미는 공이라면
                if (_pushCoroutine != null)
                {
                    StopCoroutine(_pushCoroutine);
                }

                float pushStrength = 30f;
                float duration = 0.1f;

                // 새 밀림 효과 시작
                _pushCoroutine = StartCoroutine(CoPush(hitPosition, pushStrength, duration));
            }
        }

        private IEnumerator CoPush(Vector3 hitPosition, float pushStrength, float duration)
        {
            var characterController = this.GetComponent<CharacterController>();
            if (characterController == null)
            {
                Debug.LogWarning("CharacterController is missing");
                yield break;
            }

            // 밀리는 방향 계산
            Vector3 pushDirection = (this.transform.position - hitPosition).normalized;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // 밀림 효과 적용
                characterController.Move(pushDirection * (pushStrength * Time.deltaTime));

                // 시간 경과
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Coroutine 종료
            _pushCoroutine = null;
        }

        public void IncreaseThrowCount()
        {
            _currentThrowCount++;
        }
        
        public void ResetThrowCount()
        {
            _currentThrowCount = 0;
        }

        IEnumerator CoDisplayHitEffectForNSec(Vector3 hitPosition, float sec = 0.5f, bool isJustDodge = false,
                                              int effectIndex = 0)
        {
            if (isJustDodge)
            {
                GameObject effect = Instantiate(justDodgeEffect, hitPosition, Quaternion.identity);
                yield return new WaitForSeconds(sec);
                Destroy(effect.gameObject);
            }
            else
            {
                GameObject effect = Instantiate(pfHitEffects[effectIndex], hitPosition, Quaternion.identity);
                yield return new WaitForSeconds(sec);
                Destroy(effect.gameObject);
            }
        }
    }
}
