using HP;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMoveControl : NetworkBehaviour
{
    /*
     * 틱 단위 동기화
     * TO Do  한 틱에 Input값을 오버라이드 하고 있는데 이는 키 씹힘으로 여겨질수 있음. 문제시 큐로 대체 
     * 
     */


    CharacterController _characterController;
    WASD_MouseBinding _WASD_MouseBinding;
    const int MAX_TICKS = 100;

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
    public struct TickMoveEvent : INetworkSerializable
    {
        public int tick;
        public Vector3 moveInput;
        public bool hasMoveInput;

        public Vector2 mouseInput;
        public bool hasMouseInput;

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref hasMoveInput);
            if (hasMoveInput)
            {
                serializer.SerializeValue(ref moveInput);
            }

            serializer.SerializeValue(ref hasMouseInput);
            if (hasMouseInput)
            {
                serializer.SerializeValue(ref mouseInput);
            }
        }
    }
    TickMoveState[] _stateHistory;
    TickMoveEvent[] _eventHistory;

    bool _hasMoveInputInTick;
    Vector3 _moveInputInTick;
    bool _hasMouseInputInTick;
    Vector2 _mouseInputInTick;

    public override void OnDestroy()
    {
        
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
        _stateHistory = new TickMoveState[MAX_TICKS];
        _eventHistory = new TickMoveEvent[MAX_TICKS];
        NetworkManager.Singleton.NetworkTickSystem.Tick += OnNetworkUpdate;
        _hasMoveInputInTick = false;
        _hasMouseInputInTick = false;
    }

    public void OnNetworkUpdate()
    {
        if(IsOwner)
        {
            int localTick = NetworkManager.Singleton.NetworkTickSystem.LocalTime.Tick;
            int historyIndex = localTick % MAX_TICKS;
            ResetEvent(localTick, historyIndex);
            CashState(localTick);
            if (IsOwner)
            {
                if (_hasMouseInputInTick)
                {
                    _eventHistory[historyIndex].hasMouseInput = true;
                    _eventHistory[historyIndex].mouseInput = _mouseInputInTick;
                }
                if (_hasMoveInputInTick)
                {
                    _eventHistory[historyIndex].hasMoveInput = true;
                    _eventHistory[historyIndex].moveInput = _moveInputInTick;
                }
                SendMoveEventRpc(_eventHistory[historyIndex]);
                _hasMoveInputInTick = false;
                _hasMouseInputInTick = false;
            }
            ApplyEvent(localTick);
        }
        else
        {
            int serverTick = NetworkManager.Singleton.NetworkTickSystem.ServerTime.Tick;
            int historyIndex = serverTick % MAX_TICKS;
            ResetEvent(serverTick, historyIndex);
            CashState(serverTick);
            ApplyEvent(serverTick);
        }

        if (IsServer)
        {
            int serverTick = NetworkManager.Singleton.NetworkTickSystem.ServerTime.Tick;
            int historyIndex = serverTick % MAX_TICKS;
            SendMoveStateRpc(_stateHistory[historyIndex]);
        }
    }

    void Reconciliation(int historyIndex)
    {
        // 위치를 재 시뮬레이션 후 보정해야함...


    }
    void ResetEvent(int localTick,int historyIndex)
    {
        if(_eventHistory[historyIndex].tick != localTick)
        {
            _eventHistory[historyIndex].hasMoveInput = false;
            _eventHistory[historyIndex].hasMouseInput = false;
        }
        _eventHistory[historyIndex].tick = localTick;
    }

    [Rpc(SendTo.NotServer)]
    void SendMoveStateRpc(TickMoveState moveState)
    {
        int index = moveState.tick % MAX_TICKS;
        if(_stateHistory[index].worldRot != moveState.worldRot ||
            _stateHistory[index].worldPos != moveState.worldPos)
        {
            _stateHistory[index] = moveState;
            Reconciliation(index);
        }


    }


    void ApplyEvent(int tick)
    {
        Vector3 moveInput = Vector3.zero;
        Vector2 mouseDeltaInput = Vector2.zero;
        int historyIndex = tick % MAX_TICKS;
        if (_eventHistory[historyIndex].hasMouseInput)
        {
            mouseDeltaInput = _eventHistory[historyIndex].mouseInput;
        }

        for (int i = 0; i < MAX_TICKS; i++)
        {
            historyIndex = (MAX_TICKS + tick - i) % MAX_TICKS;
            if (_eventHistory[historyIndex].hasMoveInput)
            {
                moveInput = _eventHistory[historyIndex].moveInput;
                break;
            }
        }
        _characterController.Move( 20 * moveInput * NetworkManager.Singleton.NetworkTickSystem.LocalTime.FixedDeltaTime);
    }

    [Rpc(SendTo.NotMe)]
    void SendMoveEventRpc(TickMoveEvent moveEvent)
    {
        int index = moveEvent.tick % MAX_TICKS;
        _eventHistory[index] = moveEvent;
    }


    void CashState(int localTick)
    {
        int index = localTick % MAX_TICKS;
        _stateHistory[index].worldPos = transform.position;
        _stateHistory[index].worldRot = transform.rotation;
    }

    private void OnMoveInputChanged(Vector3 val)
    {
        _hasMoveInputInTick = true;
        _moveInputInTick = val;
    }
    private void OnMouseInputChanged(Vector2 val)
    {
        _hasMouseInputInTick = true;
        _mouseInputInTick = val;
    }
}
