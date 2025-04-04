// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Connect
{
    /// <summary>
    /// Output parameters for the <see cref="ConnectInterface.Login" /> function.
    /// </summary>
    public struct LoginCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="ConnectInterface.Login" />.
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// If login was successful, this is the Product User ID of the local player that logged in.
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// If the user was not found with credentials passed into <see cref="ConnectInterface.Login" />,
        /// this continuance token can be passed to either <see cref="ConnectInterface.CreateUser" />
        /// or <see cref="ConnectInterface.LinkAccount" /> to continue the flow.
        /// </summary>
        public ContinuanceToken ContinuanceToken { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref LoginCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            ContinuanceToken = other.ContinuanceToken;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct LoginCallbackInfoInternal : ICallbackInfoInternal, IGettable<LoginCallbackInfo>, ISettable<LoginCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_ContinuanceToken;

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

        public ContinuanceToken ContinuanceToken
        {
            get
            {
                ContinuanceToken value;
                Helper.Get(m_ContinuanceToken, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ContinuanceToken);
            }
        }

        public void Set(ref LoginCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            ContinuanceToken = other.ContinuanceToken;
        }

        public void Set(ref LoginCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                ClientData = other.Value.ClientData;
                LocalUserId = other.Value.LocalUserId;
                ContinuanceToken = other.Value.ContinuanceToken;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_ContinuanceToken);
        }

        public void Get(out LoginCallbackInfo output)
        {
            output = new LoginCallbackInfo();
            output.Set(ref this);
        }
    }
}