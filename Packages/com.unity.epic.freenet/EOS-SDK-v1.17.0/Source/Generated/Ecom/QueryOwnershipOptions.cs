// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Input parameters for the <see cref="EcomInterface.QueryOwnership" /> function.
    /// </summary>
    public struct QueryOwnershipOptions
    {
        /// <summary>
        /// The Epic Account ID of the local user whose ownership to query
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }

        /// <summary>
        /// The array of Catalog Item IDs to check for ownership
        /// </summary>
        public Utf8String[] CatalogItemIds { get; set; }

        /// <summary>
        /// Optional product namespace, if not the one specified during initialization
        /// </summary>
        public Utf8String CatalogNamespace { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct QueryOwnershipOptionsInternal : ISettable<QueryOwnershipOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_CatalogItemIds;
        private uint m_CatalogItemIdCount;
        private System.IntPtr m_CatalogNamespace;

        public EpicAccountId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String[] CatalogItemIds
        {
            set
            {
                Helper.Set(value, ref m_CatalogItemIds, out m_CatalogItemIdCount);
            }
        }

        public Utf8String CatalogNamespace
        {
            set
            {
                Helper.Set(value, ref m_CatalogNamespace);
            }
        }

        public void Set(ref QueryOwnershipOptions other)
        {
            m_ApiVersion = EcomInterface.QueryownershipApiLatest;
            LocalUserId = other.LocalUserId;
            CatalogItemIds = other.CatalogItemIds;
            CatalogNamespace = other.CatalogNamespace;
        }

        public void Set(ref QueryOwnershipOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = EcomInterface.QueryownershipApiLatest;
                LocalUserId = other.Value.LocalUserId;
                CatalogItemIds = other.Value.CatalogItemIds;
                CatalogNamespace = other.Value.CatalogNamespace;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_CatalogItemIds);
            Helper.Dispose(ref m_CatalogNamespace);
        }
    }
}