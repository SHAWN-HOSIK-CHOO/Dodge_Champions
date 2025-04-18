using Game;
using System.Collections;
using UnityEngine;

namespace Ball
{
    public class BallScript : MonoBehaviour
    {
        protected bool _canStart = false;

        protected Vector3 _startPosition;
        protected Vector3 _targetPosition;
        protected float _timeElapsed;
        protected bool _isInitialized;
        protected bool _ownerSpawnThisBall;

        private Rigidbody rb;

        public int ballDamage = 10;
        public int hitEffectIndex = 0;

        public float ballLaunchSpeed = 42f;
        public float ballHeight = 0.3f;
        public bool isInfinite = false;

        [Header("Curve mode")] public bool isHorizontalThrow = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        public void SetBallDamage(int dmg)
        {
            ballDamage = dmg;
        }

        public virtual float GetCalculatedBallSpeed()
        {
            return ballLaunchSpeed;
        }

        public virtual float GetCalculatedBallHeight()
        {
            return ballHeight;
        }

        public virtual void Initialize(Vector3 target, float speed, float height,
             bool horizontalThrow = false, bool isThisOwner = true)
        {
            _startPosition = Vector3.zero;
            _targetPosition = target;
            ballLaunchSpeed = speed;
            ballHeight = height;
            _timeElapsed = 0f;
            isHorizontalThrow = horizontalThrow;
            _isInitialized = true;
            _canStart = false;
            _ownerSpawnThisBall = isThisOwner;
        }

        public void StartCommand(Vector3 startPosition)
        {
            if (_isInitialized)
            {
                _canStart = true;
                _startPosition = startPosition;

                if (startPosition == _targetPosition)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        void Update()
        {
            if (_isInitialized && _canStart)
            {
                BallUpdate();
            }
        }

        protected virtual void BallUpdate()
        {
            _timeElapsed += Time.deltaTime;
            float travelDuration = Vector3.Distance(_startPosition, _targetPosition) / ballLaunchSpeed;
            float t = _timeElapsed / travelDuration;

            transform.position = CalculateParabolicPosition(_startPosition, _targetPosition, ballHeight, t, isHorizontalThrow);
        }

        Vector3 CalculateParabolicPosition(Vector3 start, Vector3 target, float maxHeight, float t, bool horizontal)
        {
            // 수평 또는 수직 궤적에 따라 Lerp 방향 변경
            Vector3 horizontalPosition = Vector3.Lerp(start, target, t);
            float parabolicHeight = 4 * maxHeight * t * (1 - t);

            if (horizontal)
            {
                // 수평 궤적: Z 높이가 아닌 X를 궤적에 포함
                return new Vector3(start.x + parabolicHeight, horizontalPosition.y, horizontalPosition.z);
            }
            else
            {
                // 기본 수직 궤적
                return new Vector3(horizontalPosition.x, start.y + parabolicHeight, horizontalPosition.z);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Floor") || other.CompareTag("Wall"))
            {
                //상대방이 던진 공이 나를 맞추지 못하고, 내가 Just Dodge를 한 것도 아니며, 벽과 땅에 닿았다면
                if (this.CompareTag("Fake"))
                {
                    //상대방에게 턴을 넘기라고 RPC로 지시한다.
                    //TODO:턴제 제거에 따른 조치 필요
                    //InputManager.Instance.RequestTurnSwapToEnemy();
                }

                if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
                {
                    //싱글플레이 모드에서는 플레이어 위주로 회피, 피격, 적중 판단이 진행된다
                    if (this.CompareTag("Real"))
                    {
                        //InputManager.Instance.RequestTurnSwapToEnemy();
                    }
                }

                // 태그 변경
                this.gameObject.tag = "Useless";
                _canStart = false;

                // 충돌 법선 계산: 충돌체의 중심 기준
                Vector3 collisionNormal = (transform.position - other.bounds.center).normalized;

                // 기존 이동 방향 (입사 벡터)
                Vector3 incomingVelocity = (_targetPosition - _startPosition).normalized * ballLaunchSpeed;

                // 반사 벡터 계산
                Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, collisionNormal);

                // 튕기기 시작
                StartBounce(reflectedVelocity);
            }
        }

        private void StartBounce(Vector3 initialVelocity)
        {
            StartCoroutine(BounceEffect(initialVelocity));
        }

        private IEnumerator BounceEffect(Vector3 velocity)
        {
            float duration = 1f; // 튕기는 효과 지속 시간
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // 중력 효과 추가
                velocity += Physics.gravity * Time.deltaTime;

                // 공의 위치 갱신
                transform.position += velocity * Time.deltaTime;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(this.gameObject);

        }


    }
}
