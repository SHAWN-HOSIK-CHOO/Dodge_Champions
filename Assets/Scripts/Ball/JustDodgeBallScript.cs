using CharacterAttributes;
using Game;
using UnityEngine;

namespace Ball
{
    public class JustDodgeBallScript : BallScript
    {
        public override void Initialize(Vector3 target, float speed, float height, bool horizontalThrow = false, bool isThisOwner = true)
        {
            base.Initialize(target, speed, height, horizontalThrow, isThisOwner);
            if (_ownerSpawnThisBall)
            {
                int successCount = GameManager.Instance.localPlayer.GetComponent<CharacterStatus>()
                                              .justDodgeSuccessCounts;

                if (successCount >= 4)
                {
                    hitEffectIndex =  2;
                    ballDamage     *= 2;
                }
                else if (successCount >= 2)
                {
                    hitEffectIndex =  2;
                }
                else
                {
                    hitEffectIndex = 1;
                }
                
                Debug.Log("Ball Damage : " + ballDamage);
            }
            else
            {
                int successCount = GameManager.Instance.enemyPlayer.GetComponent<CharacterStatus>()
                                              .justDodgeSuccessCounts;
                
                if (successCount >= 4)
                {
                    hitEffectIndex =  2;
                    ballDamage     *= 2;
                }
                else if (successCount >= 2)
                {
                    hitEffectIndex =  2;
                }
                else
                {
                    hitEffectIndex = 1;
                }
                
            }
        }

        public override float GetCalculatedBallHeight()
        {
            return base.GetCalculatedBallHeight();
        }

        public override float GetCalculatedBallSpeed()
        {
            float multiplyFactor = 1f;
            
            if (_ownerSpawnThisBall)
            {
                int successCount = GameManager.Instance.localPlayer.GetComponent<CharacterStatus>()
                                              .justDodgeSuccessCounts;
                
                multiplyFactor = 1 + Mathf.Lerp(0,1,successCount);
            }
            else
            {
                int successCount = GameManager.Instance.enemyPlayer.GetComponent<CharacterStatus>()
                                             .justDodgeSuccessCounts;
                
                multiplyFactor = 1 + Mathf.Lerp(0,1,successCount);
            }
            
            return base.GetCalculatedBallSpeed() * multiplyFactor;
        }
    }
}
