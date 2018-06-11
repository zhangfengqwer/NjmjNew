using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class NetConfig
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
        
        public void init(string jsonData)
        {
            try
            {
                JsonData jd = JsonMapper.ToObject(jsonData);

                formal_url = jd["formal"]["url"].ToString();
                formal_port = (int)jd["formal"]["port"];
                formal_web = jd["formal"]["weburl"].ToString();

                test_url = jd["test"]["url"].ToString();
                test_port = (int)jd["test"]["port"];
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
    }
}
