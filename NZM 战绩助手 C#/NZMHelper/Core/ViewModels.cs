using System.Collections.Generic;
using System.Windows.Media;

namespace NZMHelper
{
    public class GameViewModel
    {
        public string RoomId { get; set; }
        public string Result { get; set; }
        public Brush ResultColor { get; set; }
        public string Score { get; set; }
        public string Duration { get; set; }
        public string Mode { get; set; }
        public ImageSource MapIcon { get; set; }
        public string DetailText { get; set; }
    }

    public class CollectionViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public int Quality { get; set; }
        public bool Owned { get; set; }
        public Brush BgColor { get; set; }
        public Brush BorderColor { get; set; }
        public Brush TextColor { get; set; }
        public string QualityText { get; set; }
    }

    public class ModeStatViewModel
    {
        public string Name { get; set; }
        public string TotalText { get; set; }
        public string DetailText { get; set; }
    }

    public class MapStatViewModel
    {
        public string Name { get; set; }
        public string Mode { get; set; }
        public ImageSource Icon { get; set; }
        public string TotalWinRate { get; set; }
        public List<DiffStat> Diffs { get; set; }
    }

    public class DiffStat
    {
        public string DisplayText { get; set; }
    }

    public class FragmentItemViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public Brush QualityColor { get; set; }
        public double ProgressPercent { get; set; }
        public string ProgressText { get; set; }
    }

    public class PlayerDetailViewModel
    {
        public string Nickname { get; set; }
        public ImageSource Avatar { get; set; }
        public string ScoreText { get; set; }
        public string KDAText { get; set; }
        public string BossDmgText { get; set; }
        public string MobsDmgText { get; set; }
        public string CoinText { get; set; }
        public List<WeaponMiniViewModel> Weapons { get; set; }
    }

    public class WeaponMiniViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public Brush QualityColor { get; set; }
        public string QualityText { get; set; }
        public List<PluginViewModel> Plugins { get; set; }
    }

    public class PluginViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public Brush QualityColor { get; set; }
    }
}