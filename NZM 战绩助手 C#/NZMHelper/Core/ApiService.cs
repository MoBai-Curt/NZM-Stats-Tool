using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NZMHelper.Core
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;
        private string _cookie;
        private string _qrsig;

        public string AuthType { get; private set; } = "qq";

        public ApiService()
        {
            _handler = new HttpClientHandler { UseCookies = true, CookieContainer = new CookieContainer() };
            _client = new HttpClient(_handler);
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");
        }

        public async Task<byte[]> GetQrCodeAsync()
        {
            string url = $"https://ssl.ptlogin2.qq.com/ptqrshow?appid=549000912&e=2&l=M&s=3&d=72&v=4&t=0.5{DateTimeOffset.Now.ToUnixTimeMilliseconds()}&daid=5&pt_3rd_aid=0";
            var response = await _client.GetAsync(url);
            foreach (Cookie c in _handler.CookieContainer.GetCookies(new Uri("https://ssl.ptlogin2.qq.com")))
            {
                if (c.Name == "qrsig") _qrsig = c.Value;
            }
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<int> CheckQrStatusAsync()
        {
            if (string.IsNullOrEmpty(_qrsig)) return -1;
            int e = 0; foreach (char c in _qrsig) e += (e << 5) + c; int token = 2147483647 & e;
            string url = $"https://ssl.ptlogin2.qq.com/ptqrlogin?u1=https%3A%2F%2Fqzone.qq.com%2F&ptqrtoken={token}&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=0-0-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}&js_ver=21020514&js_type=1&login_sig=&pt_uistyle=40&aid=549000912&daid=5&";
            var res = await _client.GetAsync(url);
            string text = await res.Content.ReadAsStringAsync();
            if (text.Contains("ptuiCB('0'"))
            {
                var cookies = _handler.CookieContainer.GetCookies(new Uri("https://qzone.qq.com"));
                string uin = "", skey = "";
                foreach (Cookie c in cookies) { if (c.Name == "uin") uin = c.Value; if (c.Name == "skey") skey = c.Value; }
                _cookie = $"uin={uin}; skey={skey};";
                AuthType = "qq";
                SaveSession(_cookie, "qq");
                return 0;
            }
            if (text.Contains("ptuiCB('65'")) return 65;
            return 66;
        }

        public void SetManualCookie(string cookie, string type)
        {
            _cookie = cookie;
            AuthType = type;
            SaveSession(cookie, type);
        }

        private void SaveSession(string c, string t)
        {
            File.WriteAllText("nzm_session.json", JsonConvert.SerializeObject(new { c, t = DateTimeOffset.Now.ToUnixTimeSeconds(), type = t }));
        }

        public bool LoadLocalSession()
        {
            if (!File.Exists("nzm_session.json")) return false;
            try
            {
                var json = JObject.Parse(File.ReadAllText("nzm_session.json"));
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - (json["t"]?.Value<long>() ?? 0) < 86400)
                {
                    _cookie = json["c"]?.ToString();
                    AuthType = json["type"]?.ToString() ?? "qq";
                    return true;
                }
            }
            catch { }
            return false;
        }

        public void Logout() { _cookie = null; if (File.Exists("nzm_session.json")) File.Delete("nzm_session.json"); }

        public async Task<JToken> PostApiAsync(string method, object param)
        {
            if (string.IsNullOrEmpty(_cookie)) return null;
            try
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "iChartId", "430662" }, { "iSubChartId", "430662" }, { "sIdeToken", "NoOapI" },
                    { "method", method }, { "from_source", "2" }, { "param", JsonConvert.SerializeObject(param) }
                });
                var req = new HttpRequestMessage(HttpMethod.Post, "https://comm.ams.game.qq.com/ide/") { Content = content };
                req.Headers.Add("Cookie", _cookie);
                req.Headers.Referrer = new Uri("https://servicewechat.com/wx4e8cbe4fb0eca54c/9/page-frame.html");
                var res = await _client.SendAsync(req);
                var json = JObject.Parse(await res.Content.ReadAsStringAsync());
                if (json["ret"]?.Value<int>() == 0) return json["jData"]?["data"]?["data"];
            }
            catch { }
            return null;
        }

        public async Task<JToken> GetUserSummary() => await PostApiAsync("center.user.stats", new { seasonID = GlobalConfig.SEASON_ID });

        public async Task<List<JToken>> GetHistoryAsync()
        {
            var tasks = new List<Task<JToken>>();
            for (int i = 1; i <= 10; i++)
            {
                tasks.Add(PostApiAsync("center.user.game.list", new { seasonID = GlobalConfig.SEASON_ID, page = i, limit = 10 }));
            }

            var results = await Task.WhenAll(tasks);
            var allGames = new List<JToken>();

            foreach (var res in results)
            {
                if (res?["gameList"] != null)
                {
                    var pageGames = res["gameList"].ToObject<List<JToken>>();
                    if (pageGames != null && pageGames.Count > 0)
                        allGames.AddRange(pageGames);
                }
            }
            return allGames.OrderByDescending(g => g["dtGameStartTime"]?.ToString()).ToList();
        }

        public async Task<List<JToken>> GetHomeCollection()
        {
            var res = await PostApiAsync("collection.home", new { seasonID = GlobalConfig.SEASON_ID, limit = 6 });
            return res?["weaponList"]?.ToObject<List<JToken>>() ?? new List<JToken>();
        }

        public async Task<JToken> GetGameDetail(string roomId)
            => await PostApiAsync("center.game.detail", new { seasonID = GlobalConfig.SEASON_ID, roomID = roomId });

        public async Task<List<JToken>> GetCollection(string type)
        {
            string method = type switch { "weapon" => "collection.weapon.list", "trap" => "collection.trap.list", _ => "collection.plugin.list" };
            var param = type == "weapon" ? (object)new { seasonID = GlobalConfig.SEASON_ID, queryTime = true } : new { seasonID = GlobalConfig.SEASON_ID };
            var res = await PostApiAsync(method, param);
            return res?["list"]?.ToObject<List<JToken>>() ?? new List<JToken>();
        }
    }
}