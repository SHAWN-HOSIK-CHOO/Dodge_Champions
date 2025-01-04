using System.Collections;
using UnityEngine;

namespace Skill
{
    [CreateAssetMenu(fileName = "Skills", menuName = "Skills/Dash")]
    public class Skill_Dash : SkillBase
    {
        public override ESkillInputType ThisSkillType => ESkillInputType.Vector3Target;
        
        public float dashSpeed       = 20f;
        public float dashDuration    = 0.2f;
        public float bounceBackForce = 2f; // 튕겨져 나오는 힘

        private Vector3 _direction;
        
        public override IEnumerator Activate(CharacterController characterController, ISkillInput input)
        {
            if (input is TargetVector3Input dashInput)
            {
                _direction = dashInput.TargetVector.normalized; // 방향 벡터를 정규화
            }
            else
            {
                Debug.LogError("Wrong input type for dash input");
                yield break;
            }

            if (characterController == null)
            {
                Debug.LogError("CharacterController is missing on this GameObject!");
                yield break;
            }

            float dashTimeRemaining = dashDuration;

            while (dashTimeRemaining > 0)
            {
                // 대시 이동
                Vector3 move = _direction * (dashSpeed * Time.deltaTime);

                // 이동 및 충돌 감지
                CollisionFlags flags = characterController.Move(move);

                // 벽 충돌 처리
                if ((flags & CollisionFlags.Sides) != 0)
                {
                    //Debug.Log("Collision detected, bouncing back");
                    Vector3 collisionNormal = -_direction; // 충돌 방향 반대
                    Vector3 bounceDirection = Vector3.Reflect(_direction, collisionNormal).normalized;

                    characterController.Move(bounceDirection * (bounceBackForce * Time.deltaTime));
                    break; // 대시 중단
                }

                // 남은 시간 감소
                dashTimeRemaining -= Time.deltaTime;
                //Debug.Log($"Dash Time Remaining: {dashTimeRemaining}");

                yield return null; // 다음 프레임 대기
            }

            Debug.Log("Dash ended");
        }
    }
}
