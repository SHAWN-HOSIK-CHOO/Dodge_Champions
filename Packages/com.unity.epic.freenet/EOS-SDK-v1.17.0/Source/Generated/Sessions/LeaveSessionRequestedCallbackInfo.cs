// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
    /// <summary>
    /// Output parameters for the <see cref="OnLeaveSessionRequestedCallback" /> Function.
    /// </summary>
    public struct LeaveSessionRequestedCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Context that was passed into <see cref="OnLeaveSessionRequestedCallback" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The Product User ID of the local user who received the leave session notification.
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// Name of the session associated with the leave session request.
        /// </summary>
        public Utf8String SessionName { get; set; }

        public Result? GetResultCode()
        {
            return null;
        }

        internal void Set(ref LeaveSessionRequestedCallbackInfoInternal other)
        {
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            SessionName = other.SessionName;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LeaveSessionRequestedCallbackInfoInternal : ICallbackInfoInternal, IGettable<LeaveSessionRequestedCallbackInfo>, ISettable<LeaveSessionRequestedCallbackInfo>, System.IDisposable
    {
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_SessionName;

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

        public ProductUserId LocalUserId
        {
            get
            {
                ProductUserId value;
                Helper.Get(m_LocalUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String SessionName
        {
            get
            {
                Utf8String value;
                Helper.Get(m_SessionName, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_SessionName);
            }
        }

        public void Set(ref LeaveSessionRequestedCallbackInfo other)
        {
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            SessionName = other.SessionName;
        }

        public void Set(ref LeaveSessionRequestedCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
                SessionName = other.Value.SessionName;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_SessionName);
        }

        public void Get(out LeaveSessionRequestedCallbackInfo output)
        {
            output = new LeaveSessionRequestedCallbackInfo();
            output.Set(ref this);
        }
    }
}