using Codice.CM.Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MoveControl : MonoBehaviour
{
    CharacterController _characterController;
    [Header("Set Params")]
    [SerializeField]
    Vector3 _speed;
    [Tooltip("The radius of the grounded check")]
    float _groundedRadius = 0.28f;
    [SerializeField]
    bool _useGravity;
    [SerializeField]
    [Tooltip("What layers the character uses as ground")]
    LayerMask _groundLayers;
    [SerializeField]
    [Tooltip("The height the player can jump")]
    float _jumpHeight = 1.2f;
    [SerializeField]
    [Tooltip("Handle speedChangeRate")]
    float _moveAccel = 10.0f;
    [SerializeField]
    [Tooltip("Handle speedChangeRate on Stop")]
    float _stopAccel = 10.0f;
    float _gravityAcceleration = -9.8f;
    [SerializeField]
    [Tooltip("Mouse Speed multiplyer")]
    float _mouseMultiplyer;
    [SerializeField]
    [Tooltip("Pitch Range")]
    Vector2 _pitchRange;

    [SerializeField]
    string _bindingMapName;
    protected bool _onGround;
    InputBinding _inputBinding;
    DefaultBinding _defaultBinding;
    JumpBinding _jumpBinding;

    #region Debug
    [Header("For Debugging"), Space(20)]
    [SerializeField]
    protected bool _Jump;
    [SerializeField]
    Vector3 _moveflag;
    [SerializeField]
    Vector2 _mouseflag;
    [SerializeField]
    float _pitch;
    [SerializeField]
    float _yaw;
    #endregion

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputBinding = GetComponent<InputBinding>();
    }
    protected void Start()
    {
        BindInput();
    }

    void BindInput()
    {
        _defaultBinding = new DefaultBinding(_inputBinding, _bindingMapName);
        _jumpBinding = new JumpBinding(_inputBinding, _bindingMapName);
        _inputBinding.EnableMap(_bindingMapName);
        _defaultBinding._onMoveInputChanged += OnMoveInputChange;
        _defaultBinding._onMouseInputChanged += OnMouseInputChanged;
        _jumpBinding._onJumpInputChanged += OnJumpInputChanged;
    }


    private void Update()
    {
        MoveUpdate(Time.deltaTime);
    }

    void OnJumpInputChanged(float val)
    {
        if(val == 1)
        {
            if(_onGround)
            {
                _Jump = true;
            }
        }
    }
    void OnMoveInputChange(Vector3 val)
    {
        _moveflag = val;
    }
    void OnMouseInputChanged(Vector2 val)
    {
        _mouseflag = val * _mouseMultiplyer;
        _pitch -= _mouseflag.y;
        _yaw += _mouseflag.x;
        _pitch = Mathf.Clamp(_pitch, _pitchRange.x, _pitchRange.y);
        _pitch = _pitch % 360;
        _yaw = _yaw % 360;
    }
    private void OnDestroy()
    {
        _defaultBinding._onMoveInputChanged -= OnMoveInputChange;
        _defaultBinding._onMouseInputChanged -= OnMouseInputChanged;
        _jumpBinding._onJumpInputChanged -= OnJumpInputChanged;
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(((1 << hit.gameObject.layer) & _groundLayers)!= 0)
        {
            _onGround = true;
            Debug.Log("On Ground");
        }
    }
    void MoveUpdate(float deltaTime)
    {
        if (_useGravity)
        {
            _speed.y +=  _gravityAcceleration * deltaTime;
            if (_onGround )
            {
                _speed.y = Mathf.Clamp(_speed.y, -3, 0);
                if (_Jump)
                {
                    _speed.y = Mathf.Sqrt(_jumpHeight * -2f * _gravityAcceleration);
                    _Jump = false;
                }
            }
        }
        _onGround = false;
        var moveflag = _moveflag;
        moveflag.y = 1;
        Quaternion pitchRotation = Quaternion.AngleAxis(_pitch, Vector3.right);
        Quaternion yawRotation = Quaternion.AngleAxis(_yaw, Vector3.up);
        var finalSpeed = yawRotation * AccelCorrection(Vector3.Scale(moveflag, _speed), deltaTime);
        var vector = finalSpeed * deltaTime;
        _characterController.Move(vector);
        transform.rotation = yawRotation * pitchRotation;
    }
    Vector3 AccelCorrection(Vector3 speed, float deltatime)
    {
        Quaternion yawRotation = Quaternion.AngleAxis(-_yaw, Vector3.up);
        Vector3 charactercontrollerSpeed = yawRotation * _characterController.velocity;
        Vector3 correctedSpeed = speed;
        if (charactercontrollerSpeed.x > speed.x)
        {
            float speedDelta = (speed.x == 0) ? _stopAccel : _moveAccel;
            correctedSpeed.x = charactercontrollerSpeed.x - speedDelta * deltatime;
            correctedSpeed.x = Mathf.Clamp(correctedSpeed.x, speed.x, charactercontrollerSpeed.x);
        }
        else if (charactercontrollerSpeed.x < speed.x)
        {
            float speedDelta = (speed.x == 0) ? _stopAccel : _moveAccel;
            correctedSpeed.x = charactercontrollerSpeed.x + speedDelta * deltatime;
            correctedSpeed.x = Mathf.Clamp(correctedSpeed.x, charactercontrollerSpeed.x, speed.x);
        }

        if (charactercontrollerSpeed.z > speed.z)
        {
            float speedDelta = (speed.z == 0) ? _stopAccel : _moveAccel;
            correctedSpeed.z = charactercontrollerSpeed.z - speedDelta * deltatime;
            correctedSpeed.z = Mathf.Clamp(correctedSpeed.z, speed.z, charactercontrollerSpeed.z);
        }
        else if (charactercontrollerSpeed.z < speed.z)
        {
            float speedDelta = (speed.z == 0) ? _stopAccel : _moveAccel;
            correctedSpeed.z = charactercontrollerSpeed.z + speedDelta * deltatime;
            correctedSpeed.z = Mathf.Clamp(correctedSpeed.z, charactercontrollerSpeed.z, speed.z);
        }
        return correctedSpeed;
    }
    private void OnDrawGizmos()
    {
        if (_characterController != null)
        {
            DrawGroundSphere(CheckGround());
        }
    }
    void DrawGroundSphere(bool isGrounded)
    {
        var spherePosition = _characterController.bounds.center - new Vector3(0, _characterController.bounds.extents.y, 0);
        Gizmos.color = _characterController.isGrounded ? Color.red : Color.green;
        Gizmos.DrawSphere(spherePosition, _groundedRadius);
    }
    bool CheckGround()
    {
        if (_characterController.isGrounded)
        {
            var spherePosition = _characterController.bounds.center - new Vector3(0, _characterController.bounds.extents.y, 0);
            bool grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
            return grounded;
        }
        return false;
    }
}
