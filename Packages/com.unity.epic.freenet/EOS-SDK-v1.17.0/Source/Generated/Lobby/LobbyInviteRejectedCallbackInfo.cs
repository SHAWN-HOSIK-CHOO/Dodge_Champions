// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Output parameters for the <see cref="OnLobbyInviteRejectedCallback" /> Function.
    /// </summary>
    public struct LobbyInviteRejectedCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Context that was passed into <see cref="LobbyInterface.AddNotifyLobbyInviteRejected" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The invite ID
        /// </summary>
        public Utf8String InviteId { get; set; }

        /// <summary>
        /// The Product User ID of the local user who received the invitation
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The Product User ID of the user who sent the invitation
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        /// <summary>
        /// Lobby ID that the user has been invited to
        /// </summary>
        public Utf8String LobbyId { get; set; }

        public Result? GetResultCode()
        {
            return null;
        }

        internal void Set(ref LobbyInviteRejectedCallbackInfoInternal other)
        {
            ClientData = other.ClientData;
            InviteId = other.InviteId;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
            LobbyId = other.LobbyId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbyInviteRejectedCallbackInfoInternal : ICallbackInfoInternal, IGettable<LobbyInviteRejectedCallbackInfo>, ISettable<LobbyInviteRejectedCallbackInfo>, System.IDisposable
    {
        private System.IntPtr m_ClientData;
        private System.IntPtr m_InviteId;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_TargetUserId;
        private System.IntPtr m_LobbyId;

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

        public Utf8String InviteId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_InviteId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_InviteId);
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

        public ProductUserId TargetUserId
        {
            get
            {
                ProductUserId value;
                Helper.Get(m_TargetUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public Utf8String LobbyId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_LobbyId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_LobbyId);
            }
        }

        public void Set(ref LobbyInviteRejectedCallbackInfo other)
        {
            ClientData = other.ClientData;
            InviteId = other.InviteId;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
            LobbyId = other.LobbyId;
        }

        public void Set(ref LobbyInviteRejectedCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ClientData = other.Value.ClientData;
                InviteId = other.Value.InviteId;
                LocalUserId = other.Value.LocalUserId;
                TargetUserId = other.Value.TargetUserId;
                LobbyId = other.Value.LobbyId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_InviteId);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_TargetUserId);
            Helper.Dispose(ref m_LobbyId);
        }

        public void Get(out LobbyInviteRejectedCallbackInfo output)
        {
            output = new LobbyInviteRejectedCallbackInfo();
            output.Set(ref this);
        }
    }
}