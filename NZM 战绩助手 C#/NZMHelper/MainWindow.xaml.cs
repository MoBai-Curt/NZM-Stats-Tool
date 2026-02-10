using Newtonsoft.Json.Linq;
using NZMHelper.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NZMHelper
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _api;
        private bool _isPollingQr = false;
        private DispatcherTimer _sessionTimer;
        private bool _isDarkTheme = true;

        private List<CollectionViewModel> _currentCollection = new();
        private List<GameViewModel> _allGames = new();
        private List<GameViewModel> _currentFilteredList = new();
        private int _pageIndex = 0;
        private const int PAGE_SIZE = 20;
        private string _currentType = "weapon";

        public ICommand OpenDetailCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            _sessionTimer = new DispatcherTimer { Interval = TimeSpan.FromHours(24) };
            _sessionTimer.Tick += SessionTimer_Tick;
            this.Loaded += MainWindow_Loaded;
            _api = new ApiService();
            OpenDetailCommand = new RelayCommand(param => { if (param is string rid) new MatchDetailWindow(rid, _api).Show(); });
            DataContext = this;
        }

        private int SafeParseInt(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (int.TryParse(input, out int result)) return result;
            return 0;
        }

        private void BtnSwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            string themeFile = _isDarkTheme ? "Themes/ThemeDark.xaml" : "Themes/ThemeIOS.xaml";
            try
            {
                var appResources = Application.Current.Resources;
                var oldTheme = appResources.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.ToString().Contains("Themes/"));
                if (oldTheme != null) appResources.MergedDictionaries.Remove(oldTheme);
                appResources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri(themeFile, UriKind.Relative) });
            }
            catch { }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;
            string announcement = "公告加载中...";
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var jsonStr = await client.GetStringAsync(GlobalConfig.URL_REMOTE_CONFIG + "?t=" + DateTime.Now.Ticks);
                    var json = JObject.Parse(jsonStr);
                    string remoteVer = json["LatestVersion"]?.ToString();
                    GlobalConfig.URL_UPDATE = json["DownloadUrl"]?.ToString();
                    GlobalConfig.PWD_UPDATE = json["UpdatePassword"]?.ToString();
                    string ann = json["Announcement"]?.ToString();
                    if (!string.IsNullOrEmpty(ann)) announcement = ann;

                    if (!string.IsNullOrEmpty(remoteVer) && remoteVer != GlobalConfig.CURRENT_VERSION)
                    {
                        if (MessageBox.Show($"发现新版本 {remoteVer}！\n密码已复制。\n是否更新？", "更新提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Clipboard.SetText(GlobalConfig.PWD_UPDATE);
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(GlobalConfig.URL_UPDATE) { UseShellExecute = true });
                            Application.Current.Shutdown();
                            return;
                        }
                    }
                }
            }
            catch { announcement = "网络连接失败"; }

            var notice = new NoticeWindow(announcement);
            if (notice.ShowDialog() != true)
            {
                Application.Current.Shutdown();
                return;
            }

            if (_api.LoadLocalSession()) { OnLoginSuccess(); }
        }

        private void OnLoginSuccess()
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
            _sessionTimer.Start();
            _ = LoadDataAsync();
            OpenFragmentWindow();
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            _sessionTimer.Stop();
            MessageBox.Show("登录凭证已过期", "提示");
            BtnLogout_Click(null, null);
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"当前版本: {GlobalConfig.CURRENT_VERSION}\n点击“是”前往网盘。\n密码 [{GlobalConfig.PWD_UPDATE}] 将自动复制。", "检查更新", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Clipboard.SetText(GlobalConfig.PWD_UPDATE);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(GlobalConfig.URL_UPDATE) { UseShellExecute = true });
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _api.Logout();
            _isPollingQr = false;
            _sessionTimer.Stop();
            MainGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        private void BtnLoginManual_Click(object sender, RoutedEventArgs e)
        {
            _api.SetManualCookie(WxCookieInput.Text.Trim(), "wx");
            OnLoginSuccess();
        }

        private async void BtnGetQr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bytes = await _api.GetQrCodeAsync();
                using (var ms = new System.IO.MemoryStream(bytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    QrImage.Source = bitmap;
                }
                QrStatusText.Text = "请使用手机QQ扫码";
                _isPollingQr = true;
                _ = PollQrStatus();
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
                    _isPollingQr = false;
                    Dispatcher.Invoke(OnLoginSuccess);
                    break;
                }
                await Task.Delay(2000);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                string zone = _api.AuthType == "qq" ? "QQ区" : "微信区";
                if (TxtAppTitle != null) TxtAppTitle.Text = $"NZM Helper ({zone})";

                var summary = await _api.GetUserSummary();
                if (summary == null)
                {
                    _sessionTimer.Stop();
                    MessageBox.Show("凭证失效", "错误");
                    BtnLogout_Click(null, null);
                    return;
                }

                TxtTotalGames.Text = summary["huntGameCount"]?.ToString() ?? "--";
                int mins = SafeParseInt(summary["playtime"]?.ToString());
                TxtTotalHours.Text = $"{mins / 60}时";

                var games = await _api.GetHistoryAsync();
                ProcessStats(games);
            }
            catch (Exception ex) { MessageBox.Show("数据异常: " + ex.Message); }
        }

        private async void ProcessStats(List<JToken> games)
        {
            _allGames.Clear();
            int totalWins = 0; long totalDmg = 0;

            var mapData = new Dictionary<string, (int Total, int Wins, string Mode, string Icon, Dictionary<string, (int T, int W)> Diffs)>();
            var modeStats = new Dictionary<string, (int Total, int Wins)>();

            foreach (var g in games)
            {
                string mid = g["iMapId"]?.ToString();
                bool isWin = g["iIsWin"]?.ToString() == "1";
                string mode = GlobalConfig.GetModeName(mid);
                string diff = GlobalConfig.DiffMap.GetValueOrDefault(g["iSubModeType"]?.ToString(), "普通");

                int score = SafeParseInt(g["iScore"]?.ToString());
                int duration = SafeParseInt(g["iDuration"]?.ToString());

                var vm = new GameViewModel
                {
                    RoomId = g["DsRoomId"]?.ToString(),
                    Result = isWin ? "胜利" : "失败",
                    ResultColor = isWin ? GlobalConfig.Colors.Red : System.Windows.Media.Brushes.Gray,
                    Score = score.ToString("N0"),
                    Duration = $"{duration / 60}分{duration % 60}秒",
                    Mode = mode,
                    MapIcon = await GlobalConfig.LoadImageAsync(GlobalConfig.MapConfig.ContainsKey(mid) ? GlobalConfig.MapConfig[mid].Icon : ""),
                    DetailText = $"{GlobalConfig.MapConfig.GetValueOrDefault(mid, ("未知", "", "")).Item1} - {diff} {g["dtGameStartTime"]?.ToString()?.Substring(5, 11)}"
                };
                _allGames.Add(vm);

                if (isWin) totalWins++;
                totalDmg += score;

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

            _currentFilteredList = new List<GameViewModel>(_allGames);
            _pageIndex = 0;
            RenderGames();

            TxtRecentCount.Text = games.Count.ToString();
            TxtRecentWinRate.Text = games.Count > 0 ? $"{(double)totalWins / games.Count * 100:F1}%" : "0%";
            TxtRecentAvgDmg.Text = games.Count > 0 ? (totalDmg / games.Count).ToString("N0") : "0";

            var msList = new List<ModeStatViewModel>();
            foreach (var kv in modeStats)
                msList.Add(new ModeStatViewModel { Name = kv.Key, TotalText = $"{kv.Value.Total} 场", DetailText = $"胜{kv.Value.Wins} {(kv.Value.Total > 0 ? (double)kv.Value.Wins / kv.Value.Total * 100 : 0):F0}%" });
            ModeStatList.ItemsSource = msList;

            var mapList = new List<MapStatViewModel>();
            foreach (var kv in mapData)
            {
                var dList = new List<DiffStat>();
                foreach (var d in kv.Value.Diffs)
                {
                    double winRate = d.Value.T > 0 ? (double)d.Value.W / d.Value.T * 100 : 0;
                    dList.Add(new DiffStat { DisplayText = $"{d.Key} {d.Value.T}场 ({winRate:F0}%)" });
                }
                mapList.Add(new MapStatViewModel
                {
                    Name = kv.Key,
                    Mode = kv.Value.Mode,
                    Icon = await GlobalConfig.LoadImageAsync(kv.Value.Icon),
                    TotalWinRate = $"总胜率 {(int)((double)kv.Value.Wins / kv.Value.Total * 100)}%",
                    Diffs = dList
                });
            }
            MapStatList.ItemsSource = mapList;
        }

        private void FilterGames_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string tag = btn.Tag.ToString();

            if (btn.Parent is StackPanel sp)
            {
                foreach (var child in sp.Children)
                    if (child is Button b)
                    {
                        b.Background = System.Windows.Media.Brushes.Transparent;
                        b.SetResourceReference(Control.ForegroundProperty, "TextSubBrush");
                    }
            }
            btn.SetResourceReference(Control.BackgroundProperty, "CardBgBrush");
            btn.SetResourceReference(Control.ForegroundProperty, "TextMainBrush");

            switch (tag)
            {
                case "hunt": _currentFilteredList = _allGames.Where(x => x.Mode.Contains("僵尸") || x.Mode.Contains("猎场")).ToList(); break;
                case "td": _currentFilteredList = _allGames.Where(x => x.Mode.Contains("塔防")).ToList(); break;
                default: _currentFilteredList = new List<GameViewModel>(_allGames); break;
            }
            _pageIndex = 0;
            RenderGames();
        }

        private void RenderGames()
        {
            int totalPages = (int)Math.Ceiling((double)_currentFilteredList.Count / PAGE_SIZE);
            if (totalPages == 0) totalPages = 1;
            if (_pageIndex >= totalPages) _pageIndex = totalPages - 1;
            if (_pageIndex < 0) _pageIndex = 0;

            GamesList.ItemsSource = _currentFilteredList.Skip(_pageIndex * PAGE_SIZE).Take(PAGE_SIZE).ToList();
            TxtPageInfo.Text = $"{_pageIndex + 1} / {totalPages}";
        }

        private void BtnPagePrev_Click(object sender, RoutedEventArgs e) { if (_pageIndex > 0) { _pageIndex--; RenderGames(); } }
        private void BtnPageNext_Click(object sender, RoutedEventArgs e) { if (_pageIndex < (int)Math.Ceiling((double)_currentFilteredList.Count / PAGE_SIZE) - 1) { _pageIndex++; RenderGames(); } }

        private async void SwitchColl_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Parent is StackPanel sp)
            {
                foreach (var child in sp.Children)
                    if (child is Button b)
                    {
                        b.Background = System.Windows.Media.Brushes.Transparent;
                        b.SetResourceReference(Control.ForegroundProperty, "TextSubBrush");
                    }
            }
            btn.SetResourceReference(Control.BackgroundProperty, "CardBgBrush");
            btn.SetResourceReference(Control.ForegroundProperty, "TextMainBrush");

            await LoadCollectionData(btn.Tag.ToString());
        }

        private async Task LoadCollectionData(string type)
        {
            _currentType = type;
            TxtCollTitle.Text = "加载中...";
            var items = await _api.GetCollection(type);
            var bag = new ConcurrentBag<CollectionViewModel>();

            using (var semaphore = new SemaphoreSlim(20))
            {
                var tasks = items.Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        string url = item["pic"]?.ToString() ?? item["icon"]?.ToString();
                        string name = item["weaponName"]?.ToString() ?? item["itemName"]?.ToString() ?? item["trapName"]?.ToString();
                        int q = item["quality"]?.Value<int>() ?? 1;
                        bool owned = item["owned"]?.Value<bool>() ?? false;

                        bag.Add(new CollectionViewModel
                        {
                            Name = name,
                            Icon = await GlobalConfig.LoadImageAsync(url),
                            Quality = q,
                            Owned = owned,
                            BgColor = owned ? GlobalConfig.Colors.CardBg : System.Windows.Media.Brushes.Black,
                            BorderColor = owned ? GlobalConfig.GetQualityBrush(q) : System.Windows.Media.Brushes.DarkGray,
                            TextColor = owned ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Gray,
                            QualityText = GetQualityText(q)
                        });
                    }
                    finally { semaphore.Release(); }
                });
                await Task.WhenAll(tasks);
            }
            _currentCollection = bag.ToList();
            UpdateCollectionFilter("all");
            TxtCollTitle.Text = $"图鉴进度 ({_currentCollection.Count(x => x.Owned)}/{_currentCollection.Count})";
        }

        private void FilterColl_Click(object sender, RoutedEventArgs e) => UpdateCollectionFilter((sender as Button).Tag.ToString());
        private void UpdateCollectionFilter(string tag) { var list = tag == "all" ? _currentCollection : _currentCollection.Where(x => x.Quality.ToString() == tag).ToList(); list = list.OrderByDescending(x => x.Owned).ThenByDescending(x => x.Quality).ToList(); CollectionList.ItemsSource = list; }
        private string GetQualityText(int q) => q switch { 4 => "传说", 3 => "史诗", 2 => "稀有", 1 => "精良", _ => "普通" };

        private void BtnAuthorInfo_Click(object sender, RoutedEventArgs e) { new NoticeWindow(GlobalConfig.STATIC_ANNOUNCEMENT).Show(); }
        private void BtnOpenFragment_Click(object sender, RoutedEventArgs e) => OpenFragmentWindow();
        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => _ = LoadDataAsync();

        private void OpenFragmentWindow()
        {
            try { var fw = new FragmentWindow(_api); fw.Left = SystemParameters.PrimaryScreenWidth - fw.Width - 50; fw.Top = 100; fw.Show(); } catch { }
        }

        private void Nav_Stats_Click(object sender, RoutedEventArgs e) { if (ViewStats != null && ViewCollection != null) { ViewStats.Visibility = Visibility.Visible; ViewCollection.Visibility = Visibility.Collapsed; } }
        private void Nav_Coll_Click(object sender, RoutedEventArgs e) { if (ViewStats != null && ViewCollection != null) { ViewStats.Visibility = Visibility.Collapsed; ViewCollection.Visibility = Visibility.Visible; if (_currentCollection.Count == 0) _ = LoadCollectionData("weapon"); } }
    }
}