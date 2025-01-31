using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MoveControl : BindInput
{
    protected bool _Jump;
    protected bool _onGround;

    [SerializeField]
    Vector3 _speed;
    [Tooltip("The radius of the grounded check")]
    float _groundedRadius = 0.28f;
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

    protected void Awake()
    {
        base.Awake();
    }
    protected void Start()
    {
        base.Start();
        _direction = Quaternion.identity;

    }
    private void FixedUpdate()
    {
        if (_onGround = CheckGround())
        {
            _speed.y = 0;
            if (_Jump)
            {
                _speed.y +=  Mathf.Sqrt(_jumpHeight * -2f * _gravityAcceleration);
                _Jump = false;
            }
        }
        else
        {
            // Gravity
            _speed.y += _gravityAcceleration * Time.fixedDeltaTime;
        }
        var moveflag = _moveInput; 
        if(_speed.y != 0 ) moveflag.y = 1;
        Quaternion yRotation = Quaternion.Euler(0, _direction.eulerAngles.y, 0);
        var finalSpeed = yRotation* AccelCorrection(Vector3.Scale(moveflag, _speed),Time.fixedDeltaTime);
        var vector = finalSpeed * Time.fixedDeltaTime;
        _characterController.Move(vector);
        transform.forward = _direction * Vector3.forward * Time.fixedDeltaTime;
    }
    Vector3 AccelCorrection(Vector3 speed,float deltatime)
    {
        Quaternion yRotation = Quaternion.Euler(0, -_direction.eulerAngles.y, 0);
        Vector3 charactercontrollerSpeed = yRotation*_characterController.velocity;
        Vector3 correctedSpeed = speed;
        if(charactercontrollerSpeed.x > speed.x)
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
        if(_characterController != null)
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
        if(_characterController.isGrounded)
        {
            var spherePosition = _characterController.bounds.center - new Vector3(0, _characterController.bounds.extents.y, 0);
            bool grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
            return grounded;
        }
        return false;
    }
    public override void OnJumpPressed(InputAction.CallbackContext ctx)
    {
        if (_onGround)
        {
            _Jump = true;
        }
    }
}
