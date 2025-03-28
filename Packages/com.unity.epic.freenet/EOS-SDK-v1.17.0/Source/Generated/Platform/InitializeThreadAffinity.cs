// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Platform
{
    /// <summary>
    /// Options for initializing defining thread affinity for use by Epic Online Services SDK.
    /// Set the affinity to 0 to allow EOS SDK to use a platform specific default value.
    /// </summary>
    public struct InitializeThreadAffinity
    {
        /// <summary>
        /// Any thread related to network management that is not IO.
        /// </summary>
        public ulong NetworkWork { get; set; }

        /// <summary>
        /// Any thread that will interact with a storage device.
        /// </summary>
        public ulong StorageIo { get; set; }

        /// <summary>
        /// Any thread that will generate web socket IO.
        /// </summary>
        public ulong WebSocketIo { get; set; }

        /// <summary>
        /// Any thread that will generate IO related to P2P traffic and management.
        /// </summary>
        public ulong P2PIo { get; set; }

        /// <summary>
        /// Any thread that will generate http request IO.
        /// </summary>
        public ulong HttpRequestIo { get; set; }

        /// <summary>
        /// Any thread that will generate IO related to RTC traffic and management.
        /// </summary>
        public ulong RTCIo { get; set; }

        /// <summary>
        /// Main thread of the external overlay
        /// </summary>
        public ulong EmbeddedOverlayMainThread { get; set; }

        /// <summary>
        /// Worker threads of the external overlay
        /// </summary>
        public ulong EmbeddedOverlayWorkerThreads { get; set; }

        internal void Set(ref InitializeThreadAffinityInternal other)
        {
            NetworkWork = other.NetworkWork;
            StorageIo = other.StorageIo;
            WebSocketIo = other.WebSocketIo;
            P2PIo = other.P2PIo;
            HttpRequestIo = other.HttpRequestIo;
            RTCIo = other.RTCIo;
            EmbeddedOverlayMainThread = other.EmbeddedOverlayMainThread;
            EmbeddedOverlayWorkerThreads = other.EmbeddedOverlayWorkerThreads;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct InitializeThreadAffinityInternal : IGettable<InitializeThreadAffinity>, ISettable<InitializeThreadAffinity>, System.IDisposable
    {
        private int m_ApiVersion;
        private ulong m_NetworkWork;
        private ulong m_StorageIo;
        private ulong m_WebSocketIo;
        private ulong m_P2PIo;
        private ulong m_HttpRequestIo;
        private ulong m_RTCIo;
        private ulong m_EmbeddedOverlayMainThread;
        private ulong m_EmbeddedOverlayWorkerThreads;

        public ulong NetworkWork
        {
            get
            {
                return m_NetworkWork;
            }

            set
            {
                m_NetworkWork = value;
            }
        }

        public ulong StorageIo
        {
            get
            {
                return m_StorageIo;
            }

            set
            {
                m_StorageIo = value;
            }
        }

        public ulong WebSocketIo
        {
            get
            {
                return m_WebSocketIo;
            }

            set
            {
                m_WebSocketIo = value;
            }
        }

        public ulong P2PIo
        {
            get
            {
                return m_P2PIo;
            }

            set
            {
                m_P2PIo = value;
            }
        }

        public ulong HttpRequestIo
        {
            get
            {
                return m_HttpRequestIo;
            }

            set
            {
                m_HttpRequestIo = value;
            }
        }

        public ulong RTCIo
        {
            get
            {
                return m_RTCIo;
            }

            set
            {
                m_RTCIo = value;
            }
        }

        public ulong EmbeddedOverlayMainThread
        {
            get
            {
                return m_EmbeddedOverlayMainThread;
            }

            set
            {
                m_EmbeddedOverlayMainThread = value;
            }
        }

        public ulong EmbeddedOverlayWorkerThreads
        {
            get
            {
                return m_EmbeddedOverlayWorkerThreads;
            }

            set
            {
                m_EmbeddedOverlayWorkerThreads = value;
            }
        }

        public void Set(ref InitializeThreadAffinity other)
        {
            m_ApiVersion = PlatformInterface.InitializeThreadaffinityApiLatest;
            NetworkWork = other.NetworkWork;
            StorageIo = other.StorageIo;
            WebSocketIo = other.WebSocketIo;
            P2PIo = other.P2PIo;
            HttpRequestIo = other.HttpRequestIo;
            RTCIo = other.RTCIo;
            EmbeddedOverlayMainThread = other.EmbeddedOverlayMainThread;
            EmbeddedOverlayWorkerThreads = other.EmbeddedOverlayWorkerThreads;
        }

        public void Set(ref InitializeThreadAffinity? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = PlatformInterface.InitializeThreadaffinityApiLatest;
                NetworkWork = other.Value.NetworkWork;
                StorageIo = other.Value.StorageIo;
                WebSocketIo = other.Value.WebSocketIo;
                P2PIo = other.Value.P2PIo;
                HttpRequestIo = other.Value.HttpRequestIo;
                RTCIo = other.Value.RTCIo;
                EmbeddedOverlayMainThread = other.Value.EmbeddedOverlayMainThread;
                EmbeddedOverlayWorkerThreads = other.Value.EmbeddedOverlayWorkerThreads;
            }
        }

        public void Dispose()
        {
        }

        public void Get(out InitializeThreadAffinity output)
        {
            output = new InitializeThreadAffinity();
            output.Set(ref this);
        }
    }
}