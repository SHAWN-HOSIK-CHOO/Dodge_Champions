using System;
using System.Collections;
using Unity.Profiling;
using UnityEngine;

namespace Unity.Netcode
{
    /// <summary>
    /// <see cref="NetworkTimeSystem"/> is a standalone system which can be used to run a network time simulation.
    /// The network time system maintains both a local and a server time. The local time is based on the server time
    /// as last received from the server plus an offset based on the current RTT - in other words, it is a best-guess
    /// effort at predicting what the server tick will be when a given network action is processed on the server.
    /// </summary>
    public class NetworkTimeSystem
    {
        /// <remarks>
        /// This was the original comment when it lived in NetworkManager:
        /// todo talk with UX/Product, find good default value for this
        /// </remarks>
        private const float k_DefaultBufferSizeSec = 0.05f;

        /// <summary>
        /// Time synchronization frequency defaults to 1 synchronization message per second
        /// </summary>
        private const double k_TimeSyncFrequency = 1.0d;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private static ProfilerMarker s_SyncTime = new ProfilerMarker($"{nameof(NetworkManager)}.SyncTime");
#endif

        public double m_TimeSec { get; set; }

        public double LocalBufferSec { get; set; }

        public double ServerBufferSec { get; set; }

        private NetworkConnectionManager m_ConnectionManager;
        private NetworkTransport m_NetworkTransport;
        private NetworkTickSystem m_NetworkTickSystem;
        private NetworkManager m_NetworkManager;

        /// <summary>
        /// <see cref="k_TimeSyncFrequency"/>
        /// </summary>
        private int m_TimeSyncFrequencyTicks;

        public double m_BaseTime;
        NtpClient client;
        Coroutine m_timeSyncCoroutine;

        /// <summary>
        /// The constructor class for <see cref="NetworkTickSystem"/>
        /// </summary>
        /// <param name="localBufferSec">The amount of time, in seconds, the server should buffer incoming client messages.</param>
        /// <param name="serverBufferSec">The amount of the time in seconds the client should buffer incoming messages from the server.</param>
        /// <param name="hardResetThresholdSec">The threshold, in seconds, used to force a hard catchup of network time.</param>
        /// <param name="adjustmentRatio">The ratio at which the NetworkTimeSystem speeds up or slows down time.</param>
        public NetworkTimeSystem(double localBufferSec, double serverBufferSec = k_DefaultBufferSizeSec)
        {
            LocalBufferSec = localBufferSec;
            ServerBufferSec = serverBufferSec;
        }

        /// <summary>
        /// The primary time system is initialized when a server-host or client is started
        /// </summary>
        internal NetworkTickSystem Initialize(NetworkManager networkManager)
        {
            m_NetworkManager = networkManager;
            m_ConnectionManager = networkManager.ConnectionManager;
            m_NetworkTransport = networkManager.NetworkConfig.NetworkTransport;
            m_TimeSyncFrequencyTicks = (int)(k_TimeSyncFrequency * networkManager.NetworkConfig.TickRate);
            m_NetworkTickSystem = new NetworkTickSystem(networkManager.NetworkConfig.TickRate,0);
            client = new NtpClient("time.windows.com");
            m_BaseTime = (client.GetNetworkTime() - NtpClient.baseTime).TotalSeconds;
            m_TimeSec = 0;
            m_timeSyncCoroutine = m_NetworkManager.StartCoroutine(OnTickSyncTime());
            return m_NetworkTickSystem;
        }

        internal void UpdateTime()
        {
            if (!m_ConnectionManager.LocalClient.IsConnected)
            {
                return;
            }
            m_TimeSec += m_NetworkManager.RealTimeProvider.UnscaledDeltaTime;
            m_NetworkTickSystem.UpdateTick(m_TimeSec);
        }

        /// <summary>
        /// Server-Side:
        /// Synchronizes time with clients based on the given <see cref="m_TimeSyncFrequencyTicks"/>.
        /// Also: <see cref="k_TimeSyncFrequency"/>
        /// </summary>
        /// <remarks>
        /// The default is to send 1 time synchronization message per second
        /// </remarks>
        private IEnumerator OnTickSyncTime()
        {
            while (true)
            {
                if (m_NetworkTickSystem.Time.Tick % m_TimeSyncFrequencyTicks == 0)
                {
                    Sync();
                    foreach (var client in m_ConnectionManager.ConnectedClients.Values)
                    {
                        if (client.ClientId != m_ConnectionManager.LocalClient.ClientId)
                        {
                            var msg = new TimeMessage
                            {
                                ClientId = m_ConnectionManager.LocalClient.ClientId,
                                Time = m_TimeSec,
                                SendDelay = client.ReceiveDelay
                            };
                            m_ConnectionManager.SendMessage(ref msg, NetworkDelivery.UnreliableSequenced, client.ClientId);
                        }
                    }
                }
                yield return null;
            }
        }


        internal void Shutdown()
        {
            if (m_timeSyncCoroutine != null)
                m_NetworkManager.StopCoroutine(m_timeSyncCoroutine);
        }

        public void Sync()
        {
            DateTime netTime = client.GetNetworkTime();
            m_TimeSec = (netTime - NtpClient.baseTime).TotalSeconds - m_BaseTime;
        }
    }
}
