// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
    /// <summary>
    /// Input parameters for the <see cref="UIInterface.ShowFriends" /> function.
    /// </summary>
    public struct ShowFriendsOptions
    {
        /// <summary>
        /// The Epic Account ID of the user whose friend list is being shown.
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct ShowFriendsOptionsInternal : ISettable<ShowFriendsOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;

        public EpicAccountId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public void Set(ref ShowFriendsOptions other)
        {
            m_ApiVersion = UIInterface.ShowfriendsApiLatest;
            LocalUserId = other.LocalUserId;
        }

        public void Set(ref ShowFriendsOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = UIInterface.ShowfriendsApiLatest;
                LocalUserId = other.Value.LocalUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
        }
    }
}