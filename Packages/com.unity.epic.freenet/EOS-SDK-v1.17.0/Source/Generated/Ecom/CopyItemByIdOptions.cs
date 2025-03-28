// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Input parameters for the <see cref="EcomInterface.CopyItemById" /> function.
    /// </summary>
    public struct CopyItemByIdOptions
    {
        /// <summary>
        /// The Epic Account ID of the local user whose item is being copied
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }

        /// <summary>
        /// The ID of the item to get.
        /// </summary>
        public Utf8String ItemId { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyItemByIdOptionsInternal : ISettable<CopyItemByIdOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_ItemId;

        public EpicAccountId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String ItemId
        {
            set
            {
                Helper.Set(value, ref m_ItemId);
            }
        }

        public void Set(ref CopyItemByIdOptions other)
        {
            m_ApiVersion = EcomInterface.CopyitembyidApiLatest;
            LocalUserId = other.LocalUserId;
            ItemId = other.ItemId;
        }

        public void Set(ref CopyItemByIdOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = EcomInterface.CopyitembyidApiLatest;
                LocalUserId = other.Value.LocalUserId;
                ItemId = other.Value.ItemId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_ItemId);
        }
    }
}