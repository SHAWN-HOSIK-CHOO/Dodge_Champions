// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Mods
{
    /// <summary>
    /// Output parameters for the <see cref="ModsInterface.EnumerateMods" /> Function. These parameters are received through the callback provided to <see cref="ModsInterface.EnumerateMods" />
    /// </summary>
    public struct EnumerateModsCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Result code for the operation. <see cref="Result.Success" /> is returned if the enumeration was successful, otherwise one of the error codes is returned.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// The Epic Account ID of the user for which mod enumeration was requested
        /// </summary>
        public EpicAccountId LocalUserId { get; set; }

        /// <summary>
        /// Context that is passed into <see cref="ModsInterface.EnumerateMods" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// Type of the enumerated mods
        /// </summary>
        public ModEnumerationType Type { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref EnumerateModsCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            LocalUserId = other.LocalUserId;
            ClientData = other.ClientData;
            Type = other.Type;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct EnumerateModsCallbackInfoInternal : ICallbackInfoInternal, IGettable<EnumerateModsCallbackInfo>, ISettable<EnumerateModsCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_LocalUserId;
        private System.IntPtr m_ClientData;
        private ModEnumerationType m_Type;

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

        public ModEnumerationType Type
        {
            get
            {
                return m_Type;
            }

            set
            {
                m_Type = value;
            }
        }

        public void Set(ref EnumerateModsCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            LocalUserId = other.LocalUserId;
            ClientData = other.ClientData;
            Type = other.Type;
        }

        public void Set(ref EnumerateModsCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                LocalUserId = other.Value.LocalUserId;
                ClientData = other.Value.ClientData;
                Type = other.Value.Type;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_LocalUserId);
            Helper.Dispose(ref m_ClientData);
        }

        public void Get(out EnumerateModsCallbackInfo output)
        {
            output = new EnumerateModsCallbackInfo();
            output.Set(ref this);
        }
    }
}