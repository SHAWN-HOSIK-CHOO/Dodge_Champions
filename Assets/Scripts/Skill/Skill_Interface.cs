using UnityEngine;

namespace Skill
{
   public interface ISkillInput {}

   public struct TargetVector3Input : ISkillInput
   {
      public Vector3 TargetVector;
   }

   public struct TransformScaleInput : ISkillInput
   {
      public Vector3 TargetScale;
   }
   
   public struct HealInput : ISkillInput
   {
      public float HealAmount;
   }
}