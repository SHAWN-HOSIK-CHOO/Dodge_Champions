// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.CustomInvites
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="CustomInvitesInterface.AcceptRequestToJoin" />
    /// </summary>
    /// <param name="data">A <see cref="AcceptRequestToJoinCallbackInfo" /> containing the output information and result</param>
    public delegate void OnAcceptRequestToJoinCallback(ref AcceptRequestToJoinCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnAcceptRequestToJoinCallbackInternal(ref AcceptRequestToJoinCallbackInfoInternal data);
}