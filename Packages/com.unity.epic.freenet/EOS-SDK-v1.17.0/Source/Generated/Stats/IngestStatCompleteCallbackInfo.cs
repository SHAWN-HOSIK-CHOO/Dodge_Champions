// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Stats
{
    /// <summary>
    /// Data containing the result information for an ingest stat request.
    /// 
    /// NOTE: A result code of <see cref="Result.Success" /> indicates the ingest request
    /// reached the server successfully, but does not guarantee successful processing.
    /// For example, if an incorrect StatName value is provided in the ingest call,
    /// processing may still fail.
    /// </summary>
    public struct IngestStatCompleteCallbackInfo : ICallbackInfo
    {
        /// <summary>
        /// Result code for the operation. <see cref="Result.Success" /> is returned for a successful request, other codes indicate an error.
        /// </summary>
        public Result ResultCode { get; set; }

        /// <summary>
        /// Context that was passed into <see cref="StatsInterface.IngestStat" />.
        /// </summary>
        public object ClientData { get; set; }

        /// <summary>
        /// The Product User ID for the user requesting the ingest
        /// </summary>
        public ProductUserId LocalUserId { get; set; }

        /// <summary>
        /// The Product User ID for the user whose stat is being ingested
        /// </summary>
        public ProductUserId TargetUserId { get; set; }

        public Result? GetResultCode()
        {
            return ResultCode;
        }

        internal void Set(ref IngestStatCompleteCallbackInfoInternal other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct IngestStatCompleteCallbackInfoInternal : ICallbackInfoInternal, IGettable<IngestStatCompleteCallbackInfo>, ISettable<IngestStatCompleteCallbackInfo>, System.IDisposable
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

        public void Set(ref IngestStatCompleteCallbackInfo other)
        {
            ResultCode = other.ResultCode;
            ClientData = other.ClientData;
            LocalUserId = other.LocalUserId;
            TargetUserId = other.TargetUserId;
        }

        public void Set(ref IngestStatCompleteCallbackInfo? other)
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

        public void Get(out IngestStatCompleteCallbackInfo output)
        {
            output = new IngestStatCompleteCallbackInfo();
            output.Set(ref this);
        }
    }
}