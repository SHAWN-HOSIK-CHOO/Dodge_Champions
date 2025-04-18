// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyInterface.GetInviteCount" /> function.
    /// </summary>
    public struct GetInviteCountOptions
    {
        /// <summary>
        /// The Product User ID of the local user whose cached lobby invitations you want to count
        /// </summary>
        public ProductUserId LocalUserId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct GetInviteCountOptionsInternal : ISettable<GetInviteCountOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public void Set(ref GetInviteCountOptions other)
        {
            m_ApiVersion = LobbyInterface.GetinvitecountApiLatest;
            LocalUserId = other.LocalUserId;
        }

        public void Set(ref GetInviteCountOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyInterface.GetinvitecountApiLatest;
                LocalUserId = other.Value.LocalUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
        }
    }
}