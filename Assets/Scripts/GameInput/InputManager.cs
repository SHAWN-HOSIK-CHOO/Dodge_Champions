using System;
using CharacterAttributes;
using Game;
using Skill;
using Tests;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance = null;
        public  static InputManager Instance => _instance == null ? null : _instance;

        public PlayerInput playerInput;

        private CharacterMovement      _localCharacterMovement;
        private CharacterBallLauncher  _characterBallLauncher;
        private CharacterSkillLauncher _characterSkillLauncher;
        private CharacterManager       _characterManager;
        private CharacterController    _characterController;
        private CharacterStatus        _characterStatus;
        
        public Transform localPlayerBallSpawnTransform;
        public LayerMask mouseColliderLayerMask = new LayerMask();
      
        public Vector3 currentTargetPosition;

        public float ballLaunchSpeedBase = 10f;

        //public Transform debugTransform;

        public void InitCallWhenLocalPlayerSpawned()
        {
            _localCharacterMovement       = GameManager.Instance.localPlayer.GetComponent<CharacterMovement>();
            _characterBallLauncher        = GameManager.Instance.localPlayer.GetComponent<CharacterBallLauncher>();
            _characterSkillLauncher       = GameManager.Instance.localPlayer.GetComponent<CharacterSkillLauncher>();
            _characterManager             = GameManager.Instance.localPlayer.GetComponent<CharacterManager>();
            _characterController          = GameManager.Instance.localPlayer.GetComponent<CharacterController>();
            _characterStatus              = GameManager.Instance.localPlayer.GetComponent<CharacterStatus>();
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

            playerInput.actions["Action"].started  += OnActionPressed;
            playerInput.actions["Action"].canceled += OnActionReleased;

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

        public  bool canThrowBall       = false;

        public void RequestTurnSwapToEnemy()
        {
            _characterManager.RequestTurnSwapServerRPC();
        }
        
        private void OnAttackPressed(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance.isGameReadyToStart && GameManager.Instance.isLocalPlayerAttackTurn && canThrowBall)
            {
                _localCharacterMovement.LayerTransition(true);
                _characterBallLauncher.SpawnBallServerRPC();
            }
        }

        private void OnAttackReleased(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance.isGameReadyToStart && GameManager.Instance.isLocalPlayerAttackTurn && canThrowBall)
            {
                _localCharacterMovement.SetThrowAnimation(true); // Animation 동작이 완료되면 throw = false 만드는 콜백 존재함
                
                _characterBallLauncher.ThrowBallServerRPC(currentTargetPosition);
                
                FixPlayerForwardDirection(0f);
                _characterManager.IncreaseThrowCount();
                canThrowBall = false;
            }
        }

        private void OnActionPressed(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance.isGameReadyToStart)
            {
                //Debug.Log("Skill Button Pressed");
                if (_characterController == null)
                {
                    Debug.LogError("CharacterController is missing on this GameObject!");
                    return;
                }

                switch (_characterSkillLauncher.currentSkill.ThisSkillType)
                {
                    case ESkillInputType.Vector3Target:
                    {
                        Vector3 currentDirection = _characterController.velocity.normalized;

                        if (currentDirection == Vector3.zero)
                        {
                            currentDirection = -_localCharacterMovement.transform.forward;
                        }

                        var input = new TargetVector3Input
                                    {
                                        TargetVector = currentDirection
                                    };
                        
                        _characterSkillLauncher.StartSkill(input);
                        break;
                    }
                    case ESkillInputType.Scalar3Value:
                    {
                        break;
                    }
                    case ESkillInputType.JustBoolean:
                    {
                        break;
                    }
                }
                
            }
        }

        private void OnActionReleased(InputAction.CallbackContext ctx)
        {
            if (GameManager.Instance.isGameReadyToStart)
            {
                //Debug.Log("Skill Button Released");
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

                //debugTransform.transform.position = hit.point;
                FixPlayerForwardDirection(25f);
            }

            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     Cursor.lockState = CursorLockMode.None;
            // }
        }
    }
}
