using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NZMHelper.Core
{
    public static class GlobalConfig
    {
        public const string CURRENT_VERSION = "V1.5.0";
        public const int SEASON_ID = 1;

        public const string URL_REMOTE_CONFIG = "http://mobaiya.icu/files/version.json";

        public const string STATIC_ANNOUNCEMENT =
            "欢迎使用 NZM 战绩助手\n\n" +
            "1. 本软件仅供学习交流。\n" +
            "2. 严禁用于商业用途。\n\n" +
            "使用说明：\n" +
            "- 登录后可查看战绩与装备\n" +
            "- 碎片悬浮窗可置顶\n" +
            "- 登录有效期为24小时";

        public const string URL_MY_BLOG = "http://mobaiya.icu/";
        public static string URL_UPDATE = "";
        public static string PWD_UPDATE = "";

        private static readonly HttpClient _httpClient = new HttpClient();

        public static Brush GetQualityBrush(int quality)
        {
            return quality switch { 4 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d4a84b")), 3 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a855f7")), 2 => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3b82f6")), _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#71717a")) };
        }

        public static class Colors { public static Brush CardBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27272a")); public static Brush Red = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ef4444")); }

        public static Dictionary<string, string> DiffMap = new() { { "0", "默认" }, { "1", "引导" }, { "2", "普通" }, { "3", "困难" }, { "4", "英雄" }, { "5", "炼狱" }, { "6", "折磨I" }, { "7", "折磨II" }, { "8", "折磨III" }, { "9", "折磨IV" }, { "10", "折磨V" }, { "32", "练习" } };

        public static Dictionary<string, (string Name, string Mode, string Icon)> MapConfig = new()
        {
            { "1000", ("风暴峡谷", "机甲战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1000.png") },
            { "1001", ("风暴峡谷", "机甲战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1001.png") },
            { "1002", ("凯旋之地", "机甲战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-1002.png") },
            { "112", ("黑暗复活节", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-112.png") },
            { "114", ("大都会", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-114.png") },
            { "115", ("冰点源起", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-115.png") },
            { "12", ("黑暗复活节", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-12.png") },
            { "132", ("飓风要塞", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-132.png") },
            { "135", ("苍穹之上", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-135.png") },
            { "14", ("大都会", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-14.png") },
            { "16", ("昆仑神宫", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-16.png") },
            { "17", ("精绝古城", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-17.png") },
            { "21", ("冰点源起", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-21.png") },
            { "30", ("猎场-新手关", "僵尸猎场", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-30.png") },
            { "300", ("空间站", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-300.png") },
            { "304", ("20号星港", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-304.png") },
            { "306", ("联盟大厦", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-306.png") },
            { "308", ("塔防-新手关", "塔防战", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-308.png") },
            { "321", ("根除变异", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-321.png") },
            { "322", ("夺回资料", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-322.png") },
            { "323", ("猎杀南十字", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-323.png") },
            { "324", ("追猎-新手关", "时空追猎", "https://nzm.playerhub.qq.com/playerhub/60106/maps/maps-324.png") }
        };

        public static string GetModeName(string mapId) { if (MapConfig.ContainsKey(mapId)) return MapConfig[mapId].Mode; if (int.TryParse(mapId, out int mid)) { if (mid >= 1000) return "机甲战"; if (mid >= 300 && mid < 400) return "塔防战"; if (mid >= 10 && mid < 200) return "僵尸猎场"; } return "未知模式"; }

        public static async Task<BitmapImage> LoadImageAsync(string url) { if (string.IsNullOrEmpty(url)) return null; try { url = System.Net.WebUtility.UrlDecode(url); string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"); if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir); string hash = BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", ""); string filePath = Path.Combine(cacheDir, hash + ".png"); if (File.Exists(filePath)) return LoadBitmapFromFile(filePath); var bytes = await _httpClient.GetByteArrayAsync(url); await File.WriteAllBytesAsync(filePath, bytes); return LoadBitmapFromFile(filePath); } catch { return null; } }
        private static BitmapImage LoadBitmapFromFile(string path) { try { var bitmap = new BitmapImage(); bitmap.BeginInit(); bitmap.CacheOption = BitmapCacheOption.OnLoad; bitmap.UriSource = new Uri(path); bitmap.EndInit(); bitmap.Freeze(); return bitmap; } catch { return null; } }
    }
}