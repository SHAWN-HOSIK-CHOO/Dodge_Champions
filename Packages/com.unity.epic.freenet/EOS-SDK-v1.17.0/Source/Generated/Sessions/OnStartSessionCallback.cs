// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="SessionsInterface.StartSession" />
    /// </summary>
    /// <param name="data">A <see cref="StartSessionCallbackInfo" /> containing the output information and result</param>
    public delegate void OnStartSessionCallback(ref StartSessionCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnStartSessionCallbackInternal(ref StartSessionCallbackInfoInternal data);
}