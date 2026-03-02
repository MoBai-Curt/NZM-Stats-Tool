using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace NZMHelper.Core
{
    public class CookieSniffer
    {
        private ProxyServer _proxyServer;
        private ExplicitProxyEndPoint _endPoint;

        public bool IsRunning { get; private set; }

        public event Action<string> OnCookieCaptured;
        public event Action<string> OnLog;

        public void Start()
        {
            if (IsRunning) return;

            try
            {
                _proxyServer = new ProxyServer(false);
                _proxyServer.CertificateManager.CertificateEngine = Titanium.Web.Proxy.Network.CertificateEngine.DefaultWindows;
                _proxyServer.CertificateManager.EnsureRootCertificate();
                _proxyServer.CertificateManager.TrustRootCertificate(true);

                _proxyServer.BeforeRequest += OnRequest;

                _endPoint = new ExplicitProxyEndPoint(IPAddress.Any, 9527, true);
                _proxyServer.AddEndPoint(_endPoint);
                _proxyServer.Start();

                _proxyServer.SetAsSystemHttpProxy(_endPoint);
                _proxyServer.SetAsSystemHttpsProxy(_endPoint);

                IsRunning = true;
                OnLog?.Invoke("代理已开启，请在小程序打开战绩页...");
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"代理启动失败: {ex.Message}");
                Stop();
            }
        }

        private Task OnRequest(object sender, SessionEventArgs e)
        {
            var host = e.HttpClient.Request.RequestUri.Host;

            if (host.Contains("ams.game.qq.com") || host.Contains("comm.ams.game.qq.com"))
            {
                var cookieHeader = e.HttpClient.Request.Headers.GetHeaders("Cookie").FirstOrDefault();
                if (cookieHeader != null)
                {
                    string cookieVal = cookieHeader.Value;

                    if (cookieVal.Contains("openid=") && (cookieVal.Contains("access_token=") || cookieVal.Contains("ieg_ams_token=")))
                    {
                        string cleanCookie = ExtractTargetCookies(cookieVal);

                        if (IsRunning)
                        {
                            OnCookieCaptured?.Invoke(cleanCookie);
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        private string ExtractTargetCookies(string rawCookie)
        {
            var targetKeys = new[] { "openid", "appid", "acctype", "access_token", "ieg_ams_token", "ieg_ams_token_time", "ieg_ams_token_v2", "ieg_ams_session_token", "unionid" };
            var parts = rawCookie.Split(';');
            var dict = new System.Collections.Generic.Dictionary<string, string>();

            foreach (var part in parts)
            {
                var pair = part.Trim().Split(new[] { '=' }, 2);
                if (pair.Length == 2)
                {
                    string key = pair[0].Trim().ToLower();
                    if (targetKeys.Contains(key))
                    {
                        dict[key] = pair[1].Trim();
                    }
                }
            }

            return string.Join("; ", dict.Select(x => $"{x.Key}={x.Value}"));
        }

        public void Stop()
        {
            if (_proxyServer != null)
            {
                try
                {
                    _proxyServer.BeforeRequest -= OnRequest;
                    _proxyServer.Stop();
                    _proxyServer.Dispose();
                }
                catch { }
                finally
                {
                    _proxyServer = null;
                    IsRunning = false;
                    OnLog?.Invoke("网络代理已关闭，系统恢复正常。");
                }
            }
        }
    }
}