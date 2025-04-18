// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Friends
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="FriendsInterface.RejectInvite" />
    /// </summary>
    /// <param name="data">A <see cref="RejectInviteCallbackInfo" /> containing output information and the result.</param>
    public delegate void OnRejectInviteCallback(ref RejectInviteCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnRejectInviteCallbackInternal(ref RejectInviteCallbackInfoInternal data);
}