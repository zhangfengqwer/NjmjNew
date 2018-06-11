using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public class NetConfig
    {
        public int clickCount = 0;

        public bool isFormal = true;
        public static NetConfig s_instance = null;

        string formal_url = "";
        int formal_port = 0;

        string test_url = "";
        int test_port = 0;

        string formal_web = "";
        string test_web = "";

        public static NetConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new NetConfig();
            }

            return s_instance;
        }

        public async Task Req(string url)
        {
            using (UnityWebRequestAsync webRequestAsync = ETModel.ComponentFactory.Create<UnityWebRequestAsync>())
            {
                await webRequestAsync.DownloadAsync(url);
                init(webRequestAsync.Request.downloadHandler.text);
            }
        }

        public void init(string jsonData)
        {
            try
            {
                JsonData jd = JsonMapper.ToObject(jsonData);

                formal_url = jd["formal"]["url"].ToString();
                formal_port = (int) jd["formal"]["port"];
                formal_web = jd["formal"]["weburl"].ToString();

                test_url = jd["test"]["url"].ToString();
                test_port = (int) jd["test"]["port"];
                test_web = jd["test"]["weburl"].ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public string getServerUrl()
        {
            if (isFormal)
            {
                return formal_url;
            }
            else
            {
                return test_url;
            }
        }

        public int getServerPort()
        {
            if (isFormal)
            {
                return formal_port;
            }
            else
            {
                return test_port;
            }
        }

        public string getWebUrl()
        {
            if (isFormal)
            {
                return formal_web;
            }
            else
            {
                return test_web;
            }
        }

        public IPEndPoint ToIPEndPointWithYuMing()
        {
            string serverUrl = NetConfig.getInstance().getServerUrl();
            int serverPort = NetConfig.getInstance().getServerPort();

            Log.Debug("serverUrl:" + serverUrl);
            Log.Debug("serverPort:" + serverPort);
            IPAddress ip;
            IPHostEntry IPinfo = Dns.GetHostEntry(serverUrl);
            if (IPinfo.AddressList.Length <= 0)
            {
                ToastScript.createToast("域名解析出错");
                return null;
            }
            ip = IPinfo.AddressList[0];
            return ToIPEndPoint(ip, serverPort);
        }

        public IPEndPoint ToIPEndPoint(IPAddress host, int port)
        {
            return new IPEndPoint(host, port);
        }

    }
}