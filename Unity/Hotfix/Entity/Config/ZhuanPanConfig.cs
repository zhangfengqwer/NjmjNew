using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class ZhuanPanConfig
    {
        public static ZhuanPanConfig s_instance = null;

        public List<ZhuanPanInfo> m_zhuanpanInfoList = new List<ZhuanPanInfo>();

        public static ZhuanPanConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ZhuanPanConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_zhuanpanInfoList.Clear();
            
            JsonData jd = JsonMapper.ToObject(jsonData);

            for (int i = 0; i < jd.Count; i++)
            {
                ZhuanPanInfo temp = new ZhuanPanInfo();

                temp.itemId = (int)jd[i]["itemId"];
                temp.prop_id = (int)jd[i]["prop_id"];

                temp.prop_num = (int)jd[i]["prop_num"];
                if (temp.prop_id == 3)
                {
                    temp.prop_num /= 100.0f;
                }

                temp.probability = (int)jd[i]["probability"];

                m_zhuanpanInfoList.Add(temp);
            }
        }

        public List<ZhuanPanInfo> getZhuanPanInfoList()
        {
            return m_zhuanpanInfoList;
        }

        public ZhuanPanInfo getZhuanPanInfoById(int id)
        {
            ZhuanPanInfo zhuanpanInfo = null;

            for (int i = 0; i < m_zhuanpanInfoList.Count; i++)
            {
                if (m_zhuanpanInfoList[i].prop_id == id)
                {
                    zhuanpanInfo = m_zhuanpanInfoList[i];
                    break;
                }
            }

            return zhuanpanInfo;
        }
    }

    public class ZhuanPanInfo
    {
        public int itemId;
        public int prop_id;
        public float prop_num;
        public int probability;
    }
}
