// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Input parameters for the <see cref="LobbyDetails.CopyMemberInfo" /> function.
    /// </summary>
    public struct LobbyDetailsCopyMemberInfoOptions
    {
        /// <summary>
        /// The Product User ID of the lobby member to copy.
        /// </summary>
        public ProductUserId TargetUserId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LobbyDetailsCopyMemberInfoOptionsInternal : ISettable<LobbyDetailsCopyMemberInfoOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_TargetUserId;

        public ProductUserId TargetUserId
        {
            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public void Set(ref LobbyDetailsCopyMemberInfoOptions other)
        {
            m_ApiVersion = LobbyDetails.LobbydetailsCopymemberinfoApiLatest;
            TargetUserId = other.TargetUserId;
        }

        public void Set(ref LobbyDetailsCopyMemberInfoOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = LobbyDetails.LobbydetailsCopymemberinfoApiLatest;
                TargetUserId = other.Value.TargetUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_TargetUserId);
        }
    }
}