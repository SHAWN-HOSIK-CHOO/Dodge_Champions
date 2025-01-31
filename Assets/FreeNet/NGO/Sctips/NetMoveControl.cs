using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static NetMoveControl;

public class NetMoveControl : NetBindInput
{
    /*
     * 클라 (Owner) 
     *      (Input) 변경시 (Input,time,transform) 정보를 서버로 전송
     *      MovementRadius를 수신 받아 그 안에서 이동
     *      주기적으로 (time,transform,direction) 정보를 서버로 전송
     *      
     * 서버
     *      MovementRadius 설정.
     *      클라(Owner)의 (Input,time,transform)을 수신 하면 클라(others) 로 전달
     *      클라(Owner)의 (time,transform, direction)을 주기적 수신하여 클라(others)로 전달 
     *      클라(All)에게 MovementRadius 전달 
     *      클라(Owner)의 수신 정보로 MovementRadius 안에서 예측 보간 이동
     *      
     *      
     * 클라 (Others)
     *      서버로 부터 클라(Owner)의 정보 및 MovementRadius를 수신 받아 
     *      MovementRadius 안에서 예측 보간 이동
     */

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


    #region Sync
    struct NetMoveControlInfo
    {
        public Vector3 _startPos;
        public Vector3 _moveInput;
        public Quaternion _direction;
        public float _time;
    }

    Queue<NetMoveControlInfo> _netContorlInfos;

    [SerializeField]
    [Tooltip("Allow Radius")]
    float _allowMoveRadius = 3.0f;
    [SerializeField]
    [Tooltip("Net Allow Radius")]
    float _netAllowMoveRadius;

    [SerializeField]
    [Tooltip("Net Allow Move Center")]
    Vector3 _netAllowMoveCenter;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _onMoveInputChanged += OnMoveInputChanged;
        _ondirectionChanged += OnDirectionChanged;
    }

    void OnDirectionChanged()
    {

    }
    void OnMoveInputChanged()
    {
        if(IsOwner)
        {
            MoveInputChangedRpc(transform.position, _direction, _moveInput ,(float)NetworkManager.Singleton.LocalTime.Time);
        }
    }

    [Rpc(SendTo.NotOwner)]
    void MoveInputChangedRpc(Vector3 pos, Quaternion dir, Vector3 moveinput, float time ,RpcParams rpcParams = default)
    {
        _netContorlInfos.Enqueue(new NetMoveControlInfo()
        {
            _startPos = pos,
            _moveInput = moveinput,
            _direction = dir,
            _time = time
        });
    }
    [Rpc(SendTo.Everyone)]
    void MovementRadiusRpc(Vector3 allowCenter, float allowRadius, float time, RpcParams rpcParams = default)
    {
        _netAllowMoveCenter = allowCenter;
        _netAllowMoveRadius = allowRadius;
    }

    protected void Awake()
    {
        base.Awake();
    }
    protected void Start()
    {
        base.Start();
        _netContorlInfos = new Queue<NetMoveControlInfo>();
        _netAllowMoveCenter = Vector3.zero;
        _netAllowMoveRadius = 0;

    }
    private void FixedUpdate()
    {
        if (!IsSpawned) return;

        var vector = Vector3.zero;
        float timeoffset = Time.fixedDeltaTime;
        if (IsOwner)
        {
            if (_onGround = CheckGround())
            {
                _speed.y = 0;
                if (_Jump)
                {
                    _speed.y += Mathf.Sqrt(_jumpHeight * -2f * _gravityAcceleration);
                    _Jump = false;
                }
            }
            else
            {
                // Gravity
                _speed.y += _gravityAcceleration * timeoffset;
            }
            var moveflag = _moveInput;
            if (_speed.y != 0) moveflag.y = 1;
            Quaternion yRotation = Quaternion.Euler(0, _direction.eulerAngles.y, 0);
            var finalSpeed = yRotation * AccelCorrection(Vector3.Scale(moveflag, _speed), timeoffset);
            vector = finalSpeed * timeoffset;
        }
        else
        {
            var localtime = IsServer ? NetworkManager.Singleton.ServerTime : NetworkManager.Singleton.LocalTime;
            while (_netContorlInfos.TryDequeue(out var info))
            {
                timeoffset += (float)localtime.Time - info._time;
                _moveInput = info._moveInput;
                _direction = info._direction;
            }
            if (_onGround = CheckGround())
            {
                _speed.y = 0;
                if (_Jump)
                {
                    _speed.y += Mathf.Sqrt(_jumpHeight * -2f * _gravityAcceleration);
                    _Jump = false;
                }
            }
            else
            {
                // Gravity
                _speed.y += _gravityAcceleration * timeoffset;
            }
            var moveflag = _moveInput;
            if (_speed.y != 0) moveflag.y = 1;
            Quaternion yRotation = Quaternion.Euler(0, _direction.eulerAngles.y, 0);
            var finalSpeed = yRotation * AccelCorrection(Vector3.Scale(moveflag, _speed), timeoffset);
            vector = finalSpeed * timeoffset;
        }
        var expectedPos = transform.position + vector;
        Vector3 posoffset = (expectedPos - _netAllowMoveCenter);
        if (posoffset.magnitude > _netAllowMoveRadius)
        {
            if (posoffset.magnitude > 0)
            {
                var allowpos = _netAllowMoveCenter + Vector3.Normalize(posoffset) * _netAllowMoveRadius;
                vector = allowpos - transform.position;
            }
        }
        _characterController.Move(vector);
        transform.forward = _direction * Vector3.forward * timeoffset;
        if (IsServer)
        {
            MovementRadiusRpc(transform.position, _allowMoveRadius, (float)NetworkManager.Singleton.ServerTime.Time);
        }
    }


    Vector3 AccelCorrection(Vector3 speed ,float deltatime)
    {
        Quaternion yRotation = Quaternion.Euler(0, -_direction.eulerAngles.y, 0);
        Vector3 charactercontrollerSpeed = yRotation * _characterController.velocity;
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

        var spheredPosition = _characterController.bounds.center - new Vector3(0, _characterController.bounds.extents.y, 0);
        bool groundded = Physics.CheckSphere(spheredPosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
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
