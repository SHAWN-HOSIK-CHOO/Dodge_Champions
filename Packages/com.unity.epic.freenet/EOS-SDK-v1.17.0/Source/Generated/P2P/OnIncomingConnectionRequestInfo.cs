// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.P2P
{
    /// <summary>
    /// Structure containing information about an incoming connection request.
    /// </summary>
    public struct OnIncomingConnectionRequestInfo : ICallbackInfo
    {
        /// <summary>
        /// Client-specified data passed into <see cref="Presence.PresenceInterface.AddNotifyOnPresenceChanged" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The Product User ID of the local user who is being requested to open a P2P session with RemoteUserId
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The Product User ID of the remote user who requested a peer connection with the local user
        /// </summary>
        public ProductUserId RemoteUserId { get; set; }

        /// <summary>
        /// The ID of the socket the Remote User wishes to communicate on
        /// </summary>
        public SocketId? SocketId { get; set; }

        public Result? GetResultCode()
        {
            return null;
        }

        internal void Set(ref OnIncomingConnectionRequestInfoInternal other)
        {
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            RemoteUserId = other.RemoteUserId;
            SocketId = other.SocketId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct OnIncomingConnectionRequestInfoInternal : ICallbackInfoInternal, IGettable<OnIncomingConnectionRequestInfo>, ISettable<OnIncomingConnectionRequestInfo>, System.IDisposable
    {
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_RemoteUserId;
        private System.IntPtr m_SocketId;

        public object ClientData
        {
            get
            {
                object value;
                Helper.Get(m_ClientData, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ClientData);
            }
        }

        public System.IntPtr ClientDataAddress
        {
            get
            {
                return m_ClientData;
            }
        }

        public ProductUserId LocalUserId
        {
            get
            {
                ProductUserId value;
                Helper.Get(m_LocalUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public ProductUserId RemoteUserId
        {
            get
            {
                ProductUserId value;
                Helper.Get(m_RemoteUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_RemoteUserId);
            }
        }

        public SocketId? SocketId
        {
            get
            {
                SocketId? value;
                Helper.Get<SocketIdInternal, SocketId>(m_SocketId, out value);
                return value;
            }

            set
            {
                Helper.Set<SocketId, SocketIdInternal>(ref value, ref m_SocketId);
            }
        }

        public void Set(ref OnIncomingConnectionRequestInfo other)
        {
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            RemoteUserId = other.RemoteUserId;
            SocketId = other.SocketId;
        }

        public void Set(ref OnIncomingConnectionRequestInfo? other)
        {
            if (other.HasValue)
            {
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
                RemoteUserId = other.Value.RemoteUserId;
                SocketId = other.Value.SocketId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_RemoteUserId);
            Helper.Dispose(ref m_SocketId);
        }

        public void Get(out OnIncomingConnectionRequestInfo output)
        {
            output = new OnIncomingConnectionRequestInfo();
            output.Set(ref this);
        }
    }
}