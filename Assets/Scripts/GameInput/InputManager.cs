using CharacterAttributes;
using Game;
using GameUI;
using SinglePlayer;
using Skill;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance = null;
        public static InputManager Instance => _instance == null ? null : _instance;

        public PlayerInput playerInput;

        private CharacterMovement _localCharacterMovement;
        private CharacterBallLauncher _characterBallLauncher;
        private CharacterSkillLauncher _characterSkillLauncher;
        private CharacterManager _characterManager;
        private CharacterController _characterController;
        private CharacterStatus _characterStatus;

        public LayerMask mouseColliderLayerMask = new LayerMask();

        public Vector3 currentTargetPosition;

        public void InitCallWhenLocalPlayerSpawned(GameObject localPlayerGameObject)
        {
            _localCharacterMovement = localPlayerGameObject.GetComponent<CharacterMovement>();
            _characterBallLauncher = localPlayerGameObject.GetComponent<CharacterBallLauncher>();
            _characterSkillLauncher = localPlayerGameObject.GetComponent<CharacterSkillLauncher>();
            _characterManager = localPlayerGameObject.GetComponent<CharacterManager>();
            _characterController = localPlayerGameObject.GetComponent<CharacterController>();
            _characterStatus = localPlayerGameObject.GetComponent<CharacterStatus>();

            playerInput.actions["Attack"].started += OnAttackPressed;
            playerInput.actions["Attack"].canceled += OnAttackReleased;

            playerInput.actions["Action"].started += OnActionPressed;
            playerInput.actions["Action"].canceled += OnActionReleased;

            Cursor.lockState = CursorLockMode.Locked;
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
        }

        private void OnDisable()
        {
            playerInput.actions["Attack"].started -= OnAttackPressed;
            playerInput.actions["Attack"].canceled -= OnAttackReleased;

            playerInput.actions["Action"].started -= OnActionPressed;
            playerInput.actions["Action"].canceled -= OnActionReleased;
        }

        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        public void OnMove(InputValue value)
        {
            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.isGameReadyToStart)
                MoveInput(value.Get<Vector2>());
            else
            {
                move = Vector2.zero;
            }
        }

        public void OnLook(InputValue value)
        {
            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.isGameReadyToStart)
                LookInput(value.Get<Vector2>());
            else
            {
                look = Vector2.zero;
            }
        }

        public void OnJump(InputValue value)
        {
            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.isGameReadyToStart)
                JumpInput(value.isPressed);
            else
            {
                jump = false;
            }
        }

        public void OnSprint(InputValue value)
        {
            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.isGameReadyToStart)
                SprintInput(value.isPressed);
            else
            {
                sprint = false;
            }
        }

        public bool canThrowBall = false;

        private bool _isAttackAlreadyPressed = false;

        public void OnAttackPressed(InputAction.CallbackContext ctx)
        {
            Debug.Log($"Canthrowball : {canThrowBall} and hitApproved : {_characterManager.hitApproved}");
            if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                if (SinglePlayerGM.Instance.IsGameReadyToStart && _characterManager.hitApproved &&
                   SinglePlayerGM.Instance.isPlayerTurn && canThrowBall && !_localCharacterMovement.shouldLockMovement && !_isAttackAlreadyPressed)
                    AttackPressed();
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
                if (GameManager.Instance.isGameReadyToStart && _characterManager.hitApproved &&
                     canThrowBall && !_localCharacterMovement.shouldLockMovement && !_isAttackAlreadyPressed)
                {
                    AttackPressed();
                }
        }

        private void AttackPressed()
        {
            _isAttackAlreadyPressed = true;
            _localCharacterMovement.LayerTransition(true);
            _characterBallLauncher.SpawnBallServerRPC();
        }

        public void OnAttackReleased(InputAction.CallbackContext ctx)
        {
            if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                if (SinglePlayerGM.Instance.IsGameReadyToStart && SinglePlayerGM.Instance.isPlayerTurn && _characterManager.hitApproved &&
                    canThrowBall && !_localCharacterMovement.shouldLockMovement && _isAttackAlreadyPressed)
                {
                    //TODO: Fill image 초기화
                    AttackReleased();
                }

            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
                if (GameManager.Instance.isGameReadyToStart &&
                    canThrowBall && _characterManager.hitApproved && !_localCharacterMovement.shouldLockMovement && _isAttackAlreadyPressed)
                {
                    UIManager.Instance.coolDownImage.fillAmount = 0f;
                    AttackReleased();
                }
        }

        private void AttackReleased()
        {
            _localCharacterMovement.SetThrowAnimation(true); // Animation 동작이 완료되면 throw = false 만드는 콜백 존재함

            _characterBallLauncher.ThrowBallServerRPC(currentTargetPosition);

            FixPlayerForwardDirection(0f);
            _characterManager.IncreaseThrowCount();

            if (_allowThrowAfterNSecCoroutine == null)
            {
                _allowThrowAfterNSecCoroutine = StartCoroutine(CoAllowThrowAfterNSec());
            }
        }

        public float throwCountDown = 0.8f;

        private Coroutine _allowThrowAfterNSecCoroutine = null;
        private IEnumerator CoAllowThrowAfterNSec()
        {
            float elapsedTime = 0f;
            canThrowBall = false;

            while (elapsedTime < throwCountDown)
            {
                elapsedTime += Time.deltaTime;
                float fillAmount = elapsedTime / throwCountDown;

                if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
                {
                    UIManager.Instance.coolDownImage.fillAmount = fillAmount;
                }
                else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
                {

                }
                yield return null;
            }

            if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
            {
                UIManager.Instance.coolDownImage.fillAmount = 1f;
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {

            }

            canThrowBall = true;

            _allowThrowAfterNSecCoroutine = null;

            _isAttackAlreadyPressed = false;
        }

        public void OnActionPressed(InputAction.CallbackContext ctx)
        {
            if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                if (SinglePlayerGM.Instance.IsGameReadyToStart && !_localCharacterMovement.shouldLockMovement && _characterSkillLauncher.canUseSkill)
                    ActionPressed();
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
                if (GameManager.Instance.isGameReadyToStart && !_localCharacterMovement.shouldLockMovement && _characterSkillLauncher.canUseSkill)
                {
                    ActionPressed();
                }
        }

        private void ActionPressed()
        {
            if (_characterController == null)
            {
                Debug.LogError("CharacterController is missing on this GameObject!");
                return;
            }

            _characterSkillLauncher.StartSkillCoolDown();

            switch (_characterSkillLauncher.currentSkill.ThisSkillType)
            {
                case ESkillInputType.Vector3Target:
                    {
                        Vector3 currentDirection = _localCharacterMovement.transform.TransformDirection(new Vector3(move.normalized.x, 0f, move.normalized.y));

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
                        var input = new PressInput
                        {
                            IsPressed = true
                        };
                        _characterSkillLauncher.StartSkill(input);
                        break;
                    }
                case ESkillInputType.RayVec3Target:
                    {
                        var input = new TargetVector3Input
                        {
                            TargetVector = currentTargetPosition
                        };
                        _characterSkillLauncher.StartSkill(input);
                        break;
                    }
            }
        }

        public void OnActionReleased(InputAction.CallbackContext ctx)
        {
            if (GameMode.Instance.CurrentGameMode == EGameMode.SINGLEPLAYER)
            {
                if (SinglePlayerGM.Instance.IsGameReadyToStart && !_localCharacterMovement.shouldLockMovement && _characterSkillLauncher.canUseSkill)
                    ActionReleased();
            }
            else if (GameMode.Instance.CurrentGameMode == EGameMode.MULTIPLAER)
                if (GameManager.Instance.isGameReadyToStart && !_localCharacterMovement.shouldLockMovement && _characterSkillLauncher.canUseSkill)
                {
                    ActionReleased();
                }
        }

        private void ActionReleased()
        {

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
            Vector3 aimDirection = (worldAimTarget - _localCharacterMovement.transform.position).normalized;

            _localCharacterMovement.transform.forward = Vector3.Lerp(_localCharacterMovement.transform.forward, aimDirection, Time.deltaTime * lerpFactor);
        }

        private void Update()
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if (GameManager.Instance.isGameReadyToStart)
                if (Physics.Raycast(ray, out RaycastHit hit, 999f, mouseColliderLayerMask))
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
