using HP;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static PlayerMoveControl;
using static PlayerMoveControl.MoveEvent;

public class PlayerMoveControl : ClientPrediction
{
    [SerializeField]
    CharacterController _characterController;
    HP.PlayerInput _playerInput;

    [SerializeField]
    Vector2 _curMouseSpeed;
    [SerializeField]
    Vector3 _curMoveSpeed;
    [SerializeField]
    Vector3 _curEnvSpeed;
    [SerializeField]
    bool _curIsJumpValid;
    LinkedList<IMoveEvent> _userInput;
    public struct TickStateMessage : ITickStateMessage
    {
        public ITickState _tickState;

        ITickState ITickStateMessage._tickState { get => _tickState; set => _tickState = value; }

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            if (serializer.IsReader)
            {
                _tickState = new MoveState();
                _tickState.Serelize(serializer);
                _tickState._isPredict = false;

            }
            else
            {
                _tickState.Serelize(serializer);
                _tickState._isPredict = false;
            }
        }
    }
    public struct TickEventListMessage : ITickEventListMessage
    {
        public ITickEvent[] _eventList;
        public int _tick;
        int ITickEventListMessage._tick { get => _tick; set => _tick = value; }
        ITickEvent[] ITickEventListMessage._eventList { get => _eventList; set => _eventList = value; }
        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            if (serializer.IsWriter)
            {
                serializer.SerializeValue(ref _tick);
                int count = _eventList.Length;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < _eventList.Length; i++)
                {
                    _eventList[i].Serelize(serializer);
                    _eventList[i]._isPredict = false;
                }
            }
            else
            {
                serializer.SerializeValue(ref _tick);
                int count = 0;
                serializer.SerializeValue(ref count);
                _eventList = new ITickEvent[count];
                for (int i = 0; i < count; i++)
                {
                    _eventList[i] = new MoveEvent();
                    _eventList[i].Serelize(serializer);
                    _eventList[i]._isPredict = false;
                }
            }
        }
    }
    public struct MoveState : ITickState
    {
        public Vector3 _worldPos;
        public Quaternion _worldRot;
        public bool _isJumpValid;
        public Vector3 _moveSpeed;
        public Vector2 _mouseSpeed;
        public Vector3 _envSpeed;
        public int _tick;
        public bool _isPredict;
        bool ITickState._isPredict { get => _isPredict; set => _isPredict = value; }
        int ITickState._tick { get => _tick; set => _tick = value; }

        ITickState ITickState.DeepCopy()
        {
            return new MoveState()
            {
                _worldPos = _worldPos,
                _worldRot = _worldRot,
                _isJumpValid = _isJumpValid,
                _moveSpeed = _moveSpeed,
                _mouseSpeed = _mouseSpeed,
                _envSpeed = _envSpeed,
                _tick = _tick,
                _isPredict = _isPredict
            };
        }

        public bool CheckStateDirty(ITickState compareState)
        {
            if (compareState == null) return true;
            var state = (MoveState)compareState;
            float threshold = 0.0001f;
            bool worldPosDirty = Vector3.Distance(_worldPos, state._worldPos) > threshold;
            bool envSpeedDirty = Vector3.Distance(_envSpeed, state._envSpeed) > threshold;
            bool moveSpeedDirty = Vector3.Distance(_moveSpeed, state._moveSpeed) > threshold;
            bool mouseSpeedDirty = Vector3.Distance(_mouseSpeed, state._mouseSpeed) > threshold;
            bool ValidJumpDirty = _isJumpValid != state._isJumpValid;
            bool worldRotDirty = Quaternion.Dot(_worldRot, state._worldRot) < (1.0f - threshold);
            bool tickDirty = _tick != state._tick;
            return (worldPosDirty || worldRotDirty || mouseSpeedDirty || moveSpeedDirty || envSpeedDirty || ValidJumpDirty || tickDirty);
        }
        public void Serelize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _tick);
            serializer.SerializeValue(ref _worldPos);
            serializer.SerializeValue(ref _worldRot);
            serializer.SerializeValue(ref _envSpeed);
            serializer.SerializeValue(ref _moveSpeed);
            serializer.SerializeValue(ref _mouseSpeed);
            serializer.SerializeValue(ref _isJumpValid);
        }
    }
    public struct MoveEvent : ITickEvent
    {
        public IMoveEvent _iMoveEvent;
        public enum MoveEventType
        {
            MoveInputEvent,
            MouseInputEvent,
            JumpInputEvent
        }
        int ITickEvent._tick { get => _iMoveEvent._tick; set => _iMoveEvent._tick = value; }    
        bool ITickEvent._isPredict { get => _iMoveEvent._isPredict; set => _iMoveEvent._isPredict = value; }
        ITickEvent ITickEvent.DeepCopy()
        {
            return new MoveEvent()
            {
                _iMoveEvent = _iMoveEvent.DeepCopy(),
            };
        }

        bool ITickEvent.CheckEventDirty(ITickEvent compare)
        {
            return _iMoveEvent.CheckEventDirty(((MoveEvent)compare)._iMoveEvent);
        }

        void ITickEvent.Serelize<T>(BufferSerializer<T> serializer)
        {

            if (serializer.IsWriter)
            {
                MoveEventType _moveEventType = _iMoveEvent._moveEventType;
                serializer.SerializeValue(ref _moveEventType);
                _iMoveEvent.Serelize(serializer);
            }
            else
            {
                MoveEventType _moveEventType = MoveEventType.MoveInputEvent;
                serializer.SerializeValue(ref _moveEventType);
                switch (_moveEventType)
                {
                    case MoveEventType.MoveInputEvent:
                        _iMoveEvent = new MoveInputEvent();
                        break;
                    case MoveEventType.MouseInputEvent:
                        _iMoveEvent = new MouseInputEvent();
                        break;
                    case MoveEventType.JumpInputEvent:
                        _iMoveEvent = new JumpInputEvent();
                        break;
                }
                _iMoveEvent.Serelize(serializer);

            }
        }
    }
    public interface IMoveEvent 
    {
        int _tick { get; set; }
        bool _isPredict { get; set; }
        MoveEventType _moveEventType { get; }
        IMoveEvent DeepCopy();
        bool CheckEventDirty(IMoveEvent compare);
        void Serelize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
    }
    public struct JumpInputEvent : IMoveEvent
    {
        public float _jumpInput;
        public int _tick;
        public bool _isPredict;

        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        bool IMoveEvent._isPredict { get => _isPredict; set => _isPredict = value; }
        MoveEventType IMoveEvent._moveEventType => MoveEventType.JumpInputEvent;

        IMoveEvent IMoveEvent.DeepCopy()
        {
            return new JumpInputEvent()
            {
                _tick = _tick,
                _jumpInput = _jumpInput,
                _isPredict = _isPredict,
            };
        }
        bool IMoveEvent.CheckEventDirty(IMoveEvent compare)
        {
            bool typeDirty = MoveEventType.JumpInputEvent != compare._moveEventType;
            if (typeDirty) return true;
            bool valueDirty = false;
            var jumpEvent =  (JumpInputEvent)compare;
            valueDirty = _jumpInput != jumpEvent._jumpInput;
            bool tickDirty = _tick != jumpEvent._tick;
            return (valueDirty || tickDirty);
        }
        void IMoveEvent.Serelize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _jumpInput);
            serializer.SerializeValue(ref _tick);
        }
    }
    public struct MoveInputEvent : IMoveEvent
    {
        public Vector3 _moveInput;
        public int _tick;
        public bool _isPredict;
        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        bool IMoveEvent._isPredict { get => _isPredict; set => _isPredict = value; }
        MoveEventType IMoveEvent._moveEventType => MoveEventType.MoveInputEvent;

        IMoveEvent IMoveEvent.DeepCopy()
        {
            return new MoveInputEvent()
            {
                _tick = _tick,
                _moveInput = _moveInput,
                _isPredict = _isPredict,
            };
        }
         bool IMoveEvent.CheckEventDirty(IMoveEvent compare)
        {
            bool typeDirty = MoveEventType.MoveInputEvent != compare._moveEventType;
            if (typeDirty) return true;
            bool valueDirty = false;
            float threshold = 0.0001f;
            var moveEvent = (MoveInputEvent)compare;
            valueDirty = Vector2.Distance(_moveInput, moveEvent._moveInput) > threshold;
            bool tickDirty = _tick != moveEvent._tick;
            return (valueDirty || tickDirty);
        }
         void IMoveEvent.Serelize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _moveInput);
            serializer.SerializeValue(ref _tick);
        }
    }
    public struct MouseInputEvent : IMoveEvent
    {
        public int _tick;
        public Vector2 _mouseInput;
        public bool _isPredict;
        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        bool IMoveEvent._isPredict { get => _isPredict; set => _isPredict = value; }
        MoveEventType IMoveEvent._moveEventType => MoveEventType.MouseInputEvent;

        IMoveEvent IMoveEvent.DeepCopy()
        {
            return new MouseInputEvent()
            {
                _tick = _tick,
                _mouseInput = _mouseInput,
                _isPredict = _isPredict,
            };
        }
        bool IMoveEvent.CheckEventDirty(IMoveEvent compare)
        {
            bool typeDirty = MoveEventType.MouseInputEvent != compare._moveEventType;
            if (typeDirty) return true;
            bool valueDirty = false;
            float threshold = 0.0001f;
            var moveEvent = (MouseInputEvent)compare;
            valueDirty = Vector2.Distance(_mouseInput, moveEvent._mouseInput) > threshold;
            bool tickDirty = _tick != moveEvent._tick;
            return (valueDirty || tickDirty);
        }
        void IMoveEvent.Serelize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _mouseInput);
            serializer.SerializeValue(ref _tick);
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            _playerInput.Dispose();
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _userInput = new LinkedList<IMoveEvent>();
            BindKey();
        }
    }
    public override void OnLocalTick()
    {
        int localTick = NetworkManager.NetworkTickSystem.LocalTime.Tick;
        if(IsOwner)
        {
            foreach (var imoveEvent in _userInput)
            {
                MoveEvent newEvent = new MoveEvent();
                newEvent._iMoveEvent = imoveEvent;
                newEvent._iMoveEvent._tick = localTick;
                newEvent._iMoveEvent._isPredict = false;
                CashEvent(newEvent);
            }
            _userInput.Clear();
        }
        base.OnLocalTick();
    }
    void BindKey()
    {
        _playerInput = new HP.PlayerInput("Player");
        _playerInput.Enable(true);
        _playerInput._onMouseInputChanged += OnMouseInputChanged;
        _playerInput._onMoveInputChanged += OnMoveInputChanged;
        _playerInput._onJumpInputChanged += OnJumpInputChanged;
    }
    private void Update()
    {
        if (IsSpawned)
        {
            Vector3 capsuleBottom = _characterController.transform.position - Vector3.up * (_characterController.height / 2);
            Vector3 capsuleTop = _characterController.transform.position + Vector3.up * (_characterController.height / 2);
            var color = Color.cyan;
            if (OnGround())
            {
                color = Color.red;
            }
            DebugExtension.DebugCapsule(capsuleBottom, capsuleTop, radius: _characterController.radius + 0.05f, color: color);
        }
    }
    void ApplyMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        foreach (var moveEvent in _tickEventHistory._buffer[historyIndex])
        {
            var history = (MoveEvent)moveEvent;
            if (history._iMoveEvent._moveEventType == MoveEvent.MoveEventType.MouseInputEvent)
            {
                var message = (MouseInputEvent)history._iMoveEvent;
                Vector2 mouseInput = message._mouseInput;
                Vector3 currentRotation = _characterController.transform.rotation.eulerAngles;
                float newYaw = currentRotation.y + mouseInput.x * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime * _curMouseSpeed.x;
                float newPitch = currentRotation.x - mouseInput.y * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime * _curMouseSpeed.y;
                // Pitch 값 클램핑 (-60 ~ 60도)
                if (newPitch > 180f) newPitch -= 360f;
                newPitch = Mathf.Clamp(newPitch, -60f, 60f);
                Quaternion finalRotation = Quaternion.Euler(newPitch, newYaw, 0);
                _characterController.transform.rotation = finalRotation;
            }
            else if (history._iMoveEvent._moveEventType == MoveEvent.MoveEventType.MoveInputEvent)
            {
                var message = (MoveInputEvent)history._iMoveEvent;
                Vector3 moveInput = message._moveInput;
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                Quaternion yawRotation = Quaternion.Euler(0, _characterController.transform.eulerAngles.y, 0);
                moveDirection = yawRotation * moveDirection;
                _characterController.Move(Vector3.Scale(moveDirection, _curMoveSpeed) * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
            }
            else if (history._iMoveEvent._moveEventType == MoveEvent.MoveEventType.JumpInputEvent)
            {
                if (_curIsJumpValid)
                {
                    _curEnvSpeed.y = Mathf.Sqrt(2 * 4.0f * 9.8f * 2.0f);
                    _characterController.Move(Vector3.Scale(Vector3.up, _curEnvSpeed) * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
                    if (OnGround())
                    {
                        //Maybe This Should Not Happen
                        _curEnvSpeed.y = -0.1f;
                        _curIsJumpValid = true;
                        Debug.LogWarning("Double Jump Warning");
                    }
                }
            }
        }
    }
    void ApplyGravity(int tick)
    {
        //Gravity Event
        _curEnvSpeed.y -= 4.0f * 9.8f * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime;
        _characterController.Move(Vector3.Scale(Vector3.up, _curEnvSpeed) * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
        if (OnGround())
        {
            _curEnvSpeed.y = -0.1f;
            _curIsJumpValid = true;
        }
        else
        {
            _curIsJumpValid = false;
        }
    }
    override public void ApplyTickEvent(int tick)
    {
        AddPrevMoveInputEvent(tick);
        if (!CheckTickStateDirty(tick + 1))
        {
            return;
        }
        ApplyMoveEvent(tick);
        ApplyGravity(tick);
    }
    override public void SetTickStateFromHistory(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        var history = (MoveState)_tickStateHistory._buffer[historyIndex];
        _characterController.enabled = false;
        transform.position = history._worldPos;
        transform.rotation = history._worldRot;
        _curMoveSpeed = history._moveSpeed;
        _curMouseSpeed = history._mouseSpeed;
        _curEnvSpeed = history._envSpeed;
        _curIsJumpValid = history._isJumpValid;
        _characterController.enabled = true;
    }
    override public void CashCurrentState(int tick)
    {
        CashState(new MoveState()
        {
            _tick = tick,
            _worldPos = transform.position,
            _worldRot = transform.rotation,
            _isPredict = true,
            _moveSpeed = _curMoveSpeed,
            _mouseSpeed = _curMouseSpeed,
            _envSpeed = _curEnvSpeed,
            _isJumpValid = _curIsJumpValid
        });
    }
    override public void SendTickState(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        SendTickStateRpc(new TickStateMessage()
        {
            _tickState = _tickStateHistory._buffer[historyIndex].DeepCopy()
        });
    }
    override public void SendTickEventList(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        if (_tickEventHistory._buffer[historyIndex].Count != 0)
        {
            var eventList = new ITickEvent[_tickEventHistory._buffer[historyIndex].Count];
            int i = 0;
            foreach (var moveEvent in _tickEventHistory._buffer[historyIndex])
            {
                eventList[i] = moveEvent.DeepCopy();
                i++;
            }
            SendTickEventListRpc(new TickEventListMessage()
            {
                _tick = tick,
                _eventList = eventList
            });
        }
    }
    [Rpc(SendTo.NotServer)]
    void SendTickStateRpc(TickStateMessage tickStateMessage)
    {
        _reconTickStateList.Add(tickStateMessage);
    }
    [Rpc(SendTo.NotMe)]
    void SendTickEventListRpc(TickEventListMessage tickEventmesssage)
    {
        _reconTickEventList.Add(tickEventmesssage);
    }
    void AddPrevMoveInputEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        for (int i = 0; i < MaxBufferedTick; i++)
        {
            int prevHistoryIndex = (tick + MaxBufferedTick - i) % MaxBufferedTick;
            var prevhistory = _tickEventHistory._buffer[prevHistoryIndex];
            var moveEvent = prevhistory.LastOrDefault(e => ((MoveEvent)e)._iMoveEvent._moveEventType == MoveEventType.MoveInputEvent);
            if (moveEvent != null)
            {
                if (i == 0) return;
                var moveInputEvent = (MoveEvent)moveEvent;
                moveInputEvent._iMoveEvent._tick = tick;
                moveInputEvent._iMoveEvent._isPredict = true;
                _tickEventHistory._buffer[historyIndex].AddFirst(moveInputEvent);
                return;
            }
        }
        var defaultMoveInputEvent = new MoveInputEvent();
        defaultMoveInputEvent._tick = tick;
        defaultMoveInputEvent._isPredict = true;
        _tickEventHistory._buffer[historyIndex].AddFirst(new MoveEvent()
        {
            _iMoveEvent = defaultMoveInputEvent
        });
    }
    bool OnGround()
    {
        Vector3 capsuleBottom = _characterController.transform.position - Vector3.up * (_characterController.height / 2);
        Vector3 capsuleTop = _characterController.transform.position + Vector3.up * (_characterController.height / 2);

        Vector3 capsuleBottomCenter = capsuleBottom + Vector3.up * _characterController.radius;
        Vector3 capsuleTopCenter = capsuleTop - Vector3.up * _characterController.radius;

        float capsuleCastLength = 0.1f;
        bool onGround = Physics.CapsuleCast(capsuleBottomCenter, capsuleTopCenter, _characterController.radius, Vector3.down, out RaycastHit hit, capsuleCastLength);
        return onGround;
    }
    private void OnMoveInputChanged(Vector3 val)
    {
        if (!IsSpawned) return;
        var moveEvent = new MoveInputEvent();
        moveEvent._moveInput = val;
        _userInput.AddLast(moveEvent);
    }
    private void OnMouseInputChanged(Vector2 val)
    {
        if (!IsSpawned) return;
        var moveEvent = new MouseInputEvent();
        moveEvent._mouseInput = val;

        var input = _userInput.Last;
        if (input !=null)
        {
            if(input.Value is MouseInputEvent inputMouse)
            {
                inputMouse._mouseInput += val;
                input.Value = inputMouse;
                return;
            }
        }
        _userInput.AddLast(moveEvent);
    }
    private void OnJumpInputChanged(float val)
    {
        if (!IsSpawned) return;
        if (val == 0) return;
        var moveEvent = new JumpInputEvent();
        moveEvent._jumpInput = val;
        _userInput.AddLast(moveEvent);
    }
}
