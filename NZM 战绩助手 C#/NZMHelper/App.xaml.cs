using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using Microsoft.Win32;

namespace NZMHelper
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;

            SetWebBrowserFeatures();

            base.OnStartup(e);
        }

        private static void SetWebBrowserFeatures()
        {
            try
            {
                string processName = Process.GetCurrentProcess().ProcessName + ".exe";

                const int ie11Mode = 11001;

                using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
                if (key != null)
                {
                    key.SetValue(processName, ie11Mode, RegistryValueKind.DWord);
                    key.SetValue(processName.Replace(".exe", ".vshost.exe"), ie11Mode, RegistryValueKind.DWord);
                }
            }
            catch
            {
            }
        }
    }
}