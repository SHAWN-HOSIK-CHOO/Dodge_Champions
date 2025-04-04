// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.KWS
{
    /// <summary>
    /// Input parameters for the <see cref="KWSInterface.RequestPermissions" /> function.
    /// </summary>
    public struct RequestPermissionsOptions
    {
        /// <summary>
        /// Local user requesting new permissions
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// Names of the permissions to request (Setup with KWS)
        /// </summary>
        public Utf8String[] PermissionKeys { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct RequestPermissionsOptionsInternal : ISettable<RequestPermissionsOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_LocalUserId;
        private uint m_PermissionKeyCount;
        private System.IntPtr m_PermissionKeys;

        public ProductUserId LocalUserId
        {
            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String[] PermissionKeys
        {
            set
            {
                Helper.Set(value, ref m_PermissionKeys, true, out m_PermissionKeyCount);
            }
        }

        public void Set(ref RequestPermissionsOptions other)
        {
            m_ApiVersion = KWSInterface.RequestpermissionsApiLatest;
            LocalUserId = other.LocalUserId;
            PermissionKeys = other.PermissionKeys;
        }

        public void Set(ref RequestPermissionsOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = KWSInterface.RequestpermissionsApiLatest;
                LocalUserId = other.Value.LocalUserId;
                PermissionKeys = other.Value.PermissionKeys;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_PermissionKeys);
        }
    }
}