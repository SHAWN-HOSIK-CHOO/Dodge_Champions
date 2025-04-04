// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
    /// <summary>
    /// Input parameters for the <see cref="SessionsInterface.EndSession" /> function.
    /// </summary>
    public struct EndSessionOptions
    {
        /// <summary>
        /// Name of the session to set as no long in progress
        /// </summary>
        public Utf8String SessionName { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct EndSessionOptionsInternal : ISettable<EndSessionOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_SessionName;

        public Utf8String SessionName
        {
            set
            {
                Helper.Set(value, ref m_SessionName);
            }
        }

        public void Set(ref EndSessionOptions other)
        {
            m_ApiVersion = SessionsInterface.EndsessionApiLatest;
            SessionName = other.SessionName;
        }

        public void Set(ref EndSessionOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = SessionsInterface.EndsessionApiLatest;
                SessionName = other.Value.SessionName;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_SessionName);
        }
    }
}