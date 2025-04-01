using HP;
using Unity.Netcode;
using UnityEngine;
using static PlayerMoveControl.MoveEvent;

public class PlayerMoveControl : ClientPrediction
{
    [SerializeField]
    CharacterController _characterController;
    WASD_MouseBinding _WASD_MouseBinding;

    [SerializeField]
    Vector2 _curMouseSpeed;
    [SerializeField]
    Vector3 _curMoveSpeed;
    [SerializeField]
    Vector3 _curEnvSpeed;
    [SerializeField]
    bool _curIsJumpValid;

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
        public MoveInputEvent _moveInputEvent;
        public MouseInputEvent _mouseInputEvent;
        public enum MoveEventType
        {
            MoveInputEvent,
            MouseInputEvent
        }
        public MoveEventType _eventType;
        int ITickEvent._tick
        {
            get
            {
                if (_eventType == MoveEventType.MoveInputEvent)
                {
                    return _moveInputEvent._tick;
                }
                else
                {
                    return _mouseInputEvent._tick;
                }
            }

            set
            {
                if (_eventType == MoveEventType.MoveInputEvent)
                {
                    _moveInputEvent._tick = value;
                }
                else
                {
                    _mouseInputEvent._tick = value;
                }
            }

        }
        bool ITickEvent._isPredict
        {
            get
            {
                if (_eventType == MoveEventType.MoveInputEvent)
                {
                    return _moveInputEvent._isPredict;
                }
                else
                {
                    return _mouseInputEvent._isPredict;
                }
            }

            set
            {
                if (_eventType == MoveEventType.MoveInputEvent)
                {
                    _moveInputEvent._isPredict = value;
                }
                else
                {
                    _mouseInputEvent._isPredict = value;
                }
            }
        }

        ITickEvent ITickEvent.DeepCopy()
        {
            if (_eventType == MoveEventType.MoveInputEvent)
            {

                return new MoveEvent()
                {
                    _moveInputEvent = _moveInputEvent.DeepCopy(),
                    _eventType = MoveEventType.MoveInputEvent
                };
            }
            else
            {
                return new MoveEvent()
                {
                    _mouseInputEvent = _mouseInputEvent.DeepCopy(),
                    _eventType = MoveEventType.MouseInputEvent
                };
            }
        }

        bool ITickEvent.CheckEventDirty(ITickEvent compare)
        {
            if (_eventType == MoveEventType.MoveInputEvent)
            {
                return _moveInputEvent.CheckEventDirty(compare);
            }
            else
            {
                return _mouseInputEvent.CheckEventDirty(compare);
            }
        }

