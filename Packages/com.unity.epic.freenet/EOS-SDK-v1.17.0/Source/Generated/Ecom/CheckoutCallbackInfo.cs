// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Output parameters for the <see cref="EcomInterface.Checkout" /> Function.
    /// </summary>
    public struct CheckoutCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Result code for the operation. <see cref="Result.Success" /> is returned for a successful request, otherwise one of the error codes is returned. See eos_common.h
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="EcomInterface.Checkout" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The Epic Account ID of the user who initiated the purchase
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }

        /// <summary>
        /// The transaction ID which can be used to obtain an <see cref="Transaction" /> using <see cref="EcomInterface.CopyTransactionById" />.
        /// </summary>
        public Utf8String TransactionId { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref CheckoutCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TransactionId = other.TransactionId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct CheckoutCallbackInfoInternal : ICallbackInfoInternal, IGettable<CheckoutCallbackInfo>, ISettable<CheckoutCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_TransactionId;

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

        public EpicAccountId LocalUserId
        {
            get
            {
                EpicAccountId value;
                Helper.Get(m_LocalUserId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_LocalUserId);
            }
        }

        public Utf8String TransactionId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_TransactionId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_TransactionId);
            }
        }

        public void Set(ref CheckoutCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TransactionId = other.TransactionId;
        }

        public void Set(ref CheckoutCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
                TransactionId = other.Value.TransactionId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_TransactionId);
        }

        public void Get(out CheckoutCallbackInfo output)
        {
            output = new CheckoutCallbackInfo();
            output.Set(ref this);
        }
    }
}