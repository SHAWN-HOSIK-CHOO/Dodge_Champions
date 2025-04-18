// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.P2P
{
    /// <summary>
    /// Structure containing information about who would like to close connections, and by what socket ID
    /// </summary>
    public struct CloseConnectionsOptions
    {
        /// <summary>
        /// The Product User ID of the local user who would like to close all connections that use a particular socket ID
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The socket ID of the connections to close
        /// </summary>
        public SocketId? SocketId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CloseConnectionsOptionsInternal : ISettable<CloseConnectionsOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_SocketId;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public SocketId? SocketId
        {
            set
            {
                Helper.Set<SocketId, SocketIdInternal>(ref value, ref m_SocketId);
            }
        }

        public void Set(ref CloseConnectionsOptions other)
        {
            m_ApiVersion = P2PInterface.CloseconnectionsApiLatest;
            LocalUserId = other.LocalUserId;
            SocketId = other.SocketId;
        }

        public void Set(ref CloseConnectionsOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = P2PInterface.CloseconnectionsApiLatest;
                LocalUserId = other.Value.LocalUserId;
                SocketId = other.Value.SocketId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_SocketId);
        }
    }
}