// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Platform
{
    public sealed partial class PlatformInterface : Handle
    {
        /// <summary>
        /// The most recent version of the <see cref="AndroidInitializeOptionsSystemInitializeOptions" /> structure.
        /// </summary>
        public const int AndroidInitializeoptionssysteminitializeoptionsApiLatest = 2;

        public static Result Initialize(ref AndroidInitializeOptions options)
        {
            AndroidInitializeOptionsInternal optionsInternal = new AndroidInitializeOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = AndroidBindings.EOS_Initialize(ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }
    }
}