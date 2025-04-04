// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
    /// <summary>
    /// Output parameters for the <see cref="ConnectInterface.TransferDeviceIdAccount" /> Function.
    /// </summary>
    public struct TransferDeviceIdAccountCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="ConnectInterface.TransferDeviceIdAccount" />.
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The ProductUserIdToPreserve that was passed to the original <see cref="ConnectInterface.TransferDeviceIdAccount" /> call.
        /// 
        /// On successful operation, this <see cref="ProductUserId" /> will have a valid authentication session
        /// and the other <see cref="ProductUserId" /> value has been discarded and lost forever.
        /// 
        /// The application should remove any registered notification callbacks for the discarded <see cref="ProductUserId" /> as obsolete.
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref TransferDeviceIdAccountCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct TransferDeviceIdAccountCallbackInfoInternal : ICallbackInfoInternal, IGettable<TransferDeviceIdAccountCallbackInfo>, ISettable<TransferDeviceIdAccountCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;

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

        public void Set(ref TransferDeviceIdAccountCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
        }

        public void Set(ref TransferDeviceIdAccountCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
        }

        public void Get(out TransferDeviceIdAccountCallbackInfo output)
        {
            output = new TransferDeviceIdAccountCallbackInfo();
            output.Set(ref this);
        }
    }
}