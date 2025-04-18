// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="EcomInterface.QueryOwnershipBySandboxIds" />
    /// </summary>
    /// <param name="data">A <see cref="QueryOwnershipBySandboxIdsCallbackInfo" /> containing the output information and result</param>
    public delegate void OnQueryOwnershipBySandboxIdsCallback(ref QueryOwnershipBySandboxIdsCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnQueryOwnershipBySandboxIdsCallbackInternal(ref QueryOwnershipBySandboxIdsCallbackInfoInternal data);
}