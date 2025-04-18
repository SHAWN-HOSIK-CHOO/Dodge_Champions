// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
    /// <summary>
    /// Input parameters for the <see cref="ConnectInterface.CopyProductUserExternalAccountByAccountType" /> function.
    /// </summary>
    public struct CopyProductUserExternalAccountByAccountTypeOptions
    {
        /// <summary>
        /// The Product User ID to look for when copying external account info from the cache.
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        /// <summary>
        /// External auth service account type to look for when copying external account info from the cache.
        /// </summary>
        public ExternalAccountType AccountIdType { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CopyProductUserExternalAccountByAccountTypeOptionsInternal : ISettable<CopyProductUserExternalAccountByAccountTypeOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_TargetUserId;
        private ExternalAccountType m_AccountIdType;

        public ProductUserId TargetUserId
        {
            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public ExternalAccountType AccountIdType
        {
            set
            {
                m_AccountIdType = value;
            }
        }

        public void Set(ref CopyProductUserExternalAccountByAccountTypeOptions other)
        {
            m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyaccounttypeApiLatest;
            TargetUserId = other.TargetUserId;
            AccountIdType = other.AccountIdType;
        }

        public void Set(ref CopyProductUserExternalAccountByAccountTypeOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyaccounttypeApiLatest;
                TargetUserId = other.Value.TargetUserId;
                AccountIdType = other.Value.AccountIdType;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_TargetUserId);
        }
    }
}