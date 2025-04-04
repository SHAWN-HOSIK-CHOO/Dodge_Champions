// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.UI
{
    /// <summary>
    /// A structure representing a memory monitoring message.
    /// </summary>
    public struct MemoryMonitorCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Context that was passed into <see cref="UIInterface.AddNotifyMemoryMonitor" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// This field is for system specific memory monitor report.
        /// 
        /// If provided then the structure will be located in eos_<platform>_ui.h
        /// The structure will be named EOS_<platform>_MemoryMonitorReport.
        /// </summary>
        public System.IntPtr SystemMemoryMonitorReport { get; set; }

        public Result? GetResultCode()
        {
            return null;
        }

        internal void Set(ref MemoryMonitorCallbackInfoInternal other)
        {
            ClientData = other.ClientData;
            SystemMemoryMonitorReport = other.SystemMemoryMonitorReport;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct MemoryMonitorCallbackInfoInternal : ICallbackInfoInternal, IGettable<MemoryMonitorCallbackInfo>, ISettable<MemoryMonitorCallbackInfo>, System.IDisposable
    {
        private System.IntPtr m_ClientData;
        private System.IntPtr m_SystemMemoryMonitorReport;

        public object ClientData
        {
            get
            {
                object value;
                Helper.Get(m_ClientData, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ClientData);
            }
        }

        public System.IntPtr ClientDataAddress
        {
            get
            {
                return m_ClientData;
            }
        }

        public System.IntPtr SystemMemoryMonitorReport
        {
            get
            {
                return m_SystemMemoryMonitorReport;
            }

            set
            {
                m_SystemMemoryMonitorReport = value;
            }
        }

        public void Set(ref MemoryMonitorCallbackInfo other)
        {
            ClientData = other.ClientData;
            SystemMemoryMonitorReport = other.SystemMemoryMonitorReport;
        }

        public void Set(ref MemoryMonitorCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ClientData = other.Value.ClientData;
                SystemMemoryMonitorReport = other.Value.SystemMemoryMonitorReport;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_SystemMemoryMonitorReport);
        }

        public void Get(out MemoryMonitorCallbackInfo output)
        {
            output = new MemoryMonitorCallbackInfo();
            output.Set(ref this);
        }
    }
}