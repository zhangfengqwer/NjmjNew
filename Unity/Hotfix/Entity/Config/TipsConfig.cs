using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hotfix
{
    public class TipsConfig
    {
        public static TipsConfig s_instance = null;

        public List<string> m_dataList = new List<string>();

        public static TipsConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new TipsConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_dataList.Clear();

            JsonData jd = JsonMapper.ToObject(jsonData);

            for (int i = 0; i < jd.Count; i++)
            {
                string content = (string)jd[i]["content"];
                m_dataList.Add(content);
            }
        }

        public List<string> getDataList()
        {
            return m_dataList;
        }
    }
}
