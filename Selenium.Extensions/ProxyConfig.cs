using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.Extensions
{
    public class ProxyConfig
    {
        private const uint InternetOptionProxy = 38;

        public static InternetProxyInfo GetProxyInformation()
        {
            var bufferLength = 0;
            InternetQueryOption(IntPtr.Zero, InternetOptionProxy, IntPtr.Zero, ref bufferLength);
            var buffer = IntPtr.Zero;
            try
            {
                buffer = Marshal.AllocHGlobal(bufferLength);


                if (InternetQueryOption(IntPtr.Zero, InternetOptionProxy, buffer, ref bufferLength))
                {
                    var ipi = (InternetProxyInfo) Marshal.PtrToStructure(buffer, typeof (InternetProxyInfo));
                    //Debug.WriteLine(ipi.AccessType);
                    //Debug.WriteLine(ipi.ProxyAddress);
                    //Debug.WriteLine(ipi.ProxyBypass);
                    return ipi;
                }
                else
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero) Marshal.FreeHGlobal(buffer);
            }
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption, IntPtr lpBuffer,
            ref int lpdwBufferLength);
    }

    public enum InternetOpenType
    {
        Preconfig = 0,
        Direct = 1,
        Proxy = 3,
        PreconfigWithNoAutoProxy = 4
    }

    public struct InternetProxyInfo
    {
        public InternetOpenType AccessType;
        public string ProxyAddress;
        public string ProxyBypass;
    }
}