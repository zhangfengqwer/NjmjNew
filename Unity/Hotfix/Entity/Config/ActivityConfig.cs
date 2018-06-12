using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class ActivityConfig
    {
        public static ActivityConfig s_instance = null;

        public List<ActivityInfo> m_activityInfoList = new List<ActivityInfo>();

        public static ActivityConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ActivityConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_activityInfoList.Clear();

            JsonData jd = JsonMapper.ToObject(jsonData);
            
            for (int i = 0; i < jd.Count; i++)
            {
                ActivityInfo temp = new ActivityInfo();

                temp.id = (int)jd[i]["id"];
                temp.title = (string)jd[i]["title"];

                m_activityInfoList.Add(temp);
            }
        }

        public List<ActivityInfo> getActivityInfoList()
        {
            return m_activityInfoList;
        }

        public ActivityInfo getPropInfoById(int id)
        {
            ActivityInfo activityInfo = null;

            for (int i = 0; i < m_activityInfoList.Count; i++)
            {
                if (m_activityInfoList[i].id == id)
                {
                    activityInfo = m_activityInfoList[i];
                    break;
                }
            }

            return activityInfo;
        }
    }

    public class ActivityInfo
    {
        public int id = 0;
        public string title = "";
    }
}
