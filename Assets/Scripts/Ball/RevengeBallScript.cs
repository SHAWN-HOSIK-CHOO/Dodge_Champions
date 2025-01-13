using CharacterAttributes;
using Game;
using UnityEngine;

namespace Ball
{
    public class RevengeBallScript : BallScript
    {
        public override void Initialize(Vector3 target, float speed, float height, bool horizontalThrow = false, bool isThisOwner = true)
        {
            base.Initialize(target, speed, height, horizontalThrow, isThisOwner);
            if (_ownerSpawnThisBall)
            {
                int hitCount = GameManager.Instance.localPlayer.GetComponent<CharacterStatus>().hitCounts;

                if (hitCount >= 8)
                {
                    hitEffectIndex =  4;
                    ballDamage     = 40;
                }
                else if (hitCount >= 4)
                {
                    hitEffectIndex =  3;
                    ballDamage     = 20;
                }
                else
                {
                    hitEffectIndex = 3;
                }
                
                Debug.Log("Ball Damage : " + ballDamage);
            }
            else
            {
                int hitCount = GameManager.Instance.enemyPlayer.GetComponent<CharacterStatus>().hitCounts;
                
                if (hitCount >= 8)
                {
                    hitEffectIndex =  4;
                    ballDamage     = 40;
                }
                else if (hitCount >= 4)
                {
                    hitEffectIndex =  3;
                    ballDamage     = 20;
                }
                else
                {
                    hitEffectIndex = 3;
                }
                
                Debug.Log("Ball Damage : " + ballDamage);
            }
        }

        public override float GetCalculatedBallSpeed()
        {
            return base.GetCalculatedBallSpeed();
        }

        public override float GetCalculatedBallHeight()
        {
            return base.GetCalculatedBallHeight();
        }
    }
}
