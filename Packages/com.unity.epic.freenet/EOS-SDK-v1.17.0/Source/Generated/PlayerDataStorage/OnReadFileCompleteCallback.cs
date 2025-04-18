// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.PlayerDataStorage
{
    /// <summary>
    /// Callback for when <see cref="PlayerDataStorageInterface.ReadFile" /> completes
    /// </summary>
    public delegate void OnReadFileCompleteCallback(ref ReadFileCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnReadFileCompleteCallbackInternal(ref ReadFileCallbackInfoInternal data);
}