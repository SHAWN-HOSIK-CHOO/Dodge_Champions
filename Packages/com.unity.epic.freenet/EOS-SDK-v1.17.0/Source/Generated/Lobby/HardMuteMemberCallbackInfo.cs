// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Lobby
{
    /// <summary>
    /// Output parameters for the <see cref="LobbyInterface.HardMuteMember" /> function.
    /// </summary>
    public struct HardMuteMemberCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// The <see cref="Result" /> code for the operation. <see cref="Result.Success" /> indicates that the operation succeeded; other codes indicate errors.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="LobbyInterface.HardMuteMember" />
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The ID of the lobby
        /// </summary>
        public Utf8String LobbyId { get; set; }

        /// <summary>
        /// The Product User ID of the lobby member whose mute status has been updated
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref HardMuteMemberCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LobbyId = other.LobbyId;
            TargetUserId = other.TargetUserId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct HardMuteMemberCallbackInfoInternal : ICallbackInfoInternal, IGettable<HardMuteMemberCallbackInfo>, ISettable<HardMuteMemberCallbackInfo>, System.IDisposable
    {
        private Result m_ResultCode;
        private System.IntPtr m_ClientData;
        private System.IntPtr m_LobbyId;
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

        public Utf8String LobbyId
        {
            get
            {
                Utf8String value;
                Helper.Get(m_LobbyId, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_LobbyId);
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

        public void Set(ref HardMuteMemberCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LobbyId = other.LobbyId;
            TargetUserId = other.TargetUserId;
        }

        public void Set(ref HardMuteMemberCallbackInfo? other)
        {
            if (other.HasValue)
            {
                ResultCode = other.Value.ResultCode;
                ClientData = other.Value.ClientData;
                LobbyId = other.Value.LobbyId;
                TargetUserId = other.Value.TargetUserId;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ClientData);
            Helper.Dispose(ref m_LobbyId);
            Helper.Dispose(ref m_TargetUserId);
        }

        public void Get(out HardMuteMemberCallbackInfo output)
        {
            output = new HardMuteMemberCallbackInfo();
            output.Set(ref this);
        }
    }
}