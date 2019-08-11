using System;
using System.Runtime.InteropServices;
using Windows.Media.Capture.Frames;

namespace Video.Sandbox
{
    public class Video
    {
        [DllImport("/cpp/welsdec", CallingConvention = CallingConvention.Cdecl, SetLastError = true, EntryPoint = "WelsCreateDecoder")]
        static extern void WelsCreateDecoder([MarshalAs(UnmanagedType.LPStr)]string data);

        [DllImport("/cpp/welsdec", CallingConvention = CallingConvention.Cdecl, SetLastError = true, EntryPoint = "WelsDestroyDecoder")]
        static extern void WelsDestroyDecoder([MarshalAs(UnmanagedType.LPStr)]string data);

        [DllImport("/cpp/welsdec", CallingConvention = CallingConvention.Cdecl, SetLastError = true, EntryPoint = "WelsGetDecoderCapability")]
        static extern void WelsGetDecoderCapability([MarshalAs(UnmanagedType.LPStr)]string data);


        public void f()
        {
            //var mfr = new MediaFrameReader();
            
        }
    }
}
