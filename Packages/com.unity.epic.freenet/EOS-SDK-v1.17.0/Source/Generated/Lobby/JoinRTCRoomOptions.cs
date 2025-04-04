// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyInterface.JoinRTCRoom" /> function.
    /// </summary>
    public struct JoinRTCRoomOptions
    {
        /// <summary>
        /// The ID of the lobby to join the RTC Room of
        /// </summary>
        public Utf8String LobbyId { get; set; }

        /// <summary>
        /// The Product User ID of the local user in the lobby
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// Allows the local application to set local audio options for the RTC Room if it is enabled.
        /// Only updates audio options when explicitly set; does not provide defaults.
        /// </summary>
        public LocalRTCOptions? LocalRTCOptions { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct JoinRTCRoomOptionsInternal : ISettable<JoinRTCRoomOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LobbyId;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_LocalRTCOptions;

        public Utf8String LobbyId
        {
            set
            {
                Helper.Set(value, ref m_LobbyId);
            }
        }

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public LocalRTCOptions? LocalRTCOptions
        {
            set
            {
                Helper.Set<LocalRTCOptions, LocalRTCOptionsInternal>(ref value, ref m_LocalRTCOptions);
            }
        }

        public void Set(ref JoinRTCRoomOptions other)
        {
            m_ApiVersion = LobbyInterface.JoinrtcroomApiLatest;
            LobbyId = other.LobbyId;
            LocalUserId = other.LocalUserId;
            LocalRTCOptions = other.LocalRTCOptions;
        }

        public void Set(ref JoinRTCRoomOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyInterface.JoinrtcroomApiLatest;
                LobbyId = other.Value.LobbyId;
                LocalUserId = other.Value.LocalUserId;
                LocalRTCOptions = other.Value.LocalRTCOptions;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LobbyId);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_LocalRTCOptions);
        }
    }
}