// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.CustomInvites
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="CustomInvitesInterface.RejectRequestToJoin" />
    /// </summary>
    /// <param name="data">A <see cref="OnRejectRequestToJoinCallback" /> containing the output information and result</param>
    public delegate void OnRejectRequestToJoinCallback(ref RejectRequestToJoinCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnRejectRequestToJoinCallbackInternal(ref RejectRequestToJoinCallbackInfoInternal data);
}