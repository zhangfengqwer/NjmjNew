using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class OtherConfig
    {
        public static OtherConfig s_instance = null;

        public static OtherConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new OtherConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            JsonData jd = JsonMapper.ToObject(jsonData);

            // 分享开关
            {
                string data = jd["shieldShare"].ToString();
                List<string> list = new List<string>();
                CommonUtil.splitStr(data, list,';');
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].CompareTo(PlatformHelper.GetChannelName()) == 0)
                    {
                        OtherData.isShiedShare = true;
                        break;
                    }
                }
            }

            // 实名开关
            {
                string data = jd["shieldRealName"].ToString();
                List<string> list = new List<string>();
                CommonUtil.splitStr(data, list, ';');
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].CompareTo(PlatformHelper.GetChannelName()) == 0)
                    {
                        OtherData.isShiedRealName = true;
                        break;
                    }
                }
            }

            // 绑定手机开关
            {
                string data = jd["shieldBindPhone"].ToString();
                List<string> list = new List<string>();
                CommonUtil.splitStr(data, list, ';');
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].CompareTo(PlatformHelper.GetChannelName()) == 0)
                    {
                        OtherData.isShiedBindPhone = true;
                        break;
                    }
                }
            }

            // 手机登录开关
            {
                string data = jd["shieldPhoneLogin"].ToString();
                List<string> list = new List<string>();
                CommonUtil.splitStr(data, list, ';');
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].CompareTo(PlatformHelper.GetChannelName()) == 0)
                    {
                        OtherData.isShiedPhoneLogin = true;
                        break;
                    }
                }
            }
        }
    }
}