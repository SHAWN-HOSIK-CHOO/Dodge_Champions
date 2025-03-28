// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Mods
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="ModsInterface.UninstallMod" />
    /// </summary>
    /// <param name="data">A <see cref="UninstallModCallbackInfo" /> containing the output information and result</param>
    public delegate void OnUninstallModCallback(ref UninstallModCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnUninstallModCallbackInternal(ref UninstallModCallbackInfoInternal data);
}