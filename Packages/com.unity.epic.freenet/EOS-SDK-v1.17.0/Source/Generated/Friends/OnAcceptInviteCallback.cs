// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Friends
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="FriendsInterface.AcceptInvite" />
    /// </summary>
    /// <param name="data">A <see cref="AcceptInviteCallbackInfo" /> containing the output information and result.</param>
    public delegate void OnAcceptInviteCallback(ref AcceptInviteCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnAcceptInviteCallbackInternal(ref AcceptInviteCallbackInfoInternal data);
}