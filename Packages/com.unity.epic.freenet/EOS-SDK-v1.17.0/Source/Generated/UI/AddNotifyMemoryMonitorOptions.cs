// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
    /// <summary>
    /// Input parameters for the <see cref="UIInterface.AddNotifyMemoryMonitor" /> function.
    /// </summary>
    public struct AddNotifyMemoryMonitorOptions
    {
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct AddNotifyMemoryMonitorOptionsInternal : ISettable<AddNotifyMemoryMonitorOptions>, System.IDisposable
    {
        private int m_ApiVersion;

        public void Set(ref AddNotifyMemoryMonitorOptions other)
        {
            m_ApiVersion = UIInterface.AddnotifymemorymonitorApiLatest;
        }

        public void Set(ref AddNotifyMemoryMonitorOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = UIInterface.AddnotifymemorymonitorApiLatest;
            }
        }

        public void Dispose()
        {
        }
    }
}