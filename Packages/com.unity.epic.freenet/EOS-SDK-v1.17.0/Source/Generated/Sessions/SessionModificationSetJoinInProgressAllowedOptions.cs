// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
    /// <summary>
    /// Input parameters for the <see cref="SessionModification.SetJoinInProgressAllowed" /> function.
    /// </summary>
    public struct SessionModificationSetJoinInProgressAllowedOptions
    {
        /// <summary>
        /// Does the session allow join in progress
        /// </summary>
        public bool AllowJoinInProgress { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct SessionModificationSetJoinInProgressAllowedOptionsInternal : ISettable<SessionModificationSetJoinInProgressAllowedOptions>, System.IDisposable
    {
        private int m_ApiVersion;
        private int m_AllowJoinInProgress;

        public bool AllowJoinInProgress
        {
            set
            {
                Helper.Set(value, ref m_AllowJoinInProgress);
            }
        }

        public void Set(ref SessionModificationSetJoinInProgressAllowedOptions other)
        {
            m_ApiVersion = SessionModification.SessionmodificationSetjoininprogressallowedApiLatest;
            AllowJoinInProgress = other.AllowJoinInProgress;
        }

        public void Set(ref SessionModificationSetJoinInProgressAllowedOptions? other)
        {
            if (other.HasValue)
            {
                m_ApiVersion = SessionModification.SessionmodificationSetjoininprogressallowedApiLatest;
                AllowJoinInProgress = other.Value.AllowJoinInProgress;
            }
        }

        public void Dispose()
        {
        }
    }
}