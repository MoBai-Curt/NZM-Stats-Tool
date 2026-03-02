using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace NZMHelper.Core
{
    public static class BrowserFix
    {
        public static void SetWebBrowserFeatures()
        {
            try
            {
                var fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

                SetFeatureBrowserEmulation(fileName);
            }
            catch { }
        }

        private static void SetFeatureBrowserEmulation(string fileName)
        {
            const int mode = 11001;

            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
            {
                if (key != null)
                {
                    key.SetValue(fileName, mode, RegistryValueKind.DWord);
                    key.SetValue(fileName.Replace(".exe", ".vshost.exe"), mode, RegistryValueKind.DWord);
                }
            }
        }
    }
}