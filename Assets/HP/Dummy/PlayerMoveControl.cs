using HP;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static PlayerMoveControl.MouseInputEvent;
using static PlayerMoveControl.MoveInputEvent;

public class PlayerMoveControl : NetworkBehaviour
{
    CharacterController _characterController;
    WASD_MouseBinding _WASD_MouseBinding;
    public struct TickMoveState : INetworkSerializable
    {
        public int tick;
        public Vector3 worldPos;
        public Quaternion worldRot;

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref worldPos);
            serializer.SerializeValue(ref worldRot);
        }
    }
    public interface MoveEvent
    {
        public enum MoveEventType
        {
            MoveInputEvent,
            MouseInputEvent
        }
        public float GetTime();
        public MoveEventType _eventType { get; }
    }
    public struct MoveInputEvent : MoveEvent
    {
        public MoveInputEventMessage _message;
        public float GetTime() => _message.time;
        public MoveEvent.MoveEventType _eventType => MoveEvent.MoveEventType.MoveInputEvent;

        public struct MoveInputEventMessage : INetworkSerializable
        {
            public float time;
            public Vector3 moveInput;

            void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
            {
                serializer.SerializeValue(ref time);
                serializer.SerializeValue(ref moveInput);
            }
        }
    }
    public struct MouseInputEvent : MoveEvent
    {
        public MouseInputEventMessage _message;
        public float GetTime() => _message.time;
        public MoveEvent.MoveEventType _eventType => MoveEvent.MoveEventType.MouseInputEvent;
        
        public struct MouseInputEventMessage : INetworkSerializable
        {
            public float time;
            public Vector2 mouseInput;
            void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
            {
                serializer.SerializeValue(ref time);
                serializer.SerializeValue(ref mouseInput);
            }
        }
    }
    ArrayBuffer<TickMoveState> _stateHistory;
    ArrayBuffer<Dictionary<MoveEvent.MoveEventType, List<MoveEvent>>> _eventHistory;
    const int MaxBufferedTick = 20;

    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
    public override void OnNetworkSpawn()
    {
        _characterController = GetComponent<CharacterController>();
        if (IsOwner)
        {
            _WASD_MouseBinding = new WASD_MouseBinding();
            _WASD_MouseBinding.Enable(true);
            _WASD_MouseBinding._onMouseInputChanged += OnMouseInputChanged;
            _WASD_MouseBinding._onMoveInputChanged += OnMoveInputChanged;
        }
        _stateHistory = new ArrayBuffer<TickMoveState>(MaxBufferedTick);
        _eventHistory = new ArrayBuffer<Dictionary<MoveEvent.MoveEventType, List<MoveEvent>>>(MaxBufferedTick);

        for (int i = 0; i < MaxBufferedTick; i ++)
        {
            _eventHistory._buffer[i] = new Dictionary<MoveEvent.MoveEventType, List<MoveEvent>>();
            _eventHistory._buffer[i][MoveEvent.MoveEventType.MoveInputEvent] = new List<MoveEvent>();
            _eventHistory._buffer[i][MoveEvent.MoveEventType.MouseInputEvent] = new List<MoveEvent>();
        }

        NetworkManager.NetworkTickSystem.Tick += OnTick;
    }
    bool CheckTickValid(bool checkLocal)
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
    public void SendMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        List<MouseInputEventMessage> MouseInputEventMessageList = new List<MouseInputEventMessage>();
        foreach (var item in _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MouseInputEvent].ToArray())
        {
            MouseInputEvent mouseEvent = (MouseInputEvent)item;
            var eventTick = new NetworkTime(NetworkManager.NetworkConfig.TickRate, mouseEvent.GetTime()).Tick;
            if (eventTick != tick)
            {
                _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MouseInputEvent].Remove(item);
                continue;
            }
            MouseInputEventMessageList.Add(mouseEvent._message);
        }
        if (MouseInputEventMessageList.Count != 0)
        {
            SendMouseInputEventRpc(MouseInputEventMessageList.ToArray());
        }
        List<MoveInputEventMessage> MoveInputEventMessageList = new List<MoveInputEventMessage>();
        foreach (var item in _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent].ToArray())
        {
            MoveInputEvent moveEvent = (MoveInputEvent)item;
            var eventTick = new NetworkTime(NetworkManager.NetworkConfig.TickRate, moveEvent.GetTime()).Tick;
            if (eventTick != tick)
            {
                _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent].Remove(item);
                continue;
            }
            MoveInputEventMessageList.Add(moveEvent._message);

            Debug.Log(moveEvent._message.moveInput);

        }
        if (MoveInputEventMessageList.Count != 0)
        {
            SendMoveInputEventRpc(MoveInputEventMessageList.ToArray());
        }
    }
    void ApplyMoveEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;

        if (_eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MouseInputEvent].Count != 0)
        {
            var lastMoveEvent = _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MouseInputEvent].Last();
            var lastEventTick = (new NetworkTime(NetworkManager.NetworkConfig.TickRate, lastMoveEvent.GetTime())).Tick;
            if (lastEventTick != tick)
            {
                _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MouseInputEvent].Clear();
            }
        }

        if (_eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent].Count != 0)
        {
            var lastMoveEvent = _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent].Last();
            var lastEventTick = (new NetworkTime(NetworkManager.NetworkConfig.TickRate, lastMoveEvent.GetTime())).Tick;
            if (lastEventTick != tick)
            {
                _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent].Clear();
            }
        }

        if (_eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent].Count == 0)
        {
            int prevhistoryIndex = (tick - 1) % MaxBufferedTick;
            if(_eventHistory._buffer[prevhistoryIndex][MoveEvent.MoveEventType.MoveInputEvent].Count == 0)
            {
                var prevMoveEvent = new MoveInputEvent();
                prevMoveEvent._message.time = new NetworkTime(NetworkManager.NetworkConfig.TickRate, tick, 0.005f).TimeAsFloat;
                prevMoveEvent._message.moveInput = Vector3.zero;
                CashEvent(prevMoveEvent);
            }
            else
            {
                var prevMoveEvent = (MoveInputEvent)_eventHistory._buffer[prevhistoryIndex][MoveEvent.MoveEventType.MoveInputEvent].Last();
                prevMoveEvent._message.time = new NetworkTime(NetworkManager.NetworkConfig.TickRate, tick, 0.005f).TimeAsFloat;
                CashEvent(prevMoveEvent);
            }
        }
        ApplyMoveEvent(tick, _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MoveInputEvent], _eventHistory._buffer[historyIndex][MoveEvent.MoveEventType.MouseInputEvent]);
    }
    public void OnTick()
    {
        if(!IsServer && !CheckTickValid(true)) return;
        if(IsServer && !CheckTickValid(false)) return;
        int localTick = NetworkManager.NetworkTickSystem.LocalTime.Tick;
        int serverTick = NetworkManager.NetworkTickSystem.ServerTime.Tick;
        if (IsOwner)
        {
            if (!CheckTickValid(true)) return;
            CashState(localTick);
            SendMoveEvent(localTick);
            ApplyMoveEvent(localTick);
        }
        else 
        {
            CashState(serverTick);
            ApplyMoveEvent(serverTick);
        }
        if(IsServer)
        {
            int historyIndex = serverTick % MaxBufferedTick;
            SendMoveStateRpc(_stateHistory._buffer[historyIndex]);
        }
    }

    [Rpc(SendTo.NotServer)]
    void SendMoveStateRpc(TickMoveState moveState)
    {
        int historyIndex = moveState.tick % MaxBufferedTick;
        float positionThreshold = 0.001f;
        float rotationThreshold = 0.001f;

        bool worldPosDirty = Vector3.Distance(_stateHistory._buffer[historyIndex].worldPos, moveState.worldPos) > positionThreshold;
        bool worldRotDirty = Quaternion.Dot(_stateHistory._buffer[historyIndex].worldRot, moveState.worldRot) < (1.0f - rotationThreshold);

        if (worldPosDirty || worldRotDirty || (moveState.tick != _stateHistory._buffer[historyIndex].tick))
        {
            _characterController.detectCollisions = false;
            _stateHistory._buffer[historyIndex] = moveState;
            for (int i = moveState.tick; i <= NetworkManager.NetworkTickSystem._tickUpdateInfo._endLocalTick; i++)
            {
                if (i == moveState.tick)
                {
                    _characterController.enabled = false;
                    transform.position = moveState.worldPos;
                    transform.rotation = moveState.worldRot;
                    _characterController.enabled = true;
                }
                CashState(i);
                ApplyMoveEvent(i);
            }
            _characterController.detectCollisions = true;
        }
    }
    [Rpc(SendTo.NotMe)]
    void SendMoveInputEventRpc(MoveInputEvent.MoveInputEventMessage[] moveEventList)
    {
        foreach (var moveEvent in moveEventList)
        {
            var moveInputEvent = new MoveInputEvent();
            moveInputEvent._message = moveEvent;
            CashEvent(moveInputEvent);
        }
    }
    [Rpc(SendTo.NotMe)]
    void SendMouseInputEventRpc(MouseInputEvent.MouseInputEventMessage[] mouseEventList)
    {
        foreach (var mouseEvent in mouseEventList)
        {
            var mouseInputEvent = new MouseInputEvent();
            mouseInputEvent._message = mouseEvent;
            CashEvent(mouseInputEvent);
        }
    }
    void ApplyMoveEvent(int tick ,List<MoveEvent> moveEventList, List<MoveEvent> mouseEventList)
    {
        var mergedList = new List<MoveEvent>();
        mergedList.AddRange(moveEventList);
        mergedList.AddRange(mouseEventList);
        var sortedList = mergedList.OrderBy(e => e.GetTime()).ToList();
        foreach (var inputEvent in sortedList)
        {
            if (inputEvent._eventType == MoveEvent.MoveEventType.MouseInputEvent)
            {
                var message = (MouseInputEvent)inputEvent;
                var eventTick = new NetworkTime(NetworkManager.NetworkConfig.TickRate, message.GetTime()).Tick;
                Vector2 mouseInput = message._message.mouseInput;
                Quaternion yaw = Quaternion.AngleAxis(mouseInput.x * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime, Vector3.up);
                Quaternion pitch = Quaternion.AngleAxis(-mouseInput.y * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime, Vector3.right);
                _characterController.transform.rotation = yaw * _characterController.transform.rotation * pitch;
            }
            else if (inputEvent._eventType == MoveEvent.MoveEventType.MoveInputEvent)
            {
                var message = (MoveInputEvent)inputEvent;
                var eventTick = new NetworkTime(NetworkManager.NetworkConfig.TickRate, message.GetTime()).Tick;
                Vector3 moveInput = message._message.moveInput;
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
                moveDirection = _characterController.transform.TransformDirection(moveDirection);
                _characterController.Move(moveDirection * 10 * NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime);
            }
        }
    }
    public void CashState(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        _stateHistory._buffer[historyIndex].tick = tick;
        _stateHistory._buffer[historyIndex].worldPos = transform.position;
        _stateHistory._buffer[historyIndex].worldRot = transform.rotation;
    }
    public void CashEvent(MoveEvent inputEvent)
    {
        int tick = new NetworkTime(NetworkManager.NetworkConfig.TickRate, inputEvent.GetTime()).Tick;
        int historyIndex = tick % MaxBufferedTick;
        if(_eventHistory._buffer[historyIndex][inputEvent._eventType].Count != 0)
        {
            var lastMoveEvent = _eventHistory._buffer[historyIndex][inputEvent._eventType].Last();
            var lastEventTick = (new NetworkTime (NetworkManager.NetworkConfig.TickRate, lastMoveEvent.GetTime())).Tick;
            if(lastEventTick != tick)
            {
                _eventHistory._buffer[historyIndex][inputEvent._eventType].Clear();
            }
        }
        _eventHistory._buffer[historyIndex][inputEvent._eventType].Add(inputEvent);
    }
    private void OnMoveInputChanged(Vector3 val)
    {
        var moveEvent = new MoveInputEvent();
        moveEvent._message.time = (NetworkManager.NetworkTickSystem.LocalTime + NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime+0.005f).TimeAsFloat;
        moveEvent._message.moveInput = val;
        CashEvent(moveEvent);
    }
    private void OnMouseInputChanged(Vector2 val)
    {
        var moveEvent = new MouseInputEvent();
        moveEvent._message.time = (NetworkManager.NetworkTickSystem.LocalTime + NetworkManager.NetworkTickSystem.LocalTime.FixedDeltaTime + 0.005f).TimeAsFloat;
        moveEvent._message.mouseInput = val;
        CashEvent(moveEvent);
    }
}
