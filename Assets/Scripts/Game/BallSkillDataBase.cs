using Skill;
using UnityEngine;

namespace Game
{
    public class BallSkillDataBase : MonoBehaviour
    {
        public GameObject[] pfPlayerBalls = new GameObject[3];
        public GameObject[] pfEnemyBalls  = new GameObject[3];
        public SkillBase[]  soSkills      = new SkillBase[1];
    }
}
