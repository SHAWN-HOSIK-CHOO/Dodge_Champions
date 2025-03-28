// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
    /// <summary>
    /// Parameters for the <see cref="UIInterface.PrePresent" /> function.
    /// </summary>
    public struct PrePresentOptions
    {
        /// <summary>
        /// Platform specific data.
        /// </summary>
        public System.IntPtr PlatformSpecificData { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct PrePresentOptionsInternal : ISettable<PrePresentOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_PlatformSpecificData;

        public System.IntPtr PlatformSpecificData
        {
            set
            {
                m_PlatformSpecificData = value;
            }
        }

        public void Set(ref PrePresentOptions other)
        {
            m_ApiVersion = UIInterface.PrepresentApiLatest;
            PlatformSpecificData = other.PlatformSpecificData;
        }

        public void Set(ref PrePresentOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = UIInterface.PrepresentApiLatest;
                PlatformSpecificData = other.Value.PlatformSpecificData;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_PlatformSpecificData);
        }
    }
}