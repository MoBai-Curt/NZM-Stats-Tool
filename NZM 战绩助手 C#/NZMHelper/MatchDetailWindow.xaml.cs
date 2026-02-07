using Newtonsoft.Json.Linq;
using NZMHelper.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace NZMHelper
{
    public partial class MatchDetailWindow : Window
    {
        private readonly ApiService _api;

        public class SelfStatsViewModel
        {
            public ImageSource SelfAvatar { get; set; }
            public string SelfName { get; set; }
            public string SelfScore { get; set; }
            public string SelfKills { get; set; }
            public string SelfDeaths { get; set; }
            public string SelfBossDmg { get; set; }
            public string SelfMobsDmg { get; set; }
            public string SelfCoin { get; set; }
        }

        public MatchDetailWindow(string roomId, ApiService api)
        {
            InitializeComponent();
            _api = api;
            Load(roomId);
        }

        private async void Load(string rid)
        {
            var data = await _api.GetGameDetail(rid);
            TxtLoading.Visibility = Visibility.Collapsed;
            if (data == null) return;

            var allPlayers = data["list"]?.ToObject<List<JToken>>() ?? new List<JToken>();
            string myName = WebUtility.UrlDecode(data["loginUserDetail"]?["nickname"]?.ToString());

            var otherPlayers = new List<PlayerDetailViewModel>();
            List<WeaponMiniViewModel> myWeapons = null;

            foreach (var p in allPlayers)
            {
                string nick = WebUtility.UrlDecode(p["nickname"]?.ToString());
                bool isMe = nick == myName;

                var weapons = new List<WeaponMiniViewModel>();
                var equips = p["equipmentScheme"]?.ToObject<List<JToken>>();
                if (equips != null)
                {
                    foreach (var eq in equips)
                    {
                        var wvm = new WeaponMiniViewModel
                        {
                            Name = eq["weaponName"]?.ToString(),
                            Icon = await GlobalConfig.LoadImageAsync(eq["pic"]?.ToString()),
                            QualityColor = GlobalConfig.GetQualityBrush(eq["quality"]?.Value<int>() ?? 1),
                            Plugins = new List<ImageSource>()
                        };
                        var plugs = eq["commonItems"]?.ToObject<List<JToken>>();
                        if (plugs != null) foreach (var pl in plugs) { var img = await GlobalConfig.LoadImageAsync(pl["pic"]?.ToString()); if (img != null) wvm.Plugins.Add(img); }
                        weapons.Add(wvm);
                    }
                }

                if (isMe)
                {
                    myWeapons = weapons;
                    SelfStatsBar.DataContext = new SelfStatsViewModel
                    {
                        SelfName = nick,
                        SelfAvatar = await GlobalConfig.LoadImageAsync(WebUtility.UrlDecode(p["avatar"]?.ToString())),
                        SelfScore = long.Parse(p["baseDetail"]?["iScore"]?.ToString() ?? "0").ToString("N0"),
                        SelfKills = p["baseDetail"]?["iKills"]?.ToString() ?? "0",
                        SelfDeaths = p["baseDetail"]?["iDeaths"]?.ToString() ?? "0",
                        SelfBossDmg = long.Parse(p["huntingDetails"]?["DamageTotalOnBoss"]?.ToString() ?? "0").ToString("N0"),
                        SelfMobsDmg = long.Parse(p["huntingDetails"]?["DamageTotalOnMobs"]?.ToString() ?? "0").ToString("N0"),
                        SelfCoin = long.Parse(p["huntingDetails"]?["totalCoin"]?.ToString() ?? "0").ToString("N0")
                    };
                    SelfStatsBar.Visibility = Visibility.Visible;
                }
                else
                {
                    otherPlayers.Add(new PlayerDetailViewModel
                    {
                        Nickname = nick,
                        Avatar = await GlobalConfig.LoadImageAsync(WebUtility.UrlDecode(p["avatar"]?.ToString())),
                        ScoreText = "积分: " + long.Parse(p["baseDetail"]?["iScore"]?.ToString() ?? "0").ToString("N0"),
                        BossDmgText = "BOSS: " + long.Parse(p["huntingDetails"]?["DamageTotalOnBoss"]?.ToString() ?? "0").ToString("N0"),
                        Weapons = weapons
                    });
                }
            }

            PlayerList.ItemsSource = otherPlayers.OrderByDescending(p => long.Parse(p.ScoreText.Replace("积分: ", "").Replace(",", "")));
            SelfEquipList.ItemsSource = myWeapons;
        }

        private void BtnToggleSelfEquip_Click(object sender, RoutedEventArgs e)
        {
            SelfEquipList.Visibility = SelfEquipList.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class PlayerDetailViewModel
    {
        public string Nickname { get; set; }
        public ImageSource Avatar { get; set; }
        public string ScoreText { get; set; }
        public string BossDmgText { get; set; }
        public List<WeaponMiniViewModel> Weapons { get; set; }
    }

    public class WeaponMiniViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public Brush QualityColor { get; set; }
        public List<ImageSource> Plugins { get; set; }
    }
}