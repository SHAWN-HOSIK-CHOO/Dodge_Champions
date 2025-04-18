// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    public struct RTCRoomConnectionChangedCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Context that was passed into <see cref="LobbyInterface.AddNotifyRTCRoomConnectionChanged" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The ID of the lobby which had a RTC Room connection state change
        /// </summary>
        public Utf8String LobbyId { get; set; }

        /// <summary>
        /// The Product User ID of the local user who is in the lobby and registered for notifications
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The new connection state of the room
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// <see cref="Result.Success" />: The room was left locally. This may be because: the associated lobby was Left or Destroyed, the connection to the lobby was interrupted, or because the SDK is being shutdown. If the lobby connection returns (lobby did not permanently go away), we will reconnect.
        /// <see cref="Result.NoConnection" />: There was a network issue connecting to the server. We will attempt to reconnect soon.
        /// <see cref="Result.UserKicked" />: The user has been kicked by the server. We will not reconnect.
        /// <see cref="Result.UserBanned" />: The user has been banned by the server. We will not reconnect.
        /// <see cref="Result.ServiceFailure" />: A known error occurred during interaction with the server. We will attempt to reconnect soon.
        /// <see cref="Result.UnexpectedError" />: Unexpected error. We will attempt to reconnect soon.
        /// </summary>
        public Result DisconnectReason { get; set; }

        public Result? GetResultCode()
        {
            return null;
        }

        internal void Set(ref RTCRoomConnectionChangedCallbackInfoInternal other)
        {
            ClientData = other.ClientData;
            LobbyId = other.LobbyId;
            LocalUserId = other.LocalUserId;
            IsConnected = other.IsConnected;
            DisconnectReason = other.DisconnectReason;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct RTCRoomConnectionChangedCallbackInfoInternal : ICallbackInfoInternal, IGettable<RTCRoomConnectionChangedCallbackInfo>, ISettable<RTCRoomConnectionChangedCallbackInfo>, System.IDisposable
    {
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LobbyId;
        private System.IntPtr m_LocalUserId;
        private int m_IsConnected;
        private Result m_DisconnectReason;

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

        public bool IsConnected
        {
            get
            {
                bool value;
                Helper.Get(m_IsConnected, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_IsConnected);
            }
        }

        public Result DisconnectReason
        {
            get
            {
                return m_DisconnectReason;
            }

            set
            {
                m_DisconnectReason = value;
            }
        }

        public void Set(ref RTCRoomConnectionChangedCallbackInfo other)
        {
            ClientData = other.ClientData;
            LobbyId = other.LobbyId;
            LocalUserId = other.LocalUserId;
            IsConnected = other.IsConnected;
            DisconnectReason = other.DisconnectReason;
        }

        public void Set(ref RTCRoomConnectionChangedCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ClientData = other.Value.ClientData;
                LobbyId = other.Value.LobbyId;
                LocalUserId = other.Value.LocalUserId;
                IsConnected = other.Value.IsConnected;
                DisconnectReason = other.Value.DisconnectReason;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LobbyId);
            Helper.Dispose(ref m_LocalUserId);
        }

        public void Get(out RTCRoomConnectionChangedCallbackInfo output)
        {
            output = new RTCRoomConnectionChangedCallbackInfo();
            output.Set(ref this);
        }
    }
}