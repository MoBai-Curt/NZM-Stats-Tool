using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NZMHelper.Core
{
    public static class CookieHelper
    {

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int flags,
            IntPtr reserved);

        private const int InternetCookieHttponly = 0x2000;

        public static string GetGlobalCookies(string url)
        {
            int size = 0;
            InternetGetCookieEx(url, null, null, ref size, InternetCookieHttponly, IntPtr.Zero);
            if (size <= 0) return null;

            StringBuilder sb = new StringBuilder(size);
            if (InternetGetCookieEx(url, null, sb, ref size, InternetCookieHttponly, IntPtr.Zero))
            {
                return sb.ToString();
            }
            return null;
        }
    }
}