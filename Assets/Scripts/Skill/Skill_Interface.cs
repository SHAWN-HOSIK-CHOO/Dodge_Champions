using UnityEngine;

namespace Skill
{
   public interface ISkillInput {}

   public struct DashInput : ISkillInput
   {
      public Vector3 TargetVector;
   }

   public struct HealInput : ISkillInput
   {
      public float HealAmount;
   }
}