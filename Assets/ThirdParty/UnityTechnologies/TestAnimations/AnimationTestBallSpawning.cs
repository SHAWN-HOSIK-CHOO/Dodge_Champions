using UnityEngine;
using System.Collections;

namespace ThirdParty.UnityTechnologies.TestAnimations
{
    public class AnimationTestBallSpawning : MonoBehaviour
    {
        public  GameObject pfBall;
        public  Transform  ballSpawnPos;
        private GameObject _ball;

        public void CallBack_Throw3Animation()
        {
            _ball = Instantiate(pfBall, ballSpawnPos.position, ballSpawnPos.rotation);
            _ball.transform.SetParent(ballSpawnPos);
            StartCoroutine(CoDestroyBallAfter1Sec());
        }

        IEnumerator CoDestroyBallAfter1Sec()
        {
            yield return new WaitForSeconds(1.0f);
            ballSpawnPos.DetachChildren();
            Destroy(_ball.gameObject);
        }
    }
}
