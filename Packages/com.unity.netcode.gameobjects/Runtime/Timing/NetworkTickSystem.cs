using System;
using System.Diagnostics;
using Unity.Profiling;
using static Unity.Netcode.NetworkTickSystem;

namespace Unity.Netcode
{
    /// <summary>
    /// Provides discretized time.
    /// This is useful for games that require ticks happening at regular interval on the server and clients.
    /// </summary>
    public class NetworkTickSystem
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private static ProfilerMarker s_Tick = new ProfilerMarker($"{nameof(NetworkTickSystem)}.Tick");
#endif

        /// <summary>
        /// Special value to indicate "No tick information"
        /// </summary>
        public const int NoTick = int.MinValue;

        /// <summary>
        /// The TickRate of the tick system. This is used to decide how often a fixed network tick is run.
        /// </summary>
        public uint TickRate { get; internal set; }

        /// <summary>
        /// The current local time. This is the time at which predicted or client authoritative objects move.
        ///  This value is accurate when called in Update or during the <see cref="Tick"/> event but does not work correctly for FixedUpdate.
        /// </summary>
        public NetworkTime LocalTime { get; internal set; }

        /// <summary>
        /// The current server time. This value is mostly used for internal purposes and to interpolate state received from the server.
        ///  This value is accurate when called in Update or during the <see cref="Tick"/> event but does not work correctly for FixedUpdate.
        /// </summary>
        public NetworkTime ServerTime { get; internal set; }

        /// <summary>
        /// Gets invoked before every network tick.
        /// </summary>
        /// 
        public TickUpdateInfo _tickUpdateInfo;

        public event Action Tick;
        public struct TickUpdateInfo
        {
            // 업데이트 중 변경되는 값
            public int _prevUpdateLocalTick;
            public int _curUpdateLocalTick;
            public int _prevUpdateServerTick;
            public int _curUpdateServerTick;

            // 읽기 전용 값 (초기값 설정 후 변경 불가)
            public int _previousLocalTick;
            public int _previousServerTick;
            public int _startLocalTick;
            public int _startServerTick;
            public int _endLocalTick;
            public int _endServerTick;

            public TickUpdateInfo(int previousLocalTick, int previousServerTick, int startLocalTick, int startServerTick, int endLocalTick, int endServerTick)
            {
                _previousLocalTick = previousLocalTick;
                _previousServerTick = previousServerTick;
                _startLocalTick = startLocalTick;
                _startServerTick = startServerTick;
                _endLocalTick = endLocalTick;
                _endServerTick = endServerTick;
                _prevUpdateLocalTick = previousLocalTick;
                _curUpdateLocalTick = startLocalTick;
                _prevUpdateServerTick = previousServerTick;
                _curUpdateServerTick = startServerTick;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NetworkTickSystem"/> class.
        /// </summary>
        /// <param name="tickRate">The tick rate</param>
        /// <param name="localTimeSec">The initial local time to start at.</param>
        /// <param name="serverTimeSec">The initial server time to start at.</param>
        public NetworkTickSystem(uint tickRate, double localTimeSec, double serverTimeSec)
        {
            if (tickRate == 0)
            {
                throw new ArgumentException("Tick rate must be a positive value.", nameof(tickRate));
            }

            TickRate = tickRate;
            Tick = null;
            LocalTime = new NetworkTime(tickRate, localTimeSec);
            ServerTime = new NetworkTime(tickRate, serverTimeSec);
        }

        /// <summary>
        /// Resets the tick system to the given network time.
        /// </summary>
        /// <param name="localTimeSec">The local time in seconds.</param>
        /// <param name="serverTimeSec">The server time in seconds.</param>
        public void Reset(double localTimeSec, double serverTimeSec)
        {
            LocalTime = new NetworkTime(TickRate, localTimeSec);
            ServerTime = new NetworkTime(TickRate, serverTimeSec);
        }

        /// <summary>
        /// Called after advancing the time system to run ticks based on the difference in time.
        /// </summary>
        /// <param name="localTimeSec">The local time in seconds</param>
        /// <param name="serverTimeSec">The server time in seconds</param>

        public void UpdateTick(double localTimeSec, double serverTimeSec)
        {
            // store old local tick to know how many fixed ticks passed
            var previousLocalTick = LocalTime.Tick;
            var previousServerTick = ServerTime.Tick;

            LocalTime = new NetworkTime(TickRate, localTimeSec);
            ServerTime = new NetworkTime(TickRate, serverTimeSec);
            // cache times here so that we can adjust them to temporary values while simulating ticks.
            var cacheLocalTime = LocalTime;
            var cacheServerTime = ServerTime;
            var currentLocalTick = LocalTime.Tick;
            var localToServerDifference = currentLocalTick - ServerTime.Tick;

            _tickUpdateInfo = new TickUpdateInfo(previousLocalTick, previousServerTick,
                previousLocalTick + 1, previousLocalTick+1 - localToServerDifference, 
                currentLocalTick, currentLocalTick- localToServerDifference);
            for (int i = previousLocalTick + 1; i <= currentLocalTick; i++)
            {
                // set exposed time values to correct fixed values
                LocalTime = new NetworkTime(TickRate, i);
                ServerTime = new NetworkTime(TickRate, i - localToServerDifference);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
                s_Tick.Begin();
#endif
                _tickUpdateInfo._curUpdateLocalTick = i;
                _tickUpdateInfo._curUpdateServerTick = i - localToServerDifference;
                Tick?.Invoke();
                _tickUpdateInfo._prevUpdateLocalTick = i;
                _tickUpdateInfo._prevUpdateServerTick = i - localToServerDifference;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
                s_Tick.End();
#endif
            }
            // Set exposed time to values from tick system
            LocalTime = cacheLocalTime;
            ServerTime = cacheServerTime;
        }
    }
}
