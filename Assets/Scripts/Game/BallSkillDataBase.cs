using Skill;
using UnityEngine;

namespace Game
{
    public class BallSkillDataBase : MonoBehaviour
    {
        public GameObject[] pfPlayerBalls = new GameObject[6];
        public GameObject[] pfEnemyBalls  = new GameObject[6];
        public SkillBase[]  soSkills      = new SkillBase[1];
    }
}
