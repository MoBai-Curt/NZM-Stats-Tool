using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace NZMHelper
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("ret")]
        public int Ret { get; set; }
        [JsonPropertyName("jData")]
        public JData<T> JData { get; set; }
    }

    public class JData<T>
    {
        [JsonPropertyName("data")]
        public InnerData<T> Data { get; set; }
    }

    public class InnerData<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class UserSummary
    {
        [JsonPropertyName("huntGameCount")]
        public string HuntGameCount { get; set; }
        [JsonPropertyName("playtime")]
        public string Playtime { get; set; }
    }

    public class GameListResponse
    {
        [JsonPropertyName("gameList")]
        public List<GameRecord> GameList { get; set; }
    }

    public class GameRecord
    {
        [JsonPropertyName("iMapId")]
        public string MapId { get; set; }
        [JsonPropertyName("iSubModeType")]
        public string SubModeType { get; set; }
        [JsonPropertyName("iIsWin")]
        public string IsWin { get; set; }
        [JsonPropertyName("iScore")]
        public string Score { get; set; }
        [JsonPropertyName("iDuration")]
        public string Duration { get; set; }
        [JsonPropertyName("dtGameStartTime")]
        public string StartTime { get; set; }
        [JsonPropertyName("equipmentScheme")]
        public List<WeaponItem> EquipmentScheme { get; set; }

        public string MapNameDisplay { get; set; }
        public string ModeDisplay { get; set; }
        public string ResultText => IsWin == "1" ? "胜利" : "失败";
        public string ResultColor => IsWin == "1" ? "#ef4444" : "#94a3b8";

        public string ScoreFormatted
        {
            get
            {
                if (long.TryParse(Score, out var s)) return s.ToString("N0");
                return "0";
            }
        }

        public string DurationFormatted
        {
            get
            {
                if (int.TryParse(Duration, out var d)) return $"{d / 60}分{d % 60}秒";
                return "0分0秒";
            }
        }

        public string MapImage { get; set; }
        public bool IsExpanded { get; set; } = false;
    }

    public class WeaponItem
    {
        [JsonPropertyName("weaponName")]
        public string WeaponName { get; set; }
        [JsonPropertyName("pic")]
        public string Pic { get; set; }
        [JsonPropertyName("quality")]
        public int Quality { get; set; }
        [JsonPropertyName("commonItems")]
        public List<PluginItem> Plugins { get; set; }

        public string BorderColor => Quality switch
        {
            4 => "#d4a84b",
            3 => "#a855f7",
            2 => "#3b82f6",
            _ => "#10b981"
        };
    }

    public class PluginItem
    {
        [JsonPropertyName("pic")]
        public string Pic { get; set; }
    }
}