// <copyright file="Video.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Runtime.InteropServices;

namespace Video.Sandbox
{
    public class Video
    {
        [DllImport("/cpp/welsdec", CallingConvention = CallingConvention.Cdecl, SetLastError = true, EntryPoint = "WelsCreateDecoder")]
        private static extern void WelsCreateDecoder([MarshalAs(UnmanagedType.LPStr)]string data);

        [DllImport("/cpp/welsdec", CallingConvention = CallingConvention.Cdecl, SetLastError = true, EntryPoint = "WelsDestroyDecoder")]
        private static extern void WelsDestroyDecoder([MarshalAs(UnmanagedType.LPStr)]string data);

        [DllImport("/cpp/welsdec", CallingConvention = CallingConvention.Cdecl, SetLastError = true, EntryPoint = "WelsGetDecoderCapability")]
        private static extern void WelsGetDecoderCapability([MarshalAs(UnmanagedType.LPStr)]string data);

        public void Foo()
        {
            // var mfr = new MediaFrameReader();
        }
    }
}
