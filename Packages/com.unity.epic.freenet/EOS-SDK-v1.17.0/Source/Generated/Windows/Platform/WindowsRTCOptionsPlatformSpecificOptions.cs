// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Platform
{
    /// <summary>
    /// Options for initializing rtc functionality required for some platforms.
    /// </summary>
    public struct WindowsRTCOptionsPlatformSpecificOptions
    {
        /// <summary>
        /// The absolute path to a `xaudio2_9redist.dll` dependency, including the file name.
        /// </summary>
        public Utf8String XAudio29DllPath { get; set; }

        internal void Set(ref WindowsRTCOptionsPlatformSpecificOptionsInternal other)
        {
            XAudio29DllPath = other.XAudio29DllPath;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct WindowsRTCOptionsPlatformSpecificOptionsInternal : IGettable<WindowsRTCOptionsPlatformSpecificOptions>, ISettable<WindowsRTCOptionsPlatformSpecificOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private System.IntPtr m_XAudio29DllPath;

        public Utf8String XAudio29DllPath
        {
            get
            {
                Utf8String value;
                Helper.Get(m_XAudio29DllPath, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_XAudio29DllPath);
            }
        }

        public void Set(ref WindowsRTCOptionsPlatformSpecificOptions other)
        {
            m_ApiVersion = PlatformInterface.WindowsRtcoptionsplatformspecificoptionsApiLatest;
            XAudio29DllPath = other.XAudio29DllPath;
        }

        public void Set(ref WindowsRTCOptionsPlatformSpecificOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = PlatformInterface.WindowsRtcoptionsplatformspecificoptionsApiLatest;
                XAudio29DllPath = other.Value.XAudio29DllPath;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_XAudio29DllPath);
        }

        public void Get(out WindowsRTCOptionsPlatformSpecificOptions output)
        {
            output = new WindowsRTCOptionsPlatformSpecificOptions();
            output.Set(ref this);
        }
    }
}