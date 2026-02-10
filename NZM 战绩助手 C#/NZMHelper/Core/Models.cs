using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Input;

namespace NZMHelper.Core
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> e) { _execute = e; }
        public bool CanExecute(object p) => true;
        public void Execute(object p) => _execute(p);
        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class GameViewModel
    {
        public string RoomId { get; set; }
        public ImageSource MapIcon { get; set; }
        public string Result { get; set; }
        public Brush ResultColor { get; set; }
        public string Mode { get; set; }
        public string DetailText { get; set; }
        public string Score { get; set; }
        public string Duration { get; set; }
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

    public class ModeStatViewModel { public string Name { get; set; } public string TotalText { get; set; } public string DetailText { get; set; } }

    public class MapStatViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public string Mode { get; set; }
        public string TotalWinRate { get; set; }
        public List<DiffStat> Diffs { get; set; }
    }
    public class DiffStat { public string DisplayText { get; set; } }

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
        public string BossDmgText { get; set; }
        public string MobsDmgText { get; set; }
        public string CoinText { get; set; }

        public string KDAText { get; set; }

        public List<WeaponMiniViewModel> Weapons { get; set; }
    }

    public class WeaponMiniViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public Brush QualityColor { get; set; }
        public string QualityText { get; set; }
        public List<PluginViewModel> Plugins { get; set; }
        public bool HasPlugins => Plugins != null && Plugins.Count > 0;
    }

    public class PluginViewModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public Brush QualityColor { get; set; }
        public Task<ImageSource> IconSourceTask { get; set; }
    }
}