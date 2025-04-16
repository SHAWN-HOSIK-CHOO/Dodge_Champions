using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
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
    protected int SyncTickRate = 5;
    protected int EventTickRate; // SyncTickRate보다 높을 것
    protected int SyncTickBufferSize; // rtt * SyncTickRate 보다 높을 것
    protected int EventTickBufferSize;  // rtt * EventTickRate 보다 높을 것
    protected float ConfirmTimeAgo; //   (BufferSize / SyncTickRate) - rtt 보다 작을 것

    protected int _lastSyncTick;
    protected int _lastNetSyncTick;

    protected int _lastEventTick;

    virtual protected void Update()
    {
        if (IsSpawned)
        {
            var time = NetworkManager.NetworkTickSystem.Time;
            var eventTime = new NetworkTime((uint)EventTickRate, time.Time);
            var syncTime = new NetworkTime((uint)SyncTickRate, time.Time);
            for (int eventTick = _lastEventTick + 1; eventTick <= eventTime.Tick; eventTick++)
            {
                if (eventTick < 0) continue;
                var eventTickTime = new NetworkTime((uint)EventTickRate, eventTick);
                var syncTickTime = new NetworkTime((uint)SyncTickRate, eventTickTime.Time);
                Reconciliation(eventTickTime.Time);
                OnEventTick(eventTickTime);
                _lastEventTick = eventTick;
                _lastSyncTick = syncTickTime.Tick;
            }

            var confirmTime = syncTime - new NetworkTime((uint)SyncTickRate, ConfirmTimeAgo);
            for (int i = _lastNetSyncTick + 1; i <= confirmTime.Tick; i++)
            {
                if (i > 0)
                {
                    OnNetSyncTick(i);
                    _lastNetSyncTick = i;
                }
            }
        }
    }
    virtual protected void OnEventTick(NetworkTime eventTime)
    {
        Simulate(eventTime, false);
        if (IsOwner)
        {
            ClearOldEvent(eventTime.Tick);
            SendTickEventList(eventTime.Tick);
        }
    }
    virtual protected void OnNetSyncTick(int tick)
    {
        if (IsServer)
        {
            int historyIndex = tick % SyncTickBufferSize;
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
        float rtt = 0.25f;
        SyncTickRate = 5;
        EventTickRate = (SyncTickRate + 10 < 60) ? 60 : SyncTickRate + 10;
        SyncTickBufferSize = SyncTickRate + (int)Math.Ceiling(SyncTickRate * rtt);
        EventTickBufferSize = EventTickRate + (int)Math.Ceiling(EventTickRate * rtt);
        ConfirmTimeAgo = (int)Math.Floor((SyncTickBufferSize / SyncTickRate) - rtt);

        _tickStateHistory = new ArrayBuffer<ITickState>((int)SyncTickBufferSize);
        _tickEventHistory = new ArrayBuffer<LinkedList<ITickEvent>>((int)EventTickBufferSize);

        _reconTickEventList = new List<ITickEventListMessage>();
        _reconTickStateList = new List<ITickStateMessage>();
        for (int i = 0; i < EventTickBufferSize; i++)
        {
            _tickEventHistory._buffer[i] = new LinkedList<ITickEvent>();
        }

        var time = NetworkManager.NetworkTickSystem.Time;
        var eventTime = new NetworkTime((uint)EventTickRate, time.Time);
        var syncTime = new NetworkTime((uint)SyncTickRate, time.Time);

        _lastEventTick = eventTime.Tick;
        _lastSyncTick = syncTime.Tick;
        _lastNetSyncTick = syncTime.Tick;

    }
    protected void ClearOldEvent(int tick)
    {
        int historyIndex = tick % EventTickBufferSize;
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
        int historyIndex = state._tick % SyncTickBufferSize;
        _tickStateHistory._buffer[historyIndex] = state;
    }
    protected void CashEvent(ITickEvent tickEvent)
    {
        int historyIndex = tickEvent._tick % EventTickBufferSize;
        _tickEventHistory._buffer[historyIndex].AddLast(tickEvent);
    }
    int ReconciliateState(int tick)
    {
        int reconTick = -1;
        for (int i = 0; i < _reconTickStateList.Count; i++)
        {
            var reconTickState = _reconTickStateList[i]._tickState;
            int historyIndex = reconTickState._tick % SyncTickBufferSize;
            bool dirty = false;
            if (reconTickState._tick <= tick)
            {
                dirty = (_tickStateHistory._buffer[historyIndex] == null) ? true : _tickStateHistory._buffer[historyIndex].CheckStateDirty(reconTickState);
            }
            CashState(reconTickState);

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
            int historyIndex = _reconTickEventList[i]._tick % EventTickBufferSize;
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
        int historyIndex = tick % SyncTickBufferSize;
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

    protected virtual void Simulate(int SyncTick)
    {
        if (!CheckTickStateDirty(SyncTick + 1))
        {
            SetTickStateFromHistory(SyncTick + 1);
            return;
        }

        NetworkTime syncStartTime = new NetworkTime((uint)SyncTickRate, SyncTick);
        NetworkTime syncEndTime = new NetworkTime((uint)EventTickRate, SyncTick + 1);
        NetworkTime eventStartTime = new NetworkTime((uint)EventTickRate, syncStartTime.Time);
        NetworkTime eventEndTime = new NetworkTime((uint)EventTickRate, syncEndTime.Time);

        int endTick = eventEndTime.Tick;
        if (eventEndTime.FixedTime == syncEndTime.FixedTime)
        {
            endTick -= 1;
        }

        SetTickStateFromHistory(SyncTick);
        for (int eventTick = eventStartTime.Tick; eventTick <= endTick; eventTick++)
        {
            ClearOldEvent(eventTick);
            ApplyTickEvent(eventTick);
        }
        ApplyTickState(SyncTick + 1);
    }
    protected virtual void Simulate(NetworkTime eventTime , bool recon)
    {
        NetworkTime syncTime = new NetworkTime((uint)SyncTickRate, eventTime.Time);
        NetworkTime eventStartTime = new NetworkTime((uint)EventTickRate, syncTime.FixedTime);
        SetTickStateFromHistory(syncTime.Tick);
        for (int eventTick = eventStartTime.Tick; eventTick <= eventTime.Tick; eventTick++)
        {
            ClearOldEvent(eventTick);
            ApplyTickEvent(eventTick);
        }
        ApplyTickState(syncTime.Tick + 1);
    }

    protected void Reconciliation(double time)
    {
        var eventTime = new NetworkTime((uint)EventTickRate, time);
        var syncTime = new NetworkTime((uint)SyncTickRate, time);

        int moveEventDirtTick = ReconciliateEvent(eventTime.Tick);
        int moveStateDirtTick = ReconciliateState(syncTime.Tick);

        var DirtyEventTime = new NetworkTime((uint)EventTickRate, moveEventDirtTick);
        var DirtySyncTime = new NetworkTime((uint)SyncTickRate, moveStateDirtTick);
        var DirtySyncTimeToEventTime = new NetworkTime((uint)EventTickRate, DirtySyncTime.Time);
        int startTick;
        if (DirtyEventTime.Tick < 0 && DirtySyncTimeToEventTime.Tick < 0)
        {
            return;
        }
        else if (DirtyEventTime.Tick < 0)
        {
            startTick = DirtySyncTimeToEventTime.Tick;
        }
        else if (DirtySyncTimeToEventTime.Tick < 0)
        {
            startTick = DirtyEventTime.Tick;
        }
        else
        {
            startTick = (DirtyEventTime.Tick < DirtySyncTimeToEventTime.Tick) ? DirtyEventTime.Tick : DirtySyncTimeToEventTime.Tick;
        }


        for (int eventTick = startTick; eventTick <= _lastEventTick; eventTick++)
        {
            Simulate(new NetworkTime((uint)EventTickRate, eventTick), true);
        }
    }
}
