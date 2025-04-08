using System;
using Unity.Profiling;

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
        /// 
        //public NetworkTime LocalTime { get; internal set; }

        /// <summary>
        /// The current server time. This value is mostly used for internal purposes and to interpolate state received from the server.
        ///  This value is accurate when called in Update or during the <see cref="Tick"/> event but does not work correctly for FixedUpdate.
        /// </summary>
        //public NetworkTime ServerTime { get; internal set; }

        public NetworkTime Time { get; internal set; }

        /// <summary>
        /// Gets invoked before every network tick.
        /// </summary>
        /// 
        public event Action Tick;
        /// <summary>
        /// Creates a new instance of the <see cref="NetworkTickSystem"/> class.
        /// </summary>
        /// <param name="tickRate">The tick rate</param>
        /// <param name="localTimeSec">The initial local time to start at.</param>
        /// <param name="serverTimeSec">The initial server time to start at.</param>
        public NetworkTickSystem(uint tickRate, double timeSec)
        {
            if (tickRate == 0)
            {
                throw new ArgumentException("Tick rate must be a positive value.", nameof(tickRate));
            }

            TickRate = tickRate;
            Tick = null;
            Time = new NetworkTime(tickRate, timeSec);
        }

        /// <summary>
        /// Resets the tick system to the given network time.
        /// </summary>
        /// <param name="localTimeSec">The local time in seconds.</param>
        /// <param name="serverTimeSec">The server time in seconds.</param>
        public void Reset(double timeSec)
        {
            Time = new NetworkTime(TickRate, timeSec);
        }

        /// <summary>
        /// Called after advancing the time system to run ticks based on the difference in time.
        /// </summary>
        /// <param name="localTimeSec">The local time in seconds</param>
        /// <param name="serverTimeSec">The server time in seconds</param>

        public void UpdateTick(double timeSec)
        {
            var prevTick = Time.Tick;

            Time = new NetworkTime(TickRate, timeSec);
            var cacheTime = Time;
            for (int i = prevTick + 1; i <= cacheTime.Tick; i++)
            {
                Time = new NetworkTime(TickRate, i);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                s_Tick.Begin();
#endif
                Tick?.Invoke();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                s_Tick.End();
#endif
            }
            // Set exposed time to values from tick system
            Time = cacheTime;
        }
    }
}
