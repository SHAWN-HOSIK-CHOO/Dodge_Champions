using UnityEngine;

namespace Ball
{
    public class InfiniteBallScript : BallScript
    {
        public override void Initialize(Vector3 target, float speed, float height, bool horizontalThrow = false, bool isThisOwner = true)
        {
            base.Initialize(target, speed, height, horizontalThrow, isThisOwner);
            isInfinite      = true;
            ballDamage      = 8;
            ballLaunchSpeed = 50;
            hitEffectIndex  = 5;
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
