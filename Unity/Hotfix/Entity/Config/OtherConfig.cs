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

        public string shieldShare = "";

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
                shieldShare = jd["shieldShare"].ToString();
                List<string> list = new List<string>();
                CommonUtil.splitStr(shieldShare,list,';');
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].CompareTo(PlatformHelper.GetChannelName()) == 0)
                    {
                        OtherData.isShiedShare = true;
                        break;
                    }
                }
            }
        }
    }
}