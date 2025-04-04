// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.KWS
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="KWSInterface.CreateUser" />
    /// </summary>
    /// <param name="data">A <see cref="CreateUserCallbackInfo" /> containing the output information and result</param>
    public delegate void OnCreateUserCallback(ref CreateUserCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnCreateUserCallbackInternal(ref CreateUserCallbackInfoInternal data);
}