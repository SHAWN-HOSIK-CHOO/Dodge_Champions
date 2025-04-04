// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatServer
{
    /// <summary>
    /// Optional callback issued when a connected client's authentication status has changed.
    /// This callback is always issued from within <see cref="Platform.PlatformInterface.Tick" /> on its calling thread.
    /// </summary>
    public delegate void OnClientAuthStatusChangedCallback(ref AntiCheatCommon.OnClientAuthStatusChangedCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnClientAuthStatusChangedCallbackInternal(ref AntiCheatCommon.OnClientAuthStatusChangedCallbackInfoInternal data);
}