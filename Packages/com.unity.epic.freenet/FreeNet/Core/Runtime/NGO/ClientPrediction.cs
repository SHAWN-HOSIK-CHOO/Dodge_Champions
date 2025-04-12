using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static PlayerMoveControl;
using static UnityEngine.Tilemaps.Tilemap;
public interface ITickState
{
    int _tick { get; set; }
    bool _isPredict { get; set; }
    bool CheckStateDirty(ITickState compareState);
    ITickState DeepCopy();
    public void Serelize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
}
public interface ITickEvent
{
    int _tick { get; set; }
    bool _isPredict { get; set; }
    ITickEvent DeepCopy();
    bool CheckEventDirty(ITickEvent compare);
    public void Serelize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
}

public abstract class ClientPrediction : NetworkBehaviour
{
    public interface ITickStateMessage : INetworkSerializable
    {
        public ITickState _tickState { get; set; }
    }
    public interface ITickEventListMessage : INetworkSerializable
    {
        public ITickEvent[] _eventList { get; set; }
        public int _tick { get; set; }
    }
    protected List<ITickEventListMessage> _reconTickEventList;
    protected List<ITickStateMessage> _reconTickStateList;
    protected ArrayBuffer<ITickState> _tickStateHistory;
    protected ArrayBuffer<LinkedList<ITickEvent>> _tickEventHistory;

    [SerializeField]
    protected int SyncTickRate = 30;
    [SerializeField]
    protected int EventTickRate = 60;
    [SerializeField]
    protected float ConfirmTimeAgo = 0.3f;



    protected int _lastSyncTick;
    protected int _lastEventTick;

    protected int _lastNetSyncTick;
    protected int _lastNetEventTick;

    virtual protected void Update()
    {
        if (IsSpawned)
        {
            var time = NetworkManager.NetworkTickSystem.Time;
            var eventTime = new NetworkTime((uint)EventTickRate, time.Time);
            var syncTime = new NetworkTime((uint)SyncTickRate, time.Time);
            Reconciliation(time.Time);

            for (int eventTick = _lastEventTick+1; eventTick < eventTime.Tick; eventTick++)
            {
                if (eventTick < 0) continue;

                var eventTickTime = new NetworkTime((uint)EventTickRate, eventTick);
                var syncTickTime = new NetworkTime((uint)SyncTickRate, eventTickTime.Time);
                Simulate(syncTickTime.Tick, eventTickTime.Tick);
                _lastEventTick = eventTick;
                _lastSyncTick = syncTickTime.Tick;
            }


            if(_lastNetEventTick < eventTime.Tick)
            {
                OnEventTick(eventTime.Tick);
                _lastNetEventTick = eventTime.Tick;
            }

            var confirmTime =  syncTime  - new NetworkTime((uint)SyncTickRate, ConfirmTimeAgo);
            for (int i = _lastNetSyncTick + 1; i <= confirmTime.Tick; i++)
            {
                if (i < 0) continue;
                OnSyncTick(i);
                _lastNetSyncTick = i;
            }
        }
    }
    virtual protected void OnEventTick(int tick)
    {
        if (IsOwner)
        {
            ClearOldEvent(tick);
            SendTickEventList(tick);
        }
    }
    virtual protected void OnSyncTick(int tick)
    {
        if (IsServer)
        {
            int historyIndex = tick % SyncTickRate;
            _tickStateHistory._buffer[historyIndex]._isPredict = false;
            SendTickState(tick);
        }
    }
    virtual protected void SendTickEventList(int tick) { }
    virtual protected void SendTickState(int tick) { }
    virtual protected void ApplyTickEvent(int tick) { }
    virtual protected void SetTickStateFromHistory(int tick) { }
    virtual protected void CashCurrentState(int tick) { }
    override public void OnNetworkDespawn()
    { 
    }
    override public void OnNetworkSpawn()
    {
        _tickStateHistory = new ArrayBuffer<ITickState>((int)SyncTickRate);
        _tickEventHistory = new ArrayBuffer<LinkedList<ITickEvent>>((int)EventTickRate);

        _reconTickEventList = new List<ITickEventListMessage>();
        _reconTickStateList = new List<ITickStateMessage>();
        for (int i = 0; i < EventTickRate; i++)
        {
            _tickEventHistory._buffer[i] = new LinkedList<ITickEvent>();
        }

        var time = NetworkManager.NetworkTickSystem.Time;
        var eventTime = new NetworkTime((uint)EventTickRate, time.Time);
        var syncTime = new NetworkTime((uint)SyncTickRate, time.Time);

        _lastEventTick = eventTime.Tick;
        _lastNetEventTick = eventTime.Tick;
        _lastSyncTick = syncTime.Tick;
        _lastNetSyncTick = syncTime.Tick;

    }
    void ClearOldEvent(int tick)
    {
        int historyIndex = tick % EventTickRate;
        var node = _tickEventHistory._buffer[historyIndex].First;
        while (node != null)
        {
            var moveEvent = node.Value;
            if ((moveEvent._tick != tick) || moveEvent._isPredict)
            {
                var nextNode = node.Next;
                _tickEventHistory._buffer[historyIndex].Remove(node);
                node = nextNode;
            }
            else
            {
                node = node.Next;
            }
        }
    }
    protected void CashState(ITickState state)
    {
        int historyIndex = state._tick % SyncTickRate;
        _tickStateHistory._buffer[historyIndex] = state;
    }
    protected void CashEvent(ITickEvent tickEvent)
    {
        int historyIndex = tickEvent._tick % EventTickRate;
        _tickEventHistory._buffer[historyIndex].AddLast(tickEvent);
    }
    int ReconciliateState(int tick)
    {
        int reconTick = -1;
        for (int i = 0; i < _reconTickStateList.Count; i++)
        {
            var tickState = _reconTickStateList[i]._tickState;
            int historyIndex = tickState._tick % SyncTickRate;
            bool dirty = false;
            if (_reconTickStateList[i]._tickState._tick <= tick)
            {
                dirty = tickState.CheckStateDirty(_tickStateHistory._buffer[historyIndex]);
            }
            CashState(tickState);
            if (dirty)
            {
                if (reconTick == -1)
                {
                    reconTick = _reconTickStateList[i]._tickState._tick;
                }
            }
        }
        _reconTickStateList.Clear();
        return reconTick;
    }
    int ReconciliateEvent(int tick)
    {
        int reconTick = -1;
        for (int i = 0; i < _reconTickEventList.Count; i++)
        {
            bool dirty = false;
            int historyIndex = _reconTickEventList[i]._tick % EventTickRate;
            ClearOldEvent(_reconTickEventList[i]._tick);
            if (_reconTickEventList[i]._tick <= tick)
            {
                if (_tickEventHistory._buffer[historyIndex].Count != _reconTickEventList[i]._eventList.Length)
                {
                    dirty = true;
                }
                else
                {
                    int j = 0;
                    foreach (var moveEvent in _tickEventHistory._buffer[historyIndex])
                    {
                        if (moveEvent.CheckEventDirty(_reconTickEventList[i]._eventList[j]))
                        {
                            dirty = true;
                            break;
                        }
                        j++;
                    }
                }
            }
            foreach (var item in _reconTickEventList[i]._eventList)
            {
                _tickEventHistory._buffer[historyIndex].AddLast(item);
            }
            if (dirty)
            {
                if (reconTick == -1)
                {
                    reconTick = _reconTickEventList[i]._tick;
                }
            }
        }
        _reconTickEventList.Clear();
        return reconTick;
    }
    protected bool CheckTickStateDirty(int tick)
    {
        int historyIndex = tick % SyncTickRate;
        if (_tickStateHistory._buffer[historyIndex] == null) return true;
        bool tickDirty = _tickStateHistory._buffer[historyIndex]._tick != tick;
        if (tickDirty) return true;
        else return _tickStateHistory._buffer[historyIndex]._isPredict;
    }
    void ApplyTickState(int tick)
    {
        if (CheckTickStateDirty(tick))
        {
            CashCurrentState(tick);
        }
        else
        {
            SetTickStateFromHistory(tick);
        }
    }


