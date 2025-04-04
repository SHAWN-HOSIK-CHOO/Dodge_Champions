// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
    /// <summary>
    /// Contains information about an external account linked with a Product User ID.
    /// </summary>
    public struct ExternalAccountInfo
    {
        /// <summary>
        /// The Product User ID of the target user.
        /// </summary>
        public ProductUserId ProductUserId { get; set; }

        /// <summary>
        /// Display name, can be null if not set.
        /// </summary>
        public Utf8String DisplayName { get; set; }

        /// <summary>
        /// External account ID.
        /// 
        /// May be set to an empty string if the AccountIdType of another user belongs
        /// to different account system than the local user's authenticated account.
        /// The availability of this field is dependent on account system specifics.
        /// </summary>
        public Utf8String AccountId { get; set; }

        /// <summary>
        /// The identity provider that owns the external account.
        /// </summary>
        public ExternalAccountType AccountIdType { get; set; }

        /// <summary>
        /// The POSIX timestamp for the time the user last logged in, or <see cref="ConnectInterface.TimeUndefined" />.
        /// </summary>
        public System.DateTimeOffset? LastLoginTime { get; set; }

        internal void Set(ref ExternalAccountInfoInternal other)
        {
            ProductUserId = other.ProductUserId;
            DisplayName = other.DisplayName;
            AccountId = other.AccountId;
            AccountIdType = other.AccountIdType;
            LastLoginTime = other.LastLoginTime;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct ExternalAccountInfoInternal : IGettable<ExternalAccountInfo>, ISettable<ExternalAccountInfo>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_ProductUserId;
        private System.IntPtr m_DisplayName;
        private System.IntPtr m_AccountId;
        private ExternalAccountType m_AccountIdType;
        private long m_LastLoginTime;

        public ProductUserId ProductUserId
        {
            get
            {
                ProductUserId value;
                Helper.Get(m_ProductUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ProductUserId);
            }
        }

        public Utf8String DisplayName
        {
            get
            {
                Utf8String value;
                Helper.Get(m_DisplayName, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_DisplayName);
            }
        }

        public Utf8String AccountId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_AccountId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_AccountId);
            }
        }

        public ExternalAccountType AccountIdType
        {
            get
            {
                return m_AccountIdType;
            }

            set
            {
                m_AccountIdType = value;
            }
        }

        public System.DateTimeOffset? LastLoginTime
        {
            get
            {
                System.DateTimeOffset? value;
                Helper.Get(m_LastLoginTime, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_LastLoginTime);
            }
        }

        public void Set(ref ExternalAccountInfo other)
        {
            m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyindexApiLatest;
            ProductUserId = other.ProductUserId;
            DisplayName = other.DisplayName;
            AccountId = other.AccountId;
            AccountIdType = other.AccountIdType;
            LastLoginTime = other.LastLoginTime;
        }

        public void Set(ref ExternalAccountInfo? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = ConnectInterface.CopyproductuserexternalaccountbyindexApiLatest;
                ProductUserId = other.Value.ProductUserId;
                DisplayName = other.Value.DisplayName;
                AccountId = other.Value.AccountId;
                AccountIdType = other.Value.AccountIdType;
                LastLoginTime = other.Value.LastLoginTime;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ProductUserId);
            Helper.Dispose(ref m_DisplayName);
            Helper.Dispose(ref m_AccountId);
        }

        public void Get(out ExternalAccountInfo output)
        {
            output = new ExternalAccountInfo();
            output.Set(ref this);
        }
    }
}