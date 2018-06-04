using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    class ApkVersionConfig
    {
        public static ApkVersionConfig s_instance = null;

        public List<VersionInfo> m_dataList = new List<VersionInfo>();

        public static ApkVersionConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ApkVersionConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_dataList.Clear();
            
            JsonData jd = JsonMapper.ToObject(jsonData);

            for (int i = 0; i < jd.Count; i++)
            {
                VersionInfo temp = new VersionInfo();
                
                temp.channel_name = (string)jd[i]["channel_name"];
                temp.version = (string)jd[i]["version"];

                m_dataList.Add(temp);
            }
        }

        public List<VersionInfo> getPropInfoList()
        {
            return m_dataList;
        }

        public VersionInfo getDataById(string channel_name)
        {
            VersionInfo versionInfo = null;

            for (int i = 0; i < m_dataList.Count; i++)
            {
                if (m_dataList[i].channel_name == channel_name)
                {
                    versionInfo = m_dataList[i];
                    break;
                }
            }

            return versionInfo;
        }
    }

    public class VersionInfo
    {
        public string channel_name = "";
        public string version = "";
    }
}
