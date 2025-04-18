// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.KWS
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="KWSInterface.UpdateParentEmail" />
    /// </summary>
    /// <param name="data">A <see cref="UpdateParentEmailCallbackInfo" /> containing the output information and result</param>
    public delegate void OnUpdateParentEmailCallback(ref UpdateParentEmailCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnUpdateParentEmailCallbackInternal(ref UpdateParentEmailCallbackInfoInternal data);
}