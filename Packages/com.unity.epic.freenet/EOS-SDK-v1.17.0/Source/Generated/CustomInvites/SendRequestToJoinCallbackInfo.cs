// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.CustomInvites
{
    /// <summary>
    /// Output parameters for the <see cref="CustomInvitesInterface.SendRequestToJoin" /> Function. These parameters are received through the callback provided to <see cref="CustomInvitesInterface.SendRequestToJoin" />
    /// </summary>
    public struct SendRequestToJoinCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="CustomInvitesInterface.SendRequestToJoin" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// Local user requesting an invite
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// Recipient of Request Invite
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref SendRequestToJoinCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct SendRequestToJoinCallbackInfoInternal : ICallbackInfoInternal, IGettable<SendRequestToJoinCallbackInfo>, ISettable<SendRequestToJoinCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_TargetUserId;

        public Result ResultCode
        {
            get
            {
                return m_ResultCode;
            }

            set
            {
                m_ResultCode = value;
            }
        }

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

        public ProductUserId TargetUserId
        {
            get
            {
                ProductUserId value;
                Helper.Get(m_TargetUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_TargetUserId);
            }
        }

        public void Set(ref SendRequestToJoinCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
        }

        public void Set(ref SendRequestToJoinCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
                TargetUserId = other.Value.TargetUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_TargetUserId);
        }

        public void Get(out SendRequestToJoinCallbackInfo output)
        {
            output = new SendRequestToJoinCallbackInfo();
            output.Set(ref this);
        }
    }
}