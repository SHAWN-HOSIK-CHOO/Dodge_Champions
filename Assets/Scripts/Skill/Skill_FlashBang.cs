using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Skill
{
   [CreateAssetMenu(fileName = "Skills", menuName = "Skills/FlashBang")]
   public class Skill_FlashBang : SkillBase
   {
      public override ESkillInputType ThisSkillType => ESkillInputType.RayVec3Target;

      [Header("FlashBang Settings")]
      public float flashRadius = 5f;
      public float      blindDuration = 3f;
      public LayerMask  enemyLayer;
      public GameObject pfFlashBang; 

      public override IEnumerator Activate(CharacterController controller, ISkillInput input)
      {
         if (input is not TargetVector3Input targetInput)
         {
            Debug.LogError("Wrong input type");
            yield break;
         }

         if (pfFlashBang != null)
         {
            GameObject flashBang = Instantiate(
                                               pfFlashBang,
                                               controller.transform.position + Vector3.up * 1.5f, // 캐릭터 위에서 시작
                                               Quaternion.identity
                                              );

            if (flashBang.TryGetComponent<FlashBangProjectile>(out var projectile))
            {
               projectile.Initialize(
                                     targetInput.TargetVector,
                                     flashRadius,
                                     blindDuration,
                                     enemyLayer
                                    );
            }
         }

         yield return null;
      }

   }
}