        void ITickEvent.Serelize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _eventType);
            if (_eventType == MoveEventType.MoveInputEvent)
            {
                _moveInputEvent.Serelize(serializer);
            }
            else
            {
                _mouseInputEvent.Serelize(serializer);
            }
        }
    }
    public struct MoveInputEvent
    {
        public Vector3 _moveInput;
        public int _tick;
        public bool _isPredict;
        public MoveInputEvent DeepCopy()
        {
            return new MoveInputEvent()
            {
                _tick = _tick,
                _moveInput = _moveInput,
                _isPredict = _isPredict,
            };
        }
        public bool CheckEventDirty(ITickEvent compare)
        {
            var compareEvent = (MoveEvent)compare;
            bool typeDirty = MoveEventType.MoveInputEvent != compareEvent._eventType;
            if (typeDirty) return true;
            bool valueDirty = false;
            float threshold = 0.0001f;
            var moveEvent = compareEvent._moveInputEvent;
            valueDirty = Vector2.Distance(_moveInput, moveEvent._moveInput) > threshold;
            bool tickDirty = _tick != compareEvent._moveInputEvent._tick;
            return (valueDirty || tickDirty);
        }
        public void Serelize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _moveInput);
            serializer.SerializeValue(ref _tick);
        }
    }
    public struct MouseInputEvent
    {
        public int _tick;
        public Vector2 _mouseInput;
        public bool _isPredict;
        public MouseInputEvent DeepCopy()
        {
            return new MouseInputEvent()
            {
                _tick = _tick,
                _mouseInput = _mouseInput,
                _isPredict = _isPredict,
            };
        }

        public bool CheckEventDirty(ITickEvent compare)
        {
            var compareEvent = (MoveEvent)compare;
            bool typeDirty = MoveEventType.MouseInputEvent != compareEvent._eventType;
            if (typeDirty) return true;
            bool valueDirty = false;
            float threshold = 0.0001f;
            var moveEvent = (MouseInputEvent)compareEvent._mouseInputEvent;
            valueDirty = Vector2.Distance(_mouseInput, moveEvent._mouseInput) > threshold;
            bool tickDirty = _tick != moveEvent._tick;
            return (valueDirty || tickDirty);
        }

        public void Serelize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
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
            _WASD_MouseBinding.Dispose();
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _WASD_MouseBinding = new WASD_MouseBinding();
            _WASD_MouseBinding.Enable(true);
            _WASD_MouseBinding._onMouseInputChanged += OnMouseInputChanged;
            _WASD_MouseBinding._onMoveInputChanged += OnMoveInputChanged;
        }
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
    override public void ApplyTickEvent(int tick)
    {
        AddPrevMoveInputEvent(tick);
        if (!CheckTickStateDirty(tick + 1))
        {
            return;
        }
        int historyIndex = tick % MaxBufferedTick;
        for (int i = 0; i < _tickEventHistory._buffer[historyIndex].Count; i++)
        {
            var history = (MoveEvent)_tickEventHistory._buffer[historyIndex][i];
            if (history._eventType == MoveEvent.MoveEventType.MouseInputEvent)
            {
                var message = (MouseInputEvent)history._mouseInputEvent;
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
            else if (history._eventType == MoveEvent.MoveEventType.MoveInputEvent)
            {
                var message = (MoveInputEvent)history._moveInputEvent;
                Vector3 moveInput = message._moveInput;
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                Quaternion yawRotation = Quaternion.Euler(0, _characterController.transform.eulerAngles.y, 0);
                moveDirection = yawRotation * moveDirection;
                _characterController.Move(Vector3.Scale(moveDirection, _curMoveSpeed) * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
            }
        }

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
            for (int i = 0; i < _tickEventHistory._buffer[historyIndex].Count; i++)
            {
                eventList[i] = _tickEventHistory._buffer[historyIndex][i].DeepCopy();
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
            var moveEvent = prevhistory.FindLast(e => ((MoveEvent)e)._eventType == MoveEventType.MoveInputEvent);
            if (moveEvent != null)
            {
                var moveInputEvent = (MoveEvent)moveEvent;
                moveInputEvent._moveInputEvent._tick = tick;
                moveInputEvent._moveInputEvent._isPredict = true;
                _tickEventHistory._buffer[historyIndex].Insert(0, moveInputEvent);
                return;
            }
        }
        var defaultMoveInputEvent = new MoveInputEvent();
        defaultMoveInputEvent._tick = tick;
        defaultMoveInputEvent._isPredict = true;
        _tickEventHistory._buffer[historyIndex].Insert(0, new MoveEvent()
        {
            _eventType = MoveEventType.MoveInputEvent,
            _moveInputEvent = defaultMoveInputEvent
        });
    }
    bool OnGround()
    {
        Vector3 capsuleBottom = _characterController.transform.position - Vector3.up * (_characterController.height / 2);
        Vector3 capsuleTop = _characterController.transform.position + Vector3.up * (_characterController.height / 2);

        Vector3 capsuleBottomCenter = capsuleBottom + Vector3.up * _characterController.radius;
        Vector3 capsuleTopCenter = capsuleTop - Vector3.up * _characterController.radius;

        float capsuleCastLength = 0.05f;
        bool onGround = Physics.CapsuleCast(capsuleBottomCenter, capsuleTopCenter, _characterController.radius, Vector3.down, out RaycastHit hit, capsuleCastLength);
        return onGround;
    }
    private void OnMoveInputChanged(Vector3 val)
    {
        if (!IsSpawned) return;
        var moveEvent = new MoveInputEvent();
        moveEvent._tick = NetworkManager.NetworkTickSystem.LocalTime.Tick + 1;
        moveEvent._moveInput = val;
        moveEvent._isPredict = false;
        CashEvent(new MoveEvent()
        {
            _eventType = MoveEventType.MoveInputEvent,
            _moveInputEvent = moveEvent
        });
    }
    private void OnMouseInputChanged(Vector2 val)
    {
        if (!IsSpawned) return;
        var moveEvent = new MouseInputEvent();
        moveEvent._tick = NetworkManager.NetworkTickSystem.LocalTime.Tick + 1;
        moveEvent._isPredict = false;
        moveEvent._mouseInput = val;
        CashEvent(new MoveEvent()
        {
            _eventType = MoveEventType.MouseInputEvent,
            _mouseInputEvent = moveEvent
        });
    }
}
