// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.P2P
{
    /// <summary>
    /// Information related to the current state of the packet queues. It is possible for the current size
    /// to be larger than the maximum size if the maximum size changes or if the maximum queue size is
    /// set to <see cref="P2PInterface.MaxQueueSizeUnlimited" />.
    /// </summary>
    public struct PacketQueueInfo
    {
        /// <summary>
        /// The maximum size in bytes of the incoming packet queue
        /// </summary>
        public ulong IncomingPacketQueueMaxSizeBytes { get; set; }

        /// <summary>
        /// The current size in bytes of the incoming packet queue
        /// </summary>
        public ulong IncomingPacketQueueCurrentSizeBytes { get; set; }

        /// <summary>
        /// The current number of queued packets in the incoming packet queue
        /// </summary>
        public ulong IncomingPacketQueueCurrentPacketCount { get; set; }

        /// <summary>
        /// The maximum size in bytes of the outgoing packet queue
        /// </summary>
        public ulong OutgoingPacketQueueMaxSizeBytes { get; set; }

        /// <summary>
        /// The current size in bytes of the outgoing packet queue
        /// </summary>
        public ulong OutgoingPacketQueueCurrentSizeBytes { get; set; }

        /// <summary>
        /// The current amount of queued packets in the outgoing packet queue
        /// </summary>
        public ulong OutgoingPacketQueueCurrentPacketCount { get; set; }

        internal void Set(ref PacketQueueInfoInternal other)
        {
            IncomingPacketQueueMaxSizeBytes = other.IncomingPacketQueueMaxSizeBytes;
            IncomingPacketQueueCurrentSizeBytes = other.IncomingPacketQueueCurrentSizeBytes;
            IncomingPacketQueueCurrentPacketCount = other.IncomingPacketQueueCurrentPacketCount;
            OutgoingPacketQueueMaxSizeBytes = other.OutgoingPacketQueueMaxSizeBytes;
            OutgoingPacketQueueCurrentSizeBytes = other.OutgoingPacketQueueCurrentSizeBytes;
            OutgoingPacketQueueCurrentPacketCount = other.OutgoingPacketQueueCurrentPacketCount;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct PacketQueueInfoInternal : IGettable<PacketQueueInfo>, ISettable<PacketQueueInfo>, System.IDisposable
    {
        private ulong m_IncomingPacketQueueMaxSizeBytes;
        private ulong m_IncomingPacketQueueCurrentSizeBytes;
        private ulong m_IncomingPacketQueueCurrentPacketCount;
        private ulong m_OutgoingPacketQueueMaxSizeBytes;
        private ulong m_OutgoingPacketQueueCurrentSizeBytes;
        private ulong m_OutgoingPacketQueueCurrentPacketCount;

        public ulong IncomingPacketQueueMaxSizeBytes
        {
            get
            {
                return m_IncomingPacketQueueMaxSizeBytes;
            }

            set
            {
                m_IncomingPacketQueueMaxSizeBytes = value;
            }
        }

        public ulong IncomingPacketQueueCurrentSizeBytes
        {
            get
            {
                return m_IncomingPacketQueueCurrentSizeBytes;
            }

            set
            {
                m_IncomingPacketQueueCurrentSizeBytes = value;
            }
        }

        public ulong IncomingPacketQueueCurrentPacketCount
        {
            get
            {
                return m_IncomingPacketQueueCurrentPacketCount;
            }

            set
            {
                m_IncomingPacketQueueCurrentPacketCount = value;
            }
        }

        public ulong OutgoingPacketQueueMaxSizeBytes
        {
            get
            {
                return m_OutgoingPacketQueueMaxSizeBytes;
            }

            set
            {
                m_OutgoingPacketQueueMaxSizeBytes = value;
            }
        }

        public ulong OutgoingPacketQueueCurrentSizeBytes
        {
            get
            {
                return m_OutgoingPacketQueueCurrentSizeBytes;
            }

            set
            {
                m_OutgoingPacketQueueCurrentSizeBytes = value;
            }
        }

        public ulong OutgoingPacketQueueCurrentPacketCount
        {
            get
            {
                return m_OutgoingPacketQueueCurrentPacketCount;
            }

            set
            {
                m_OutgoingPacketQueueCurrentPacketCount = value;
            }
        }

        public void Set(ref PacketQueueInfo other)
        {
            IncomingPacketQueueMaxSizeBytes = other.IncomingPacketQueueMaxSizeBytes;
            IncomingPacketQueueCurrentSizeBytes = other.IncomingPacketQueueCurrentSizeBytes;
            IncomingPacketQueueCurrentPacketCount = other.IncomingPacketQueueCurrentPacketCount;
            OutgoingPacketQueueMaxSizeBytes = other.OutgoingPacketQueueMaxSizeBytes;
            OutgoingPacketQueueCurrentSizeBytes = other.OutgoingPacketQueueCurrentSizeBytes;
            OutgoingPacketQueueCurrentPacketCount = other.OutgoingPacketQueueCurrentPacketCount;
        }

        public void Set(ref PacketQueueInfo? other)
        {
            if (other.HasValue)
            {
                IncomingPacketQueueMaxSizeBytes = other.Value.IncomingPacketQueueMaxSizeBytes;
                IncomingPacketQueueCurrentSizeBytes = other.Value.IncomingPacketQueueCurrentSizeBytes;
                IncomingPacketQueueCurrentPacketCount = other.Value.IncomingPacketQueueCurrentPacketCount;
                OutgoingPacketQueueMaxSizeBytes = other.Value.OutgoingPacketQueueMaxSizeBytes;
                OutgoingPacketQueueCurrentSizeBytes = other.Value.OutgoingPacketQueueCurrentSizeBytes;
                OutgoingPacketQueueCurrentPacketCount = other.Value.OutgoingPacketQueueCurrentPacketCount;
            }
        }

        public void Dispose()
        {
        }

        public void Get(out PacketQueueInfo output)
        {
            output = new PacketQueueInfo();
            output.Set(ref this);
        }
    }
}