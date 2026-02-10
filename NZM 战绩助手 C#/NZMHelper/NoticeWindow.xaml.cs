using System;
using System.Windows;

namespace NZMHelper
{
    public partial class NoticeWindow : Window
    {
        public NoticeWindow(string content = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(content))
            {
                TxtContent.Text = content;
            }
        }

        private void BtnAgree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogResult = true;
            }
            catch (InvalidOperationException)
            {
            }
            finally
            {
                this.Close();
            }
        }
    }
}