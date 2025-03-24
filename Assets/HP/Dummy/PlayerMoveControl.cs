using HP;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static PlayerMoveControl.MouseInputEvent;
using static PlayerMoveControl.IMoveEvent;
using static PlayerMoveControl.MoveInputEvent;
using System.Collections;
using static PlayerMoveControl;
using UnityEngine.UIElements;

public class PlayerMoveControl : NetworkBehaviour
{
    [SerializeField]
    CharacterController _characterController;
    WASD_MouseBinding _WASD_MouseBinding;
    public struct TickMoveState : INetworkSerializable
    {
        public int _tick;
        public Vector3 _worldPos;
        public Quaternion _worldRot;
        public bool _isClientPredicted;

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _tick);
            serializer.SerializeValue(ref _worldPos);
            serializer.SerializeValue(ref _worldRot);
        }
    }
    public struct MoveEventMessage : INetworkSerializable
    {
        public IMoveEvent[] eventList;
        public int tick;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
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
                            serializer.SerializeValue(ref mouseEvent.mouseInput);
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
                            serializer.SerializeValue(ref mouseEvent.mouseInput);
                            eventList[i] = mouseEvent;
                            break;
                    }
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
        public bool _isUsed { get; set;}
    }
    public struct MoveInputEvent : IMoveEvent
    {
        public int _tick;
        public Vector3 _moveInput;
        public bool _isUsed;
        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        bool IMoveEvent._isUsed { get => _isUsed; set => _isUsed = value; }
        public IMoveEvent.MoveEventType _eventType => IMoveEvent.MoveEventType.MoveInputEvent;

    }
    public struct MouseInputEvent : IMoveEvent
    {
        public Vector2 mouseInput;
        public bool _isUsed; 
        public int _tick;
        bool IMoveEvent._isUsed { get => _isUsed; set => _isUsed = value; }
        int IMoveEvent._tick { get => _tick; set => _tick = value; }
        public IMoveEvent.MoveEventType _eventType => IMoveEvent.MoveEventType.MouseInputEvent;
    }
    ArrayBuffer<TickMoveState> _moveStateHistory;
    ArrayBuffer<List<IMoveEvent>> _moveEventHistory;
    int MaxBufferedTick;
    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= OnTick;
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
        _moveStateHistory = new ArrayBuffer<TickMoveState>(MaxBufferedTick);
        _moveEventHistory = new ArrayBuffer<List<IMoveEvent>>(MaxBufferedTick);

        for (int i = 0; i < MaxBufferedTick; i ++)
        {
            _moveStateHistory._buffer[i]._isClientPredicted = true;
            _moveEventHistory._buffer[i] = new List<IMoveEvent>();
        }

        NetworkManager.NetworkTickSystem.Tick += OnTick;
    }
    bool CheckTickUpdateValid(bool checkLocal)
    {
        if (checkLocal)
        {
            int prevFrameUpdateLocalTick = NetworkManager.NetworkTickSystem._tickUpdateInfo._prevUpdateLocalTick;
            int curFrameUpdateLocalTick = NetworkManager.NetworkTickSystem._tickUpdateInfo._curUpdateLocalTick;
            if ((prevFrameUpdateLocalTick + 1) != curFrameUpdateLocalTick)
            {
                Debug.LogWarning($" Localtick Jumped: {prevFrameUpdateLocalTick} to {curFrameUpdateLocalTick}");
                return false;
            }
        }
        else
        {
            int prevFrameUpdateServerTick = NetworkManager.NetworkTickSystem._tickUpdateInfo._prevUpdateServerTick;
            int curFrameUpdateServerTick = NetworkManager.NetworkTickSystem._tickUpdateInfo._curUpdateServerTick;
            if ((prevFrameUpdateServerTick + 1) != curFrameUpdateServerTick)
            {
                Debug.LogWarning($" Servertick Jumped: {prevFrameUpdateServerTick} to {curFrameUpdateServerTick}");
                return false;
            }
        }
        return true;
    }
    void ClearOldMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        foreach(var moveEvent in _moveEventHistory._buffer[historyIndex].ToArray())
        {
            if (moveEvent._tick != tick)
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
        // 만약 이번 틱의 MoveInputEvent가 없다면 이전의 MoveInputEvent에서 가져온다
        var moveInputEvent = _moveEventHistory._buffer[historyIndex].FindLast(e => e._eventType == MoveEventType.MoveInputEvent);
        if (moveInputEvent == null)
        {
            int prevhistoryIndex = (tick + MaxBufferedTick - 1) % MaxBufferedTick;
            var prevEvent = _moveEventHistory._buffer[prevhistoryIndex].FindLast(e => e._eventType == MoveEventType.MoveInputEvent);
            if (prevEvent == null)
            {
                var prevMoveEvent = new MoveInputEvent();
                prevMoveEvent._tick = tick;
                prevMoveEvent._moveInput = Vector3.zero;
                prevMoveEvent._isUsed = false;
                _moveEventHistory._buffer[historyIndex].Insert(0, prevMoveEvent);
            }
            else
            {
                var prevMoveEvent = (MoveInputEvent)prevEvent;
                prevMoveEvent._tick = tick;
                prevMoveEvent._isUsed = false;
                _moveEventHistory._buffer[historyIndex].Insert(0, prevMoveEvent);
            }
        }
    }
    bool CheckCashMoveState (int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        if (_moveStateHistory._buffer[historyIndex]._tick != tick) return true;
        return _moveStateHistory._buffer[historyIndex]._isClientPredicted;
    }
    public void SendMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        if(_moveEventHistory._buffer[historyIndex].Count != 0)
        {
            var moveEventMessage = new MoveEventMessage();
            moveEventMessage.tick = tick;
            moveEventMessage.eventList = _moveEventHistory._buffer[historyIndex].ToArray();
            SendMoveEventRpc(moveEventMessage);
        }
    }
    public void ApplyMove(int tick)
    {
        if (CheckCashMoveState (tick))
        {
            CashState(new TickMoveState()
            {
                _tick = tick,
                _worldPos = transform.position,
                _worldRot = transform.rotation,
                _isClientPredicted = true
            });
        }
        else
        {
            SetMoveStateFromHistory(tick);
        }

        ClearOldMoveEvent(tick);
        AddPrevMoveInputEvent(tick);
        ApplyMoveEvent(tick);
    }
    public void OnTick()
    {
        if(!IsServer && !CheckTickUpdateValid(true)) return;
        if(IsServer && !CheckTickUpdateValid(false)) return;

        int localTick = NetworkManager.NetworkTickSystem.LocalTime.Tick;
        int serverTick = NetworkManager.NetworkTickSystem.ServerTime.Tick;
        if (IsOwner)
        {
            ApplyMove(localTick);
            SendMoveEvent(localTick);
        }
        else 
        {
            ApplyMove(serverTick);
        }

        if(IsServer)
        {
            int historyIndex = serverTick % MaxBufferedTick;
            SendMoveStateRpc(_moveStateHistory._buffer[historyIndex]);
        }
    }
    void SetMoveStateFromHistory(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        _characterController.detectCollisions = false;
        _characterController.enabled = false;
        transform.position = _moveStateHistory._buffer[historyIndex]._worldPos;
        transform.rotation = _moveStateHistory._buffer[historyIndex]._worldRot;
        _characterController.enabled = true;
        _characterController.detectCollisions = true;
    }
    bool CheckDirty(TickMoveState stateL , TickMoveState stateR)
    {
        float positionThreshold = 0.0001f;
        float rotationThreshold = 0.0001f;

        bool worldPosDirty = Vector3.Distance(stateL._worldPos, stateR._worldPos) > positionThreshold;
        bool worldRotDirty = Quaternion.Dot(stateL._worldRot, stateR._worldRot) < (1.0f - rotationThreshold);
        return (worldPosDirty || worldRotDirty || (stateL._tick != stateR._tick));
    }
    void Reconciliation(int tick, bool local)
    {
        var endTick = local ? NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick : NetworkManager.NetworkTickSystem._tickUpdateInfo._endServerTick;
        _characterController.detectCollisions = false;
        for (int i = tick; i <= endTick; i++)
        {
            ApplyMove(i);
        }
        _characterController.detectCollisions = true;
    }
    [Rpc(SendTo.NotServer)]
    void SendMoveStateRpc(TickMoveState moveState)
    {
        int historyIndex = moveState._tick % MaxBufferedTick;
        bool dirty = CheckDirty(_moveStateHistory._buffer[historyIndex], moveState);
        if (dirty)
        {
            moveState._isClientPredicted = false;
            CashState(moveState);
            var endTick = NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick;
            for (int i = moveState._tick +1; i <= NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick; i++)
            {
                historyIndex = i % MaxBufferedTick;
                _moveStateHistory._buffer[historyIndex]._isClientPredicted = true;
            }
            Reconciliation(moveState._tick, true);
        }
    }
    [Rpc(SendTo.NotMe)]
    void SendMoveEventRpc(MoveEventMessage messsage)
    {
        if(messsage.eventList.Length >0)
        {
            int historyIndex = messsage.tick % MaxBufferedTick;
            _moveEventHistory._buffer[historyIndex] = messsage.eventList.ToList();
            for (int i = messsage.tick; i <= NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick; i++)
            {
                historyIndex = i % MaxBufferedTick;
                _moveEventHistory._buffer[historyIndex].Clear();
            }
            Reconciliation(messsage.tick, true);
        }
    }
    void ApplyMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;  

        for (int i = 0; i < _moveEventHistory._buffer[historyIndex].Count; i++)
        {
            if (_moveEventHistory._buffer[historyIndex][i]._eventType == IMoveEvent.MoveEventType.MouseInputEvent)
            {
                _moveEventHistory._buffer[historyIndex][i]._isUsed = true;
                var message = (MouseInputEvent)_moveEventHistory._buffer[historyIndex][i];
                Vector2 mouseInput = message.mouseInput;
                Quaternion yaw = Quaternion.AngleAxis(mouseInput.x * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime, Vector3.up);
                Quaternion pitch = Quaternion.AngleAxis(-mouseInput.y * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime, Vector3.right);
                _characterController.transform.rotation = yaw * _characterController.transform.rotation * pitch;
            }
            else if (_moveEventHistory._buffer[historyIndex][i]._eventType == IMoveEvent.MoveEventType.MoveInputEvent)
            {
                _moveEventHistory._buffer[historyIndex][i]._isUsed = true;
                var message = (MoveInputEvent)_moveEventHistory._buffer[historyIndex][i];
                Vector3 moveInput = message._moveInput;
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                moveDirection = _characterController.transform.TransformDirection(moveDirection);
                _characterController.Move(moveDirection * 10 * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
            }
        }
    }
    public void CashState(TickMoveState state)
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
        var moveEvent = new MoveInputEvent();
        var time = new NetworkTime(NetworkManager.NetworkConfig.TickRate, NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick +1);
        moveEvent._tick = NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick + 1;
        moveEvent._moveInput = val;
        CashEvent(moveEvent);
    }
    private void OnMouseInputChanged(Vector2 val)
    {
        var moveEvent = new MouseInputEvent();
        moveEvent._tick = NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick + 1;
        moveEvent.mouseInput = val;
        CashEvent(moveEvent);
    }
}
