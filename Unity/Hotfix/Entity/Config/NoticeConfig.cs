using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class NoticeConfig
    {
        public static NoticeConfig s_instance = null;

        public List<NoticeInfo> m_dataList = new List<NoticeInfo>();

        public static NoticeConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new NoticeConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_dataList.Clear();
            
            JsonData jd = JsonMapper.ToObject(jsonData);

            for (int i = 0; i < jd.Count; i++)
            {
                NoticeInfo temp = new NoticeInfo();

                temp.id = (int)jd[i]["id"];
                temp.title = (string)jd[i]["title"];
                temp.content = (string)jd[i]["content"];

                m_dataList.Add(temp);
            }
        }

        public List<NoticeInfo> getActivityInfoList()
        {
            return m_dataList;
        }

        public NoticeInfo getPropInfoById(int id)
        {
            NoticeInfo data = null;

            for (int i = 0; i < m_dataList.Count; i++)
            {
                if (m_dataList[i].id == id)
                {
                    data = m_dataList[i];
                    break;
                }
            }

            return data;
        }
    }

    public class NoticeInfo
    {
        public int id = 0;
        public string title = "";
        public string content = "";
    }
}
