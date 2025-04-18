// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Platform
{
    /// <summary>
    /// Function prototype type definition for functions that allocate memory.
    /// 
    /// Functions passed to <see cref="PlatformInterface.Initialize" /> to serve as memory allocators should return a pointer to the allocated memory.
    /// 
    /// The returned pointer should have at least SizeInBytes available capacity and the memory address should be a multiple of Alignment.
    /// The SDK will always call the provided function with an Alignment that is a power of 2.
    /// Allocation failures should return a null pointer.
    /// </summary>
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(Config.LibraryCallingConvention)]
    public delegate System.IntPtr AllocateMemoryFunc(System.UIntPtr sizeInBytes, System.UIntPtr alignment);
}