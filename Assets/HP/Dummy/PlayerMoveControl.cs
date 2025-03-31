using HP;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static PlayerMoveControl;
using static PlayerMoveControl.IMoveEvent;

public class PlayerMoveControl : NetworkBehaviour
{
    [SerializeField]
    CharacterController _characterController;
    WASD_MouseBinding _WASD_MouseBinding;

    [SerializeField]
    float _mouseSpeed;
    [SerializeField]
    Vector3 _curMoveSpeed;
    [SerializeField]
    Vector3 _curEnvSpeed;
    bool _curIsJumpValid;
    List<MoveEventMessage> _reconMoveEventList;
    List<MoveStateMessage> _reconMoveStateList;

    public struct MoveStateMessage : INetworkSerializable
    {
        public int _tick;
        public Vector3 _worldPos;
        public Quaternion _worldRot;
        public bool _isJumpValid;
        public Vector3 _moveSpeed;
        public Vector3 _envSpeed;

        public MoveState AsMoveState()
        {
            return new MoveState()
            {
                _tick = _tick,
                _worldPos = _worldPos,
                _worldRot = _worldRot,
                _envSpeed = _envSpeed,
                _moveSpeed = _moveSpeed,
                _isJumpValid = _isJumpValid,
                _isClientPredicted = false,
            };
        }
        public static MoveStateMessage FromMoveState(MoveState moveState)
        {
            return new MoveStateMessage()
            {
                _tick = moveState._tick,
                _worldPos = moveState._worldPos,
                _worldRot = moveState._worldRot,
                _envSpeed = moveState._envSpeed,
                _moveSpeed = moveState._moveSpeed,
                _isJumpValid = moveState._isJumpValid,
            };
        }
        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _tick);
            serializer.SerializeValue(ref _worldPos);
            serializer.SerializeValue(ref _worldRot);
            serializer.SerializeValue(ref _envSpeed);
            serializer.SerializeValue(ref _moveSpeed);
            serializer.SerializeValue(ref _isJumpValid);
        }
    }
    public struct MoveState
    {
        public int _tick;
        public Vector3 _worldPos;
        public Quaternion _worldRot;
        public bool _isJumpValid;
        public Vector3 _moveSpeed;
        public Vector3 _envSpeed;
        public bool _isClientPredicted;
    }
    public struct MoveEventMessage : INetworkSerializable
    {
        public IMoveEvent[] eventList;
        public int _tick;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _tick);
            if (serializer.IsWriter)
            {
                int count = eventList.Length;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < eventList.Length; i++)
                {
                    MoveEventType eventType = eventList[i]._eventType;
                    serializer.SerializeValue(ref eventType);
                    switch (eventType)
                    {
                        case MoveEventType.MoveInputEvent:
                            var moveEvent = (MoveInputEvent)eventList[i];
                            serializer.SerializeValue(ref moveEvent._tick);
                            serializer.SerializeValue(ref moveEvent._moveInput);
                            break;

                        case MoveEventType.MouseInputEvent:
                            var mouseEvent = (MouseInputEvent)eventList[i];
                            serializer.SerializeValue(ref mouseEvent._tick);
                            serializer.SerializeValue(ref mouseEvent._mouseInput);
                            break;
                    }
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);
                eventList = new IMoveEvent[count];
                for (int i = 0; i < count; i++)
                {
                    MoveEventType eventType = MoveEventType.MoveInputEvent;
                    serializer.SerializeValue(ref eventType);

                    switch (eventType)
                    {
                        case MoveEventType.MoveInputEvent:
                            var moveEvent = new MoveInputEvent();
                            serializer.SerializeValue(ref moveEvent._tick);
                            serializer.SerializeValue(ref moveEvent._moveInput);
                            eventList[i] = moveEvent;
                            break;

                        case MoveEventType.MouseInputEvent:
                            var mouseEvent = new MouseInputEvent();
                            serializer.SerializeValue(ref mouseEvent._tick);
                            serializer.SerializeValue(ref mouseEvent._mouseInput);
                            eventList[i] = mouseEvent;
                            break;
                    }
                    eventList[i]._isUsed = false;
                    eventList[i]._isClientPredicted = false;
                }
            }
        }
    }
    public interface IMoveEvent
    {
        public enum MoveEventType
        {
            MoveInputEvent,
            MouseInputEvent
        }
        public int _tick { get; set; }
        public MoveEventType _eventType { get; }

        public IMoveEvent DeepCopy();
        public bool _isClientPredicted { get; set; }
        public bool _isUsed { get; set; } // for Debug
    }
    public struct MoveInputEvent : IMoveEvent
    {
        public int _tick;
        public Vector3 _moveInput;
        public bool _isClientPredicted;
        public bool _isUsed; 
        public IMoveEvent DeepCopy()
        {
            return new MoveInputEvent()
            {
                _tick = _tick,
                _moveInput = _moveInput,
                _isUsed = _isUsed
            };
        }

        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        bool IMoveEvent._isUsed { get => _isUsed; set => _isUsed = value; }
        bool IMoveEvent._isClientPredicted { get => _isClientPredicted; set => _isClientPredicted = value; }
        public IMoveEvent.MoveEventType _eventType => IMoveEvent.MoveEventType.MoveInputEvent;

    }
    public struct MouseInputEvent : IMoveEvent
    {
        public Vector2 _mouseInput;
        public bool _isUsed;
        public int _tick;

        public bool _isClientPredicted;
        public IMoveEvent DeepCopy()
        {
            return new MouseInputEvent()
            {
                _tick = _tick,
                _mouseInput = _mouseInput,
                _isUsed = _isUsed
            };
        }
        bool IMoveEvent._isClientPredicted { get => _isClientPredicted; set => _isClientPredicted = value; }
        bool IMoveEvent._isUsed { get => _isUsed; set => _isUsed = value; }
        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        public IMoveEvent.MoveEventType _eventType => IMoveEvent.MoveEventType.MouseInputEvent;
    }

    ArrayBuffer<MoveState> _moveStateHistory;
    ArrayBuffer<List<IMoveEvent>> _moveEventHistory;
    int MaxBufferedTick;
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            _WASD_MouseBinding.Dispose();
        }
        NetworkManager.NetworkTickSystem.Tick -= OnLocalTick;
        NetworkManager.NetworkTickSystem.ServerTick -= OnServerTick;
    }
    public override void OnNetworkSpawn()
    {
        MaxBufferedTick = (int)NetworkManager.NetworkConfig.TickRate * 10;
        if (IsOwner)
        {
            _WASD_MouseBinding = new WASD_MouseBinding();
            _WASD_MouseBinding.Enable(true);
            _WASD_MouseBinding._onMouseInputChanged += OnMouseInputChanged;
            _WASD_MouseBinding._onMoveInputChanged += OnMoveInputChanged;
        }
        _moveStateHistory = new ArrayBuffer<MoveState>(MaxBufferedTick);
        _moveEventHistory = new ArrayBuffer<List<IMoveEvent>>(MaxBufferedTick);

        _reconMoveEventList = new List<MoveEventMessage>();
        _reconMoveStateList = new List<MoveStateMessage>();
        for (int i = 0; i < MaxBufferedTick; i++)
        {
            _moveStateHistory._buffer[i]._tick = -1;
            _moveStateHistory._buffer[i]._isClientPredicted = true;
            _moveEventHistory._buffer[i] = new List<IMoveEvent>();
        }
        NetworkManager.NetworkTickSystem.Tick += OnLocalTick;
        NetworkManager.NetworkTickSystem.ServerTick += OnServerTick;
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
    void ClearOldMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        foreach (var moveEvent in _moveEventHistory._buffer[historyIndex].ToArray())
        {
            if ((moveEvent._tick != tick) || moveEvent._isClientPredicted)
            {
                if (moveEvent._isUsed == false)
                {
                    Debug.LogWarning("tick droppted.. must increase buffer");
                }
                _moveEventHistory._buffer[historyIndex].Remove(moveEvent);
            }
        }
    }
    void AddPrevMoveInputEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        for (int i = 0; i < MaxBufferedTick; i++)
        {
            int prevHistoryIndex = (tick + MaxBufferedTick - i) % MaxBufferedTick;
            var IMoveEvent = _moveEventHistory._buffer[prevHistoryIndex].FindLast(e => e._eventType == MoveEventType.MoveInputEvent);
            if (IMoveEvent != null)
            {
                var moveInputEvent = (MoveInputEvent)IMoveEvent;
                moveInputEvent._tick = tick;
                moveInputEvent._isUsed = false;
                moveInputEvent._isClientPredicted = true;
                _moveEventHistory._buffer[historyIndex].Insert(0, moveInputEvent);
                return;
            }
        }
        var defaultMoveInputEvent = new MoveInputEvent();
        defaultMoveInputEvent._tick = tick;
        defaultMoveInputEvent._isUsed = false;
        defaultMoveInputEvent._isClientPredicted = true;
        _moveEventHistory._buffer[historyIndex].Insert(0, defaultMoveInputEvent);
    }
    bool CheckMoveStateValid(int tick , bool checkClientPredict = false)
    {
        int historyIndex = tick % MaxBufferedTick;
        bool tickDirty = _moveStateHistory._buffer[historyIndex]._tick != tick;
        bool isClientPredict = _moveStateHistory._buffer[historyIndex]._isClientPredicted;
        if (tickDirty)
        {
            return false;
        }
        else if (checkClientPredict)
        {
            return !isClientPredict;
        }
        return true;
    }
    public void SendMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        if (_moveEventHistory._buffer[historyIndex].Count != 0)
        {
            var moveEventMessage = new MoveEventMessage();
            moveEventMessage._tick = tick;
            moveEventMessage.eventList = new IMoveEvent[_moveEventHistory._buffer[historyIndex].Count];
            for (int i = 0; i < _moveEventHistory._buffer[historyIndex].Count; i++)
            {
                moveEventMessage.eventList[i] = _moveEventHistory._buffer[historyIndex][i].DeepCopy();
                moveEventMessage.eventList[i]._isUsed = false;
            }
            SendMoveEventRpc(moveEventMessage);
        }
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
    public void ApplyMoveState(int tick)
    {
        if (CheckMoveStateValid(tick , true))
        {
            SetMoveStateFromHistory(tick);
        }
        else
        {
            CashState(new MoveState()
            {
                _tick = tick,
                _worldPos = transform.position,
                _worldRot = transform.rotation,
                _isClientPredicted = true,
                _moveSpeed = _curMoveSpeed,
                _envSpeed = _curEnvSpeed,
                _isJumpValid = _curIsJumpValid
            });
        }
    }
    int ReconciliateMoveState(int tick)
    {
        int reconTick = -1;
        for (int i = 0; i < _reconMoveStateList.Count; i++)
        {
            MoveState moveState = _reconMoveStateList[i].AsMoveState();
            int historyIndex = moveState._tick % MaxBufferedTick;
            bool dirty = false;
            if (_reconMoveStateList[i]._tick <= tick)
            {
                dirty = CheckStateDirty(_moveStateHistory._buffer[historyIndex], moveState);
            }
            CashState(moveState);
            if (dirty)
            {
                if (reconTick == -1)
                {
                    reconTick = _reconMoveStateList[i]._tick;
                }
            }
        }
        _reconMoveStateList.Clear();
        return reconTick;
    }
    int ReconciliateMoveEvent(int tick)
    {
        int reconTick = -1;
        for (int i = 0; i < _reconMoveEventList.Count; i++)
        {
            bool dirty = false;
            int historyIndex = _reconMoveEventList[i]._tick % MaxBufferedTick;
            ClearOldMoveEvent(_reconMoveEventList[i]._tick);
            if (_reconMoveEventList[i]._tick <= tick)
            {
                if (_moveEventHistory._buffer[historyIndex].Count != _reconMoveEventList[i].eventList.Length)
                {
                    dirty = true;
                }
                else
                {
                    for (int j = 0; j < _reconMoveEventList[i].eventList.Length; j++)
                    {
                        if (CheckEventDirty(_moveEventHistory._buffer[historyIndex][j], _reconMoveEventList[i].eventList[j]))
                        {
                            dirty = true;
                            break;
                        }
                    }
                }
            }
            foreach (var item in _reconMoveEventList[i].eventList)
            {
                item._isUsed = !dirty;


                _moveEventHistory._buffer[historyIndex].Add(item);
            }
            if (dirty)
            {
                if (reconTick == -1)
                {
                    reconTick = _reconMoveEventList[i]._tick;
                }
            }
        }
        _reconMoveEventList.Clear();
        return reconTick;
    }
    void ReconciliationMove(int tick)
    {
        // 서버 기준으로 지난 이벤트는 무시하는 것이 맞다.
        // 그런데 key Release 이벤트는 적용해야함으로 무시하면 안된다.
        // 무조건 적용 후 재시뮬레이션 해야하는데 치트는 어떻게 막지? 

        int moveEventDirtTick = ReconciliateMoveEvent(tick);
        int moveStateDirtTick = ReconciliateMoveState(tick);

        int dirtyTick = tick;
        dirtyTick = (moveEventDirtTick == -1) ? dirtyTick : moveEventDirtTick;
        dirtyTick = (moveStateDirtTick == -1) ? dirtyTick : moveStateDirtTick;
        for (int i = dirtyTick; i <= tick; i++)
        {
            int historyIndex = i % MaxBufferedTick;
            ApplyMoveState(i);
            Debug.Log($"before {transform.position}");
            ClearOldMoveEvent(i);
            if (!CheckMoveStateValid(i + 1, true))
            {
                ApplyMoveEvent(i);
            }
            else
            {
                for (int index = 0; index < _moveEventHistory._buffer[historyIndex].Count; index++)
                {
                    _moveEventHistory._buffer[historyIndex][index]._isUsed = true;
                }
            }
            ApplyMoveState(i + 1);
            Debug.Log($"after {transform.position}");
        }

        Debug.Log($"final {transform.position}");

    }
    public void OnLocalTick()
    {
        int localTick = NetworkManager.NetworkTickSystem.LocalTime.Tick;
        if (IsOwner)
        {
            ClearOldMoveEvent(localTick);
            SendMoveEvent(localTick);

            ReconciliationMove(localTick);
        }
    }
    public void OnServerTick()
    {
        int serverTick = NetworkManager.NetworkTickSystem.ServerTime.Tick;
        if(!IsOwner)
        {
            ReconciliationMove(serverTick);
        }

        if (IsServer)
        {
            int historyIndex = serverTick % MaxBufferedTick;
            _moveStateHistory._buffer[historyIndex]._isClientPredicted = false;
            if(_moveStateHistory._buffer[historyIndex]._tick != -1)
            {
                SendMoveStateRpc(MoveStateMessage.FromMoveState(_moveStateHistory._buffer[historyIndex]));
            }

        }
    }
    void SetMoveStateFromHistory(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        _characterController.enabled = false;
        transform.position = _moveStateHistory._buffer[historyIndex]._worldPos;
        transform.rotation = _moveStateHistory._buffer[historyIndex]._worldRot;
        _curMoveSpeed = _moveStateHistory._buffer[historyIndex]._moveSpeed;
        _curEnvSpeed = _moveStateHistory._buffer[historyIndex]._envSpeed;
        _curIsJumpValid = _moveStateHistory._buffer[historyIndex]._isJumpValid;
        _characterController.enabled = true;
    }
    bool CheckEventDirty(IMoveEvent eventL, IMoveEvent eventR)
    {
        bool typeDirty = eventL._eventType != eventR._eventType;
        bool valueDirty = false;
        if (!typeDirty)
        {
            float threshold = 0.0001f;
            if (eventL._eventType == MoveEventType.MoveInputEvent)
            {
                var moveEventL = (MoveInputEvent)eventL;
                var moveEventR = (MoveInputEvent)eventR;
                valueDirty = Vector3.Distance(moveEventL._moveInput, moveEventR._moveInput) > threshold;
            }
            else if (eventL._eventType == MoveEventType.MouseInputEvent)
            {
                var moveEventL = (MouseInputEvent)eventL;
                var moveEventR = (MouseInputEvent)eventR;
                valueDirty = Vector2.Distance(moveEventL._mouseInput, moveEventR._mouseInput) > threshold;
            }
        }
        bool tickDirty = eventL._tick != eventR._tick;
        return (typeDirty || tickDirty);
    }
    bool CheckStateDirty(MoveState stateL, MoveState stateR)
    {
        float threshold = 0.0001f;
        bool worldPosDirty = Vector3.Distance(stateL._worldPos, stateR._worldPos) > threshold;
        bool envSpeedDirty = Vector3.Distance(stateL._envSpeed, stateR._envSpeed) > threshold;
        bool moveSpeedDirty = Vector3.Distance(stateL._moveSpeed, stateR._moveSpeed) > threshold;
        bool ValidJumpDirty = stateL._isJumpValid != stateR._isJumpValid;
        bool worldRotDirty = Quaternion.Dot(stateL._worldRot, stateR._worldRot) < (1.0f - threshold);
        bool tickDirty = stateL._tick != stateR._tick;
        return (worldPosDirty || worldRotDirty || envSpeedDirty || ValidJumpDirty || tickDirty);
    }
    [Rpc(SendTo.NotServer)]
    void SendMoveStateRpc(MoveStateMessage moveStateMessage)
    {
        _reconMoveStateList.Add(moveStateMessage);
    }
    [Rpc(SendTo.NotMe)]
    void SendMoveEventRpc(MoveEventMessage messsage)
    {
        _reconMoveEventList.Add(messsage);
    }
    void ApplyMoveEvent(int tick)
    {
        AddPrevMoveInputEvent(tick);
        int historyIndex = tick % MaxBufferedTick;
        for (int i = 0; i < _moveEventHistory._buffer[historyIndex].Count; i++)
        {
            _moveEventHistory._buffer[historyIndex][i]._isUsed = true;
            if (_moveEventHistory._buffer[historyIndex][i]._eventType == IMoveEvent.MoveEventType.MouseInputEvent)
            {
                var message = (MouseInputEvent)_moveEventHistory._buffer[historyIndex][i];
                Vector2 mouseInput = message._mouseInput;
                Vector3 currentRotation = _characterController.transform.rotation.eulerAngles;
                float newYaw = currentRotation.y + mouseInput.x * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime * _mouseSpeed;
                float newPitch = currentRotation.x - mouseInput.y * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime * _mouseSpeed;
                // Pitch 값 클램핑 (-60 ~ 60도)
                if (newPitch > 180f) newPitch -= 360f;
                newPitch = Mathf.Clamp(newPitch, -60f, 60f);
                Quaternion finalRotation = Quaternion.Euler(newPitch, newYaw, 0);
                _characterController.transform.rotation = finalRotation;
            }
            else if (_moveEventHistory._buffer[historyIndex][i]._eventType == IMoveEvent.MoveEventType.MoveInputEvent)
            {
                var message = (MoveInputEvent)_moveEventHistory._buffer[historyIndex][i];
                Vector3 moveInput = message._moveInput;
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                Quaternion yawRotation = Quaternion.Euler(0, _characterController.transform.eulerAngles.y, 0);
                moveDirection = yawRotation * moveDirection;
                _characterController.Move(Vector3.Scale(moveDirection, _curMoveSpeed) * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
            }
        }

        // 오브젝트가 땅에 붙어 있게 만들기 위해 중력 두배 설정
        _curEnvSpeed.y -= 2 * 9.8f * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime;
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
    public void CashState(MoveState state)
    {
        int historyIndex = state._tick % MaxBufferedTick;
        _moveStateHistory._buffer[historyIndex] = state;
    }
    public void CashEvent(IMoveEvent inputEvent)
    {
        int tick = inputEvent._tick;
        int historyIndex = tick % MaxBufferedTick;
        _moveEventHistory._buffer[historyIndex].Add(inputEvent);
    }
    private void OnMoveInputChanged(Vector3 val)
    {
        if (!IsSpawned) return;
        var moveEvent = new MoveInputEvent();
        moveEvent._tick = NetworkManager.NetworkTickSystem.LocalTime.Tick + 1;
        moveEvent._moveInput = val;
        moveEvent._isClientPredicted = false;
        CashEvent(moveEvent);
    }
    private void OnMouseInputChanged(Vector2 val)
    {
        if (!IsSpawned) return;
        var moveEvent = new MouseInputEvent();
        moveEvent._tick = NetworkManager.NetworkTickSystem.LocalTime.Tick + 1;
        moveEvent._isClientPredicted = false;
        moveEvent._mouseInput = val;
        CashEvent(moveEvent);
    }
}
