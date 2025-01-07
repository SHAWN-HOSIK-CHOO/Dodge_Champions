using System;
using System.Collections;
using CharacterAttributes;
using Game;
using UnityEngine;

namespace Ball
{
    public class BallScript : MonoBehaviour
    {
        private bool _canStart = false;
        
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _throwSpeed;
        private float _maxHeight;
        private float _timeElapsed;
        private bool _isInitialized;

        private Rigidbody rb;

        [Header("Curve mode")] public bool isHorizontalThrow = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        public void Initialize(Vector3 target, float speed, float height, 
             bool horizontalThrow = false)
        {
            _startPosition    = Vector3.zero;
            _targetPosition   = target;
            _throwSpeed       = speed;
            _maxHeight        = height;
            _timeElapsed      = 0f;
            isHorizontalThrow = horizontalThrow;
            _isInitialized    = true;
            _canStart         = false;
        }

        public void StartCommand(Vector3 startPosition)
        {
            if (_isInitialized)
            {
                _canStart      = true;
                _startPosition = startPosition;
            }
        }
        
        void Update()
        {
            if (_isInitialized && _canStart)
            {
                _timeElapsed += Time.deltaTime;
                float travelDuration = Vector3.Distance(_startPosition, _targetPosition) / _throwSpeed;
                float t = _timeElapsed / travelDuration;
                
                transform.position = CalculateParabolicPosition(_startPosition, _targetPosition, _maxHeight, t, isHorizontalThrow);
            }
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
                if(this.CompareTag("Fake"))
                    GameManager.Instance.SwapTurnServerRPC();
                
                // 태그 변경
                this.gameObject.tag = "Useless";
                _canStart           = false;

                // 충돌 법선 계산: 충돌체의 중심 기준
                Vector3 collisionNormal = (transform.position - other.bounds.center).normalized;

                // 기존 이동 방향 (입사 벡터)
                Vector3 incomingVelocity = (_targetPosition - _startPosition).normalized * _throwSpeed;

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
            float duration    = 1f; // 튕기는 효과 지속 시간
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
