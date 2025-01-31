using UnityEngine;
using UnityEngine.InputSystem;

public class BindInput : MonoBehaviour
{
    protected CharacterController _characterController;
    protected PlayerInput _playerInput;

    [SerializeField]
    protected Vector3 _moveInput;
    [SerializeField]
    protected Quaternion _direction;

    bool _up;
    bool _down;
    bool _left;
    bool _right;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
    }
    protected void Start()
    {
        _moveInput = Vector3.zero;
        _direction = Quaternion.identity;

        // 이렇게 하는게 맞나?
        _playerInput.actions["Up"].performed += OnUpPressed;
        _playerInput.actions["Up"].canceled += OnUpReleased;
        _playerInput.actions["Left"].performed += OnLeftPressed;
        _playerInput.actions["Left"].canceled += OnLeftReleased;
        _playerInput.actions["Down"].performed += OnDownPressed;
        _playerInput.actions["Down"].canceled += OnDownReleased;
        _playerInput.actions["Right"].performed += OnRightPressed;
        _playerInput.actions["Right"].canceled += OnRightReleased;

        _playerInput.actions["Look"].performed += OnLookPressed;
        _playerInput.actions["Look"].canceled += OnLookReleased;

        _playerInput.actions["Jump"].performed += OnJumpPressed;
        _playerInput.actions["Jump"].canceled += OnJumpReleased;

        _playerInput.actions["Sprint"].performed += OnSprintPressed;
        _playerInput.actions["Sprint"].canceled += OnSprintReleased;

        _playerInput.actions["Attack"].performed += OnAttackPressed;
        _playerInput.actions["Attack"].canceled += OnAttackReleased;

        _playerInput.actions["Action"].performed += OnActionPressed;
        _playerInput.actions["Action"].canceled += OnActionReleased;
    }

    private void OnRightReleased(InputAction.CallbackContext ctx)
    {
        _right = false;
        _moveInput.x = _left ? -1 : 0;
    }
    private void OnRightPressed(InputAction.CallbackContext ctx)
    {
        _right = true;
        _moveInput.x = ctx.ReadValue<float>();
    }
    private void OnDownReleased(InputAction.CallbackContext ctx)
    {
        _down = false;
        _moveInput.z = _up ? 1 : 0;
    }
    private void OnDownPressed(InputAction.CallbackContext ctx)
    {
        _down = true;
        _moveInput.z = -ctx.ReadValue<float>();
    }
    private void OnLeftPressed(InputAction.CallbackContext ctx)
    {
        _left = true;
        _moveInput.x = -ctx.ReadValue<float>();
    }
    private void OnLeftReleased(InputAction.CallbackContext ctx)
    {
        _left = false;
        _moveInput.x = _right ? 1 : 0;
    }
    private void OnUpReleased(InputAction.CallbackContext ctx)
    {
        _up = false;
        _moveInput.z = _down ? -1 : 0;
    }
    private void OnUpPressed(InputAction.CallbackContext ctx)
    {
        _up = true;
        _moveInput.z = ctx.ReadValue<float>();
    }
    public virtual void OnSprintPressed(InputAction.CallbackContext ctx)
    {

    }
    public virtual void OnSprintReleased(InputAction.CallbackContext ctx)
    {

    }
    public virtual void OnJumpPressed(InputAction.CallbackContext ctx)
    {

    }
    public virtual void OnJumpReleased(InputAction.CallbackContext ctx)
    {

    }
    public virtual void OnLookPressed(InputAction.CallbackContext ctx)
    {
        var delta = ctx.ReadValue<Vector2>();
        var rotationChange = Quaternion.Euler(delta.y, delta.x, 0);
        _direction *= rotationChange;
    }
    public virtual void OnLookReleased(InputAction.CallbackContext ctx)
    {

    }
    public virtual void OnAttackPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log($"OnAttackPressed {ctx.ReadValue<float>()}");
    }
    public virtual void OnAttackReleased(InputAction.CallbackContext ctx)
    {
        Debug.Log($"OnAttackReleased {ctx.ReadValue<float>()}");
    }
    public virtual void OnActionPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log($"OnActionPressed {ctx.ReadValue<float>()}");
    }
    public virtual void OnActionReleased(InputAction.CallbackContext ctx)
    {
        Debug.Log($"OnActionReleased {ctx.ReadValue<float>()}");
    }
}
