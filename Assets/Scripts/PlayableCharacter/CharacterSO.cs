using Skill;
using System;
using UnityEngine;

namespace PlayableCharacter
{
    [CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
    public class CharacterSO : ScriptableObject
    {
        public String characterName;
        public GameObject pfCharacter;
        public GameObject pfBall;
        public GameObject pfBallForEnemy;
        public SkillBase skillSO;
    }
}
