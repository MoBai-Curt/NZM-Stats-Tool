using Newtonsoft.Json.Linq;
using NZMHelper.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NZMHelper
{
    public partial class FragmentWindow : Window
    {
        private readonly ApiService _api;

        public FragmentWindow(ApiService api)
        {
            InitializeComponent();
            _api = api;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var list = await _api.GetHomeCollection();
                if (list == null || list.Count == 0)
                {
                    TxtEmpty.Visibility = Visibility.Visible;
                    return;
                }

                var vms = new List<FragmentItemViewModel>();
                foreach (var item in list)
                {
                    int cur = item["itemProgress"]?["current"]?.Value<int>() ?? 0;
                    int req = item["itemProgress"]?["required"]?.Value<int>() ?? 1;
                    int quality = item["quality"]?.Value<int>() ?? 1;

                    vms.Add(new FragmentItemViewModel
                    {
                        Name = item["weaponName"]?.ToString(),
                        Icon = await GlobalConfig.LoadImageAsync(item["pic"]?.ToString()),
                        QualityColor = GlobalConfig.GetQualityBrush(quality),
                        ProgressPercent = (double)cur / req * 100,
                        ProgressText = $"{cur}/{req}"
                    });
                }
                FragmentList.ItemsSource = vms;
            }
            catch
            {
                TxtEmpty.Text = "加载失败";
                TxtEmpty.Visibility = Visibility.Visible;
            }
        }
    }
}