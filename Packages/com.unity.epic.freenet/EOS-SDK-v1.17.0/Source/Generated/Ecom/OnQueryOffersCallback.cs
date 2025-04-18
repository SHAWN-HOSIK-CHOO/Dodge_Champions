// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
    /// <summary>
    /// Function prototype definition for callbacks passed to <see cref="EcomInterface.QueryOffers" />
    /// </summary>
    /// <param name="data">A <see cref="QueryOffersCallbackInfo" /> containing the output information and result</param>
    public delegate void OnQueryOffersCallback(ref QueryOffersCallbackInfo data);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    internal delegate void OnQueryOffersCallbackInternal(ref QueryOffersCallbackInfoInternal data);
}