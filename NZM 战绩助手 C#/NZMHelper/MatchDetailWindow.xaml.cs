using Newtonsoft.Json.Linq;
using NZMHelper.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NZMHelper
{
    public partial class MatchDetailWindow : Window, INotifyPropertyChanged
    {
        private readonly string _roomId;
        private readonly ApiService _api;

        private string _selfName; public string SelfName { get { return _selfName; } set { _selfName = value; OnPropertyChanged("SelfName"); } }
        private ImageSource _selfAvatar; public ImageSource SelfAvatar { get { return _selfAvatar; } set { _selfAvatar = value; OnPropertyChanged("SelfAvatar"); } }
        private string _selfScore; public string SelfScore { get { return _selfScore; } set { _selfScore = value; OnPropertyChanged("SelfScore"); } }
        private string _selfBossDmg; public string SelfBossDmg { get { return _selfBossDmg; } set { _selfBossDmg = value; OnPropertyChanged("SelfBossDmg"); } }
        private string _selfMobsDmg; public string SelfMobsDmg { get { return _selfMobsDmg; } set { _selfMobsDmg = value; OnPropertyChanged("SelfMobsDmg"); } }
        private string _selfCoin; public string SelfCoin { get { return _selfCoin; } set { _selfCoin = value; OnPropertyChanged("SelfCoin"); } }

        private string _selfKDA; public string SelfKDA { get { return _selfKDA; } set { _selfKDA = value; OnPropertyChanged("SelfKDA"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MatchDetailWindow(string roomId, ApiService api)
        {
            InitializeComponent();
            _roomId = roomId;
            _api = api;
            DataContext = this;
            this.Loaded += MatchDetailWindow_Loaded;
        }

        private void MatchDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _ = LoadDetailAsync();
        }

        private async Task LoadDetailAsync()
        {
            try
            {
                TxtLoading.Visibility = Visibility.Visible;
                var json = await _api.GetGameDetail(_roomId);
                if (json == null) { TxtLoading.Text = "数据加载失败"; return; }

                var selfNode = json["loginUserDetail"];
                if (selfNode != null)
                {
                    SelfName = System.Net.WebUtility.UrlDecode(selfNode["nickname"]?.ToString());
                    string avatarUrl = System.Net.WebUtility.UrlDecode(selfNode["avatar"]?.ToString());
                    SelfAvatar = await GlobalConfig.LoadImageAsync(avatarUrl);

                    var baseDetail = selfNode["baseDetail"];
                    if (baseDetail != null)
                    {
                        SelfScore = SafeParseLong(baseDetail["iScore"]?.ToString()).ToString("N0");
                        int k = SafeParseInt(baseDetail["iKills"]?.ToString());
                        int d = SafeParseInt(baseDetail["iDeaths"]?.ToString());
                        SelfKDA = $"{k} / {d}";
                    }

                    var huntDetail = selfNode["huntingDetails"];
                    if (huntDetail != null)
                    {
                        SelfCoin = SafeParseLong(huntDetail["totalCoin"]?.ToString()).ToString("N0");
                        SelfBossDmg = SafeParseLong(huntDetail["damageTotalOnBoss"]?.ToString()).ToString("N0");
                        SelfMobsDmg = SafeParseLong(huntDetail["damageTotalOnMobs"]?.ToString()).ToString("N0");
                    }
                    SelfStatsBar.Visibility = Visibility.Visible;

                    var selfEquips = await ParseWeaponsAsync(selfNode["equipmentScheme"]);
                    SelfEquipList.ItemsSource = selfEquips;
                }

                var listNode = json["list"];
                if (listNode != null)
                {
                    var players = listNode.ToObject<List<JToken>>() ?? new List<JToken>();
                    var viewModels = new List<PlayerDetailViewModel>();

                    foreach (var p in players)
                    {
                        string name = System.Net.WebUtility.UrlDecode(p["nickname"]?.ToString());
                        if (name == SelfName) continue;

                        string avatar = System.Net.WebUtility.UrlDecode(p["avatar"]?.ToString());
                        var pBase = p["baseDetail"];
                        var pHunt = p["huntingDetails"];

                        long score = pBase != null ? SafeParseLong(pBase["iScore"]?.ToString()) : 0;
                        int k = pBase != null ? SafeParseInt(pBase["iKills"]?.ToString()) : 0;
                        int d = pBase != null ? SafeParseInt(pBase["iDeaths"]?.ToString()) : 0;

                        long boss = pHunt != null ? SafeParseLong(pHunt["damageTotalOnBoss"]?.ToString()) : 0;
                        long mob = pHunt != null ? SafeParseLong(pHunt["damageTotalOnMobs"]?.ToString()) : 0;
                        long coin = pHunt != null ? SafeParseLong(pHunt["totalCoin"]?.ToString()) : 0;

                        var weapons = await ParseWeaponsAsync(p["equipmentScheme"]);

                        viewModels.Add(new PlayerDetailViewModel
                        {
                            Nickname = name,
                            Avatar = await GlobalConfig.LoadImageAsync(avatar),
                            ScoreText = $"积分: {score:N0}",
                            KDAText = $"K/D: {k}/{d}",
                            BossDmgText = $"BOSS: {boss:N0}",
                            MobsDmgText = $"小怪: {mob:N0}",
                            CoinText = $"金币: {coin:N0}",
                            Weapons = weapons
                        });
                    }
                    PlayerList.ItemsSource = viewModels;
                }

                TxtLoading.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) { TxtLoading.Text = "解析错误: " + ex.Message; }
        }

        private async Task<List<WeaponMiniViewModel>> ParseWeaponsAsync(JToken equipNode)
        {
            var list = new List<WeaponMiniViewModel>();
            if (equipNode == null) return list;

            var wList = equipNode.ToObject<List<JToken>>();
            if (wList == null) return list;

            foreach (var w in wList)
            {
                string wName = w["weaponName"]?.ToString();
                string wIcon = w["pic"]?.ToString();
                int quality = SafeParseInt(w["quality"]?.ToString());

                var plugins = new List<PluginViewModel>();
                var pList = w["commonItems"]?.ToObject<List<JToken>>();
                if (pList != null)
                {
                    foreach (var pl in pList)
                    {
                        plugins.Add(new PluginViewModel
                        {
                            Name = pl["itemName"]?.ToString(),
                            Icon = await GlobalConfig.LoadImageAsync(pl["pic"]?.ToString()),
                            QualityColor = GlobalConfig.GetQualityBrush(SafeParseInt(pl["quality"]?.ToString()))
                        });
                    }
                }

                list.Add(new WeaponMiniViewModel
                {
                    Name = wName,
                    Icon = await GlobalConfig.LoadImageAsync(wIcon),
                    QualityColor = GlobalConfig.GetQualityBrush(quality),
                    QualityText = GetQualityText(quality),
                    Plugins = plugins
                });
            }
            return list;
        }

        private void BtnToggleSelfEquip_Click(object sender, RoutedEventArgs e)
        {
            if (SelfEquipList.Visibility == Visibility.Visible) SelfEquipList.Visibility = Visibility.Collapsed;
            else SelfEquipList.Visibility = Visibility.Visible;
        }

        private int SafeParseInt(string input) { if (int.TryParse(input, out int r)) return r; return 0; }
        private long SafeParseLong(string input) { if (long.TryParse(input, out long r)) return r; return 0; }
        private string GetQualityText(int q) => q switch { 4 => "传说", 3 => "史诗", 2 => "稀有", 1 => "精良", _ => "普通" };
    }
}