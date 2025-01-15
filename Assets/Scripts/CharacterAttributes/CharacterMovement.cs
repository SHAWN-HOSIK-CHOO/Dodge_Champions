using System;
using Game;
using GameInput;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CharacterAttributes
{
    public class CharacterMovement : NetworkBehaviour
    {
        public CharacterController characterController;
        
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.33f;

        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        public AudioClip landingAudioClip;
        public AudioClip[] footstepAudioClips;
        [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 23f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = 10f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition = false;

        [Header("Animation blend tree")] 
        [Range(0, 1)]
        public float walkAnimationTargetValue = 0.3f;
        [Range(0, 1)] 
        public float runAnimationTargetValue = 1f;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        
        // player
        private float _speed;
        private float _animationBlend;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        
        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDThrow;

        private int _animIDXVelocity;
        private int _animIDZVelocity;
        
        private float xVelocity = 0f;
        private float zVelocity = 0f;
        
        //animation layer transit
        public int   targetLayerIndex = 1;  // A 레이어의 인덱스 (Base Layer는 0)
        public float transitionSpeed  = 1f; // 전환 속도

        private NetworkVariable<float> _upperLayerWeight        = new NetworkVariable<float>(0.0f);
        private bool                   _isTransitioningToLayer = true; // A 레이어로 전환 여부

        private PlayerInput _playerInput;
        private Animator    _animator;
        private GameObject  _mainCamera;

        private const float Threshold = 0.01f;

        private bool _hasAnimator;
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle  -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                if (_mainCamera == null)
                {
                    _mainCamera = GameManager.Instance.mainCamera;
                }

                GameManager.Instance.cinemachineCamera.GetComponent<CinemachineCamera>().Follow =
                    cinemachineCameraTarget.transform;
                GameManager.Instance.cinemachineCamera.transform.rotation = Quaternion.Euler(20f,0f,0f);
            }
        }

        private void Start()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _hasAnimator          = TryGetComponent(out _animator);
            characterController  = GetComponent<CharacterController>();
            
            AssignAnimationIDs();
            
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;

            _upperLayerWeight.OnValueChanged += OnUpperLayerWeightChanged;
        }

        public override void OnDestroy()
        {
            _upperLayerWeight.OnValueChanged -= OnUpperLayerWeightChanged;
        }

        private void Update()
        {
            if (!IsOwner || !GameManager.Instance.isGameReadyToStart)
            {
                return;
            }
            
            _hasAnimator = TryGetComponent(out _animator);
            
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            if (!IsOwner || !GameManager.Instance.isGameReadyToStart)
            {
                return;
            }
            
            CameraRotation();
        }
        
        private void AssignAnimationIDs()
        {
            _animIDSpeed       = Animator.StringToHash("Speed");
            _animIDGrounded    = Animator.StringToHash("Grounded");
            _animIDJump        = Animator.StringToHash("Jump");
            _animIDFreeFall    = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDThrow       = Animator.StringToHash("Throw");
            _animIDXVelocity = Animator.StringToHash("X_Velocity");
            _animIDZVelocity = Animator.StringToHash("Z_Velocity");
        }
        
        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                                                 transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                                           QueryTriggerInteraction.Ignore);

            // 디버깅용 구체 그리기
            Debug.DrawLine(transform.position, spherePosition, grounded ? Color.green : Color.red);
            
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded,grounded);
            }
        }

        private void CameraRotation()
        {
            if (InputManager.Instance.look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                _cinemachineTargetYaw   += InputManager.Instance.look.x;
                _cinemachineTargetPitch += InputManager.Instance.look.y;
            }
            
            _cinemachineTargetYaw   = ClampAngle(_cinemachineTargetYaw,   float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp,    topClamp);
            
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                                                                          _cinemachineTargetYaw, 0.0f);
        }
        
        private void Move()
        {
            // 입력이 없다면 속도를 0으로 설정
            float targetSpeed = InputManager.Instance.sprint ? sprintSpeed : moveSpeed;
            float animationTargetValue =
                InputManager.Instance.sprint ? runAnimationTargetValue : walkAnimationTargetValue;
            
            if (InputManager.Instance.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }
        
            // 현재 속도 계산
            float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
            float speedOffset = 0.1f;
        
            // 속도를 부드럽게 보간
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            
        
            // 입력 방향 계산
            Vector3 inputDirection = new Vector3(InputManager.Instance.move.x, 0.0f, InputManager.Instance.move.y).normalized;
            
            // x, z 속도 계산 (블렌딩 추가)
            float targetXVelocity = 0f;
            float targetZVelocity = 0f;
        
            if (InputManager.Instance.move != Vector2.zero)
            {
                // 메인 카메라를 기준으로 이동 방향 설정
                Vector3 cameraForward = _mainCamera.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();
        
                Vector3 cameraRight = _mainCamera.transform.right;
                cameraRight.y = 0;
                cameraRight.Normalize();
        
                Vector3 moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
        
                // 이동 처리
                characterController.Move(moveDirection.normalized                   * (_speed * Time.deltaTime) +
                                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        
                // 목표 속도 설정
                targetXVelocity = Vector3.Dot(moveDirection.normalized, cameraRight) * animationTargetValue;
                targetZVelocity = Vector3.Dot(moveDirection.normalized, cameraForward) * animationTargetValue;
            }
            else
            {
                // 입력이 없을 때 수직 이동만 적용
                characterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
        
            // xVelocity와 zVelocity를 부드럽게 변화
            xVelocity = Mathf.Lerp(xVelocity, targetXVelocity, Time.deltaTime * speedChangeRate);
            zVelocity = Mathf.Lerp(zVelocity, targetZVelocity, Time.deltaTime * speedChangeRate);
            
            // 애니메이션 업데이트
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDXVelocity, xVelocity); // 좌/우 (-1: 왼쪽, 1: 오른쪽)
                _animator.SetFloat(_animIDZVelocity, zVelocity); // 앞/뒤 (-1: 뒤로, 1: 앞으로)
                _animator.SetFloat(_animIDMotionSpeed, 1);
            }
        
            // 플레이어의 회전을 화면 중심으로 고정
            transform.rotation = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f);
        }
        
        private void JumpAndGravity()
        {
            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout;
                
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump,     false);
                    _animator.SetBool(_animIDFreeFall, false);
                }
                
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (InputManager.Instance.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }
                
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                InputManager.Instance.jump = false;
            }
            
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        public void LayerTransition(bool is0To1)
        {
            if (!IsOwner)
            {
                return;
            }
            
            _isTransitioningToLayer = is0To1;
            float targetWeight = _isTransitioningToLayer ? 1f : 0f;
            
            WriteNetworkVariableLayerWeightServerRPC(targetWeight);
            
            _animator.SetLayerWeight(targetLayerIndex, _upperLayerWeight.Value);
        }

        [ServerRpc]
        private void WriteNetworkVariableLayerWeightServerRPC(float newValue)
        {
            _upperLayerWeight.Value = newValue;
        }
        
        private void OnUpperLayerWeightChanged(float oldWeight, float newWeight)
        {
            _animator.SetLayerWeight(1, newWeight); 
        }

        public void Callback_Layer1To0()
        {
            LayerTransition(false);
        }

        public void SetThrowAnimation(bool val)
        {
            _animator.SetBool(_animIDThrow, val);
        }

        public void Callback_SetThrowFalse()
        {
            _animator.SetBool(_animIDThrow,false);
        }
        
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed   = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (grounded) Gizmos.color = transparentGreen;
            else Gizmos.color          = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                              new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                              groundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(characterController.center), footstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(characterController.center), footstepAudioVolume);
            }
        }
        
    }
}
