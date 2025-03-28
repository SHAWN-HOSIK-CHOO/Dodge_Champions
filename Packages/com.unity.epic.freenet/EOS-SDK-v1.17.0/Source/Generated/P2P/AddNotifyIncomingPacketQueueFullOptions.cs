// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.P2P
{
    /// <summary>
    /// Structure containing information about what version of the <see cref="P2PInterface.AddNotifyIncomingPacketQueueFull" /> function is supported.
    /// </summary>
    public struct AddNotifyIncomingPacketQueueFullOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct AddNotifyIncomingPacketQueueFullOptionsInternal : ISettable<AddNotifyIncomingPacketQueueFullOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref AddNotifyIncomingPacketQueueFullOptions other)
        {
            m_ApiVersion = P2PInterface.AddnotifyincomingpacketqueuefullApiLatest;
        }

        public void Set(ref AddNotifyIncomingPacketQueueFullOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = P2PInterface.AddnotifyincomingpacketqueuefullApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}