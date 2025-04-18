// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Friends
{
    /// <summary>
    /// Input parameters for the <see cref="FriendsInterface.RejectInvite" /> function.
    /// </summary>
    public struct RejectInviteOptions
    {
        /// <summary>
        /// The Epic Account ID of the local, logged-in user who is rejecting a friends list invitation
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }

        /// <summary>
        /// The Epic Account ID of the user who sent the friends list invitation
        /// </summary>
        public EpicAccountId TargetUserId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct RejectInviteOptionsInternal : ISettable<RejectInviteOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_TargetUserId;

        public EpicAccountId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public EpicAccountId TargetUserId
        {
            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public void Set(ref RejectInviteOptions other)
        {
            m_ApiVersion = FriendsInterface.RejectinviteApiLatest;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
        }

        public void Set(ref RejectInviteOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = FriendsInterface.RejectinviteApiLatest;
                LocalUserId = other.Value.LocalUserId;
                TargetUserId = other.Value.TargetUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_TargetUserId);
        }
    }
}