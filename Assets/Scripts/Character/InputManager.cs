using System;
using Ball;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Character
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance = null;
        public  static InputManager Instance => _instance == null ? null : _instance;

        public PlayerInput playerInput;

        private CharacterMovement _localCharacterMovement;
        private BallLauncher      _ballLauncher;
        private CharacterManager  _characterManager;
        
        public Transform localPlayerBallSpawnTransform;
        public LayerMask mouseColliderLayerMask = new LayerMask();
      
        public Vector3 currentTargetPosition;

        public void InitCallWhenLocalPlayerSpawned()
        {
            _localCharacterMovement       = GameManager.Instance.localPlayer.GetComponent<CharacterMovement>();
            _ballLauncher                 = GameManager.Instance.localPlayer.GetComponent<BallLauncher>();
            _characterManager             = GameManager.Instance.localPlayer.GetComponent<CharacterManager>();
            localPlayerBallSpawnTransform = GameManager.Instance.localPlayerBallSpawnPosition;
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            playerInput.actions["Attack"].started  += OnAttackPressed;
            playerInput.actions["Attack"].canceled += OnAttackReleased;

            Cursor.lockState = CursorLockMode.Locked;
        }

        public Vector2 move;
        public Vector2 look;
        public bool    jump;
        public bool    sprint;

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        private void OnAttackPressed(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance.isGameReadyToStart)
            {
                _localCharacterMovement.LayerTransition(true);
                _ballLauncher.SpawnBallServerRPC();
            }
        }

        private void OnAttackReleased(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance.isGameReadyToStart)
            {
                _localCharacterMovement.SetThrowAnimation(true); // Animation 동작이 완료되면 throw = false 만드는 콜백 존재함
                _ballLauncher.ThrowBallServerRPC(currentTargetPosition, 10f, 1.3f);
                
                FixPlayerForwardDirection(50f);
            }
        }

        private void MoveInput(Vector2 newMovementVector)
        {
            move = newMovementVector;
        }

        private void LookInput(Vector2 newLookVector)
        {
            look = newLookVector;
        }

        private void JumpInput(bool newJumpInput)
        {
            jump = newJumpInput;
        }

        private void SprintInput(bool newSprintInput)
        {
            sprint = newSprintInput;
        }

        private void FixPlayerForwardDirection(float lerpFactor)
        {
            if (_localCharacterMovement == null)
            {
                return;
            }
            
            Vector3 worldAimTarget = currentTargetPosition;
            worldAimTarget.y = _localCharacterMovement.transform.position.y;
            Vector3 aimDirection = ( worldAimTarget - _localCharacterMovement.transform.position ).normalized;
                
            _localCharacterMovement.transform.forward = Vector3.Lerp(_localCharacterMovement.transform.forward, aimDirection, Time.deltaTime * lerpFactor);
        }

        private void Update()
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray     ray               = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray,out RaycastHit hit, 999f, mouseColliderLayerMask))
            {
                currentTargetPosition = hit.point;

                FixPlayerForwardDirection(25f);
            }
        }
    }
}
