// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
    /// <summary>
    /// Callback for completion of query output devices information request.
    /// </summary>
    public delegate void OnQueryOutputDevicesInformationCallback(ref OnQueryOutputDevicesInformationCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnQueryOutputDevicesInformationCallbackInternal(ref OnQueryOutputDevicesInformationCallbackInfoInternal data);
}