using UnityEngine;

namespace Ball
{
    public class HardBallScript : BallScript
    {
        public override void Initialize(Vector3 target, float speed, float height, bool horizontalThrow = false, bool isThisOwner = true)
        {
            base.Initialize(target, speed, height, horizontalThrow, isThisOwner);
            hitEffectIndex = 6;
            ballDamage     = 8;
        }

        public override float GetCalculatedBallHeight()
        {
            return base.GetCalculatedBallHeight();
        }

        public override float GetCalculatedBallSpeed()
        {
            return base.GetCalculatedBallSpeed();
        }
    }
}
