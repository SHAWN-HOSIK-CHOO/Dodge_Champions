using UnityEngine;

namespace Skill
{
    public interface ISkillInput { }

    public struct TargetVector3Input : ISkillInput
    {
        public Vector3 TargetVector;
    }

    public struct TransformScaleInput : ISkillInput
    {
        public Vector3 TargetScale;
    }

    public struct IntegerIndex : ISkillInput
    {
        public int Index;
    }

    public struct HealInput : ISkillInput
    {
        public float HealAmount;
    }

    public struct PressInput : ISkillInput
    {
        public bool IsPressed;
    }
    
    public interface IFlashBlindable
    {
        public void ApplyBlind(float duration);
    }
}