// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.CustomInvites
{
    /// <summary>
    /// Output parameters for the <see cref="CustomInvitesInterface.SendCustomInvite" /> Function. These parameters are received through the callback provided to <see cref="CustomInvitesInterface.SendCustomInvite" />
    /// </summary>
    public struct SendCustomInviteCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="CustomInvitesInterface.SendCustomInvite" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// Local user sending a CustomInvite
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// Users to whom the invites were successfully sent (can be different than original call if an invite for same Payload was previously sent)
        /// </summary>
        public ProductUserId[] TargetUserIds { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref SendCustomInviteCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TargetUserIds = other.TargetUserIds;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct SendCustomInviteCallbackInfoInternal : ICallbackInfoInternal, IGettable<SendCustomInviteCallbackInfo>, ISettable<SendCustomInviteCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_TargetUserIds;
        private uint m_TargetUserIdsCount;

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

        public ProductUserId[] TargetUserIds
        {
            get
            {
                ProductUserId[] value;
                Helper.GetHandle(m_TargetUserIds, out value, m_TargetUserIdsCount);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_TargetUserIds, out m_TargetUserIdsCount);
            }
        }

        public void Set(ref SendCustomInviteCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TargetUserIds = other.TargetUserIds;
        }

        public void Set(ref SendCustomInviteCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
                TargetUserIds = other.Value.TargetUserIds;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_TargetUserIds);
        }

        public void Get(out SendCustomInviteCallbackInfo output)
        {
            output = new SendCustomInviteCallbackInfo();
            output.Set(ref this);
        }
    }
}