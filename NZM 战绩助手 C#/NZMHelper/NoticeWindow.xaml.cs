using NZMHelper.Core;
using System.Diagnostics;
using System.Windows;

namespace NZMHelper
{
    public partial class NoticeWindow : Window
    {
        public NoticeWindow()
        {
            InitializeComponent();
        }

        private void BtnMyGithub_Click(object sender, RoutedEventArgs e) => OpenUrl(GlobalConfig.URL_MY_GITHUB);
        private void BtnOriginGithub_Click(object sender, RoutedEventArgs e) => OpenUrl(GlobalConfig.URL_ORIGIN_GITHUB);


        private void OpenUrl(string url)
        {
            try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
        }
    }
}