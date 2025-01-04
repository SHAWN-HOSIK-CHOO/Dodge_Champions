using System;
using System.Collections;
using UnityEngine;

namespace Skill
{
    public enum ESkillInputType
    {
        Vector3Target, //방향
        Scalar3Value, // Transform Scale 등
        JustBoolean,  // 딸깍
        Count
    }
    
    public abstract class SkillBase : ScriptableObject
    {
        public abstract ESkillInputType ThisSkillType { get; }
        public abstract IEnumerator     Activate(CharacterController controller, ISkillInput input);
    }
}
