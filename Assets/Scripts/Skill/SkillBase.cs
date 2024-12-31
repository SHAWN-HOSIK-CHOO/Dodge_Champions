using System;
using System.Collections;
using UnityEngine;

namespace Skill
{
    public abstract class SkillBase : ScriptableObject
    {
        public abstract IEnumerator Activate(CharacterController controller, ISkillInput input);
    }
}
