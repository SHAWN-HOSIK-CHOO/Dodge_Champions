// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatCommon
{
    public struct RegisterEventParamDef
    {
        /// <summary>
        /// Parameter name. Allowed characters are 0-9, A-Z, a-z, '_', '-'
        /// </summary>
        public Utf8String ParamName { get; set; }

        /// <summary>
        /// Parameter type
        /// </summary>
        public AntiCheatCommonEventParamType ParamType { get; set; }

        internal void Set(ref RegisterEventParamDefInternal other)
        {
            ParamName = other.ParamName;
            ParamType = other.ParamType;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    internal struct RegisterEventParamDefInternal : IGettable<RegisterEventParamDef>, ISettable<RegisterEventParamDef>, System.IDisposable
    {
        private System.IntPtr m_ParamName;
        private AntiCheatCommonEventParamType m_ParamType;

        public Utf8String ParamName
        {
            get
            {
                Utf8String value;
                Helper.Get(m_ParamName, out value);
                return value;
            }

            set
            {
                Helper.Set(value, ref m_ParamName);
            }
        }

        public AntiCheatCommonEventParamType ParamType
        {
            get
            {
                return m_ParamType;
            }

            set
            {
                m_ParamType = value;
            }
        }

        public void Set(ref RegisterEventParamDef other)
        {
            ParamName = other.ParamName;
            ParamType = other.ParamType;
        }

        public void Set(ref RegisterEventParamDef? other)
        {
            if (other.HasValue)
            {
                ParamName = other.Value.ParamName;
                ParamType = other.Value.ParamType;
            }
        }

        public void Dispose()
        {
            Helper.Dispose(ref m_ParamName);
        }

        public void Get(out RegisterEventParamDef output)
        {
            output = new RegisterEventParamDef();
            output.Set(ref this);
        }
    }
}