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

public class ClientPrediction : NetworkBehaviour
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
    protected ArrayBuffer<List<ITickEvent>> _tickEventHistory;

    [SerializeField]
    public int MaxBufferedTick = 30 * 10;
    virtual public void OnLocalTick()
    {
        int localTick = NetworkManager.NetworkTickSystem.LocalTime.Tick;
        if (IsOwner)
        {
            ClearOldEvent(localTick);
            SendTickEventList(localTick);
            Reconciliation(localTick);
        }
    }
    virtual public void OnServerTick()
    {
        int serverTick = NetworkManager.NetworkTickSystem.ServerTime.Tick;
        if (!IsOwner)
        {
            Reconciliation(serverTick);
        }
        if (IsServer)
        {
            int historyIndex = serverTick % MaxBufferedTick;
            _tickStateHistory._buffer[historyIndex]._isPredict = false;
            SendTickState(serverTick);
        }
    }
    virtual public void SendTickEventList(int tick) { }
    virtual public void SendTickState(int tick) { }
    virtual public void ApplyTickEvent(int tick) { }
    virtual public void SetTickStateFromHistory(int tick) { }
    virtual public void CashCurrentState(int tick) { }
    override public void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= OnLocalTick;
        NetworkManager.NetworkTickSystem.ServerTick -= OnServerTick;
    }
    override public void OnNetworkSpawn()
    {
        _tickStateHistory = new ArrayBuffer<ITickState>(MaxBufferedTick);
        _tickEventHistory = new ArrayBuffer<List<ITickEvent>>(MaxBufferedTick);

        _reconTickEventList = new List<ITickEventListMessage>();
        _reconTickStateList = new List<ITickStateMessage>();
        for (int i = 0; i < MaxBufferedTick; i++)
        {
            _tickEventHistory._buffer[i] = new List<ITickEvent>();
        }
        NetworkManager.NetworkTickSystem.Tick += OnLocalTick;
        NetworkManager.NetworkTickSystem.ServerTick += OnServerTick;
    }
    void ClearOldEvent(int tick)
    {
        int historyIndex = tick % MaxBufferedTick;
        foreach (var moveEvent in _tickEventHistory._buffer[historyIndex].ToArray())
        {
            if ((moveEvent._tick != tick) || moveEvent._isPredict)
            {
                _tickEventHistory._buffer[historyIndex].Remove(moveEvent);
            }
        }
    }
    protected void CashState(ITickState state)
    {
        int historyIndex = state._tick % MaxBufferedTick;
        _tickStateHistory._buffer[historyIndex] = state;
    }
    protected void CashEvent(ITickEvent tickEvent)
    {
        int historyIndex = tickEvent._tick % MaxBufferedTick;
        _tickEventHistory._buffer[historyIndex].Add(tickEvent);
    }
    int ReconciliateState(int tick)
    {
        int reconTick = -1;
        for (int i = 0; i < _reconTickStateList.Count; i++)
        {
            var tickState = _reconTickStateList[i]._tickState;
            int historyIndex = tickState._tick % MaxBufferedTick;
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
            int historyIndex = _reconTickEventList[i]._tick % MaxBufferedTick;
            ClearOldEvent(_reconTickEventList[i]._tick);
            if (_reconTickEventList[i]._tick <= tick)
            {
                if (_tickEventHistory._buffer[historyIndex].Count != _reconTickEventList[i]._eventList.Length)
                {
                    dirty = true;
                }
                else
                {
                    for (int j = 0; j < _reconTickEventList[i]._eventList.Length; j++)
                    {
                        if (_tickEventHistory._buffer[historyIndex][j].CheckEventDirty(_reconTickEventList[i]._eventList[j]))
                        {
                            dirty = true;
                            break;
                        }
                    }
                }
            }
            foreach (var item in _reconTickEventList[i]._eventList)
            {
                _tickEventHistory._buffer[historyIndex].Add(item);
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
        int historyIndex = tick % MaxBufferedTick;
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
    void Reconciliation(int tick)
    {
        int moveEventDirtTick = ReconciliateEvent(tick);
        int moveStateDirtTick = ReconciliateState(tick);

        int dirtyTick = tick;
        dirtyTick = (moveEventDirtTick == -1) ? dirtyTick : moveEventDirtTick;
        dirtyTick = (moveStateDirtTick == -1) ? dirtyTick : moveStateDirtTick;
        for (int i = dirtyTick; i <= tick; i++)
        {
            int historyIndex = i % MaxBufferedTick;
            ApplyTickState(i);
            ClearOldEvent(i);
            if (CheckTickStateDirty(i + 1))
            {
                ApplyTickEvent(i);
            }
            ApplyTickState(i + 1);
        }
    }
}