    protected virtual void Simulate(int syncTick, int eventTick)
    {
        ApplyTickState(syncTick);
        if (CheckTickStateDirty(syncTick + 1))
        {
            ClearOldEvent(eventTick);
            ApplyTickEvent(eventTick);
        }
        ApplyTickState(syncTick + 1);
    }

    protected void Reconciliation(double time)
    {
        var eventTime = new NetworkTime((uint)EventTickRate, time);
        var syncTime = new NetworkTime((uint)SyncTickRate, time);

        int moveEventDirtTick = ReconciliateEvent(eventTime.Tick);
        int moveStateDirtTick = ReconciliateState(syncTime.Tick);

        var DirtyEventTime = new NetworkTime((uint)EventTickRate, moveEventDirtTick);
        var DirtyEventTimeToSyncTime = new NetworkTime((uint)SyncTickRate, DirtyEventTime.Time);
        var DirtySyncTime = new NetworkTime((uint)SyncTickRate, moveStateDirtTick);

 
        var FinalDirtySyncTime = (DirtyEventTimeToSyncTime.Time < DirtySyncTime.Time) ? DirtyEventTimeToSyncTime : DirtySyncTime;
        int startTick = FinalDirtySyncTime.Tick;
        if (startTick < 0) return;
        for (int i = startTick; i <= _lastSyncTick; i++)
        {
            double stateEndTime = new NetworkTime((uint)SyncTickRate, _lastSyncTick).Time;
            int eventTickStart = new NetworkTime((uint)EventTickRate, FinalDirtySyncTime.Time).Tick;
            int eventTickEnd = new NetworkTime((uint)EventTickRate, stateEndTime).Tick;
            for (int eventTick = eventTickStart; eventTick < eventTickEnd; eventTick++)
            {
                Simulate(i,eventTick);
            }
        }
    }
}
