using Newtonsoft.Json.Linq;
using NZMHelper.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NZMHelper
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _api;
        private bool _isPollingQr = false;
        private List<CollectionViewModel> _currentCollection = new();
        private string _currentType = "weapon";


        private const string UPDATE_PWD = "bx9d";
        private const string UPDATE_URL = "https://wwaoi.lanzouu.com/b019vntx7a";

        public ICommand OpenDetailCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            _api = new ApiService();
            OpenDetailCommand = new RelayCommand(param => { if (param is string rid) new MatchDetailWindow(rid, _api).Show(); });
            DataContext = this;
            if (_api.LoadLocalSession()) { LoginGrid.Visibility = Visibility.Collapsed; MainGrid.Visibility = Visibility.Visible; _ = LoadDataAsync(); }
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"当前版本: {GlobalConfig.CURRENT_VERSION}\n发现新版本！\n\n点击【是】将自动复制密码 [{UPDATE_PWD}] 并跳转下载。", "检查更新", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Clipboard.SetText(UPDATE_PWD);
                try { Process.Start(new ProcessStartInfo(UPDATE_URL) { UseShellExecute = true }); } catch { }
            }
        }

        private void BtnAuthorInfo_Click(object sender, RoutedEventArgs e) => new NoticeWindow().Show();

        private async void BtnGetQr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bytes = await _api.GetQrCodeAsync();
                using (var ms = new System.IO.MemoryStream(bytes))
                {
                    var bitmap = new BitmapImage(); bitmap.BeginInit(); bitmap.StreamSource = ms; bitmap.CacheOption = BitmapCacheOption.OnLoad; bitmap.EndInit();
                    QrImage.Source = bitmap;
                }
                QrStatusText.Text = "请使用手机QQ扫码"; _isPollingQr = true; _ = PollQrStatus();
            }
            catch { MessageBox.Show("获取失败"); }
        }

        private async Task PollQrStatus()
        {
            while (_isPollingQr)
            {
                int status = await _api.CheckQrStatusAsync();
                if (status == 0)
                {
                    _isPollingQr = false; Dispatcher.Invoke(() => { LoginGrid.Visibility = Visibility.Collapsed; MainGrid.Visibility = Visibility.Visible; _ = LoadDataAsync(); }); break;
                }
                else if (status == 65) { Dispatcher.Invoke(() => QrStatusText.Text = "二维码过期"); _isPollingQr = false; break; }
                await Task.Delay(2000);
            }
        }

        private void BtnLoginManual_Click(object sender, RoutedEventArgs e)
        {
            string cookie = WxCookieInput.Text.Trim();
            if (string.IsNullOrEmpty(cookie) || !cookie.Contains("ieg_ams_token"))
            {
                if (MessageBox.Show("Cookie似乎不完整，是否继续？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }
            _api.SetManualCookie(cookie, "wx");
            LoginGrid.Visibility = Visibility.Collapsed; MainGrid.Visibility = Visibility.Visible; _ = LoadDataAsync();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _api.Logout(); _isPollingQr = false;
            MainGrid.Visibility = Visibility.Collapsed; LoginGrid.Visibility = Visibility.Visible;
            QrImage.Source = null; QrStatusText.Text = "点击获取二维码";
            Title = "NZM 战绩助手 V1.2 (双区版)";
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => _ = LoadDataAsync();

        private async Task LoadDataAsync()
        {
            try
            {
                string zone = _api.AuthType == "qq" ? "QQ区" : "微信区";
                TxtAppTitle.Text = $"NZM 战绩 ({zone})";

                var summary = await _api.GetUserSummary();
                if (summary != null) { TxtTotalGames.Text = summary["huntGameCount"]?.ToString(); TxtTotalHours.Text = ((summary["playtime"]?.Value<int>() ?? 0) / 60) + "时"; }

                var games = await _api.GetHistoryAsync();
                ProcessStats(games);
                await LoadCollectionData("weapon");
            }
            catch (Exception ex) { MessageBox.Show("加载失败: " + ex.Message); }
        }

        private async void ProcessStats(List<JToken> games)
        {
            var gameVms = new List<GameViewModel>();
            var mapData = new Dictionary<string, (int Total, int Wins, string Mode, string Icon, Dictionary<string, (int T, int W)> Diffs)>();
            int totalWins = 0; long totalDmg = 0;
            var modeStats = new Dictionary<string, (int Total, int Wins)>();

            foreach (var g in games)
            {
                string mid = g["iMapId"]?.ToString();
                bool isWin = g["iIsWin"]?.ToString() == "1";
                string mode = GlobalConfig.GetModeName(mid);
                string diff = GlobalConfig.DiffMap.GetValueOrDefault(g["iSubModeType"]?.ToString(), "普通");

                var vm = new GameViewModel
                {
                    RoomId = g["DsRoomId"]?.ToString(),
                    Result = isWin ? "胜利" : "失败",
                    ResultColor = isWin ? GlobalConfig.Colors.Red : Brushes.Gray,
                    Score = int.Parse(g["iScore"]?.ToString() ?? "0").ToString("N0"),
                    Duration = $"{g["iDuration"]?.Value<int>() / 60}分{g["iDuration"]?.Value<int>() % 60}秒",
                    Mode = mode,
                    MapIcon = await GlobalConfig.LoadImageAsync(GlobalConfig.MapConfig.ContainsKey(mid) ? GlobalConfig.MapConfig[mid].Icon : ""),
                    DetailText = $"{GlobalConfig.MapConfig.GetValueOrDefault(mid, ("未知", "", "")).Item1} - {diff} {g["dtGameStartTime"]?.ToString()?.Substring(5, 11)}"
                };
                gameVms.Add(vm);

                if (isWin) totalWins++;
                totalDmg += int.Parse(g["iScore"]?.ToString() ?? "0");

                if (!modeStats.ContainsKey(mode)) modeStats[mode] = (0, 0);
                modeStats[mode] = (modeStats[mode].Total + 1, isWin ? modeStats[mode].Wins + 1 : modeStats[mode].Wins);

                if (GlobalConfig.MapConfig.ContainsKey(mid))
                {
                    var conf = GlobalConfig.MapConfig[mid];
                    if (!mapData.ContainsKey(conf.Name)) mapData[conf.Name] = (0, 0, mode, conf.Icon, new Dictionary<string, (int, int)>());
                    var md = mapData[conf.Name];
                    md.Total++; if (isWin) md.Wins++;
                    if (!md.Diffs.ContainsKey(diff)) md.Diffs[diff] = (0, 0);
                    md.Diffs[diff] = (md.Diffs[diff].T + 1, isWin ? md.Diffs[diff].W + 1 : md.Diffs[diff].W);
                    mapData[conf.Name] = md;
                }
            }
            GamesList.ItemsSource = gameVms;
            TxtRecentCount.Text = games.Count.ToString();
            TxtRecentWinRate.Text = games.Count > 0 ? $"{(double)totalWins / games.Count * 100:F1}%" : "0%";
            TxtRecentAvgDmg.Text = games.Count > 0 ? (totalDmg / games.Count).ToString("N0") : "0";

            var msList = new List<ModeStatViewModel>();
            foreach (var kv in modeStats) msList.Add(new ModeStatViewModel { Name = kv.Key, TotalText = $"{kv.Value.Total} 场", DetailText = $"胜{kv.Value.Wins} 负{kv.Value.Total - kv.Value.Wins} {(kv.Value.Total > 0 ? (double)kv.Value.Wins / kv.Value.Total * 100 : 0):F0}%" });
            ModeStatList.ItemsSource = msList;

            var mapList = new List<MapStatViewModel>();
            foreach (var kv in mapData)
            {
                var dList = new List<DiffStat>();
                foreach (var d in kv.Value.Diffs) dList.Add(new DiffStat { DisplayText = $"{d.Key} {d.Value.T}场 ({(int)((double)d.Value.W / d.Value.T * 100)}%)" });
                mapList.Add(new MapStatViewModel
                {
                    Name = kv.Key,
                    Mode = kv.Value.Mode,
                    Icon = await GlobalConfig.LoadImageAsync(kv.Value.Icon),
                    TotalWinRate = $"{kv.Value.Total}场 - {(int)((double)kv.Value.Wins / kv.Value.Total * 100)}% 胜率",
                    Diffs = dList
                });
            }
            MapStatList.ItemsSource = mapList;
        }

        private async void SwitchColl_Click(object sender, RoutedEventArgs e) => await LoadCollectionData((sender as Button).Tag.ToString());
        private async Task LoadCollectionData(string type)
        {
            _currentType = type;
            var items = await _api.GetCollection(type);
            _currentCollection.Clear();
            foreach (var item in items)
            {
                string url = item["pic"]?.ToString();
                if (string.IsNullOrEmpty(url)) url = item["icon"]?.ToString();
                string name = item["weaponName"]?.ToString() ?? item["itemName"]?.ToString() ?? item["trapName"]?.ToString();
                int q = item["quality"]?.Value<int>() ?? 1;
                bool owned = item["owned"]?.Value<bool>() ?? false;
                _currentCollection.Add(new CollectionViewModel
                {
                    Name = name,
                    Icon = await GlobalConfig.LoadImageAsync(url),
                    Quality = q,
                    Owned = owned,
                    BgColor = owned ? GlobalConfig.Colors.CardBg : Brushes.Black,
                    BorderColor = owned ? GlobalConfig.GetQualityBrush(q) : Brushes.DarkGray,
                    TextColor = owned ? Brushes.White : Brushes.Gray
                });
            }
            UpdateCollectionFilter("all");
            TxtCollTitle.Text = $"{(type == "weapon" ? "武器" : (type == "trap" ? "塔防" : "插件"))}图鉴 ({_currentCollection.Count(x => x.Owned)}/{_currentCollection.Count})";
        }

        private void FilterColl_Click(object sender, RoutedEventArgs e) => UpdateCollectionFilter((sender as Button).Tag.ToString());
        private void UpdateCollectionFilter(string tag)
        {
            var list = tag == "all" ? _currentCollection : _currentCollection.Where(x => x.Quality.ToString() == tag).ToList();
            list = list.OrderBy(x => x.Owned ? 0 : 1).ThenByDescending(x => x.Quality).ToList();
            CollectionList.ItemsSource = list;
        }
    }

    public class RelayCommand : ICommand { private readonly Action<object> _execute; public RelayCommand(Action<object> e) { _execute = e; } public bool CanExecute(object p) => true; public void Execute(object p) => _execute(p); public event EventHandler CanExecuteChanged; }
    public class CollectionViewModel { public string Name { get; set; } public ImageSource Icon { get; set; } public int Quality { get; set; } public bool Owned { get; set; } public Brush BgColor { get; set; } public Brush BorderColor { get; set; } public Brush TextColor { get; set; } }
    public class MapStatViewModel { public string Name { get; set; } public ImageSource Icon { get; set; } public string Mode { get; set; } public string TotalWinRate { get; set; } public List<DiffStat> Diffs { get; set; } }
    public class DiffStat { public string DisplayText { get; set; } }
    public class GameViewModel { public string RoomId { get; set; } public ImageSource MapIcon { get; set; } public string Result { get; set; } public Brush ResultColor { get; set; } public string Mode { get; set; } public string DetailText { get; set; } public string Score { get; set; } public string Duration { get; set; } }
    public class ModeStatViewModel { public string Name { get; set; } public string TotalText { get; set; } public string DetailText { get; set; } }
    public class FragmentViewModel { public string Name { get; set; } public ImageSource Icon { get; set; } public double Progress { get; set; } public string ProgText { get; set; } }
}