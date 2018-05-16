using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class PropConfig
    {
        public static PropConfig s_instance = null;

        public List<PropInfo> m_propInfoList = new List<PropInfo>();

        public static PropConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new PropConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_propInfoList.Clear();
            
            JsonData jd = JsonMapper.ToObject(jsonData);

            for (int i = 0; i < jd.Count; i++)
            {
                PropInfo temp = new PropInfo();

                temp.prop_id = (int)jd[i]["prop_id"];
                temp.prop_name = (string)jd[i]["prop_name"];
                temp.desc = (string)jd[i]["desc"];

                m_propInfoList.Add(temp);
            }

            //for (int i = 0; i < m_propInfoList.Count; i++)
            //{
            //    Log.Debug(m_propInfoList[i].desc + "、");
            //}
        }

        public List<PropInfo> getPropInfoList()
        {
            return m_propInfoList;
        }

        public PropInfo getPropInfoById(int id)
        {
            PropInfo propInfo = null;

            for (int i = 0; i < m_propInfoList.Count; i++)
            {
                if (m_propInfoList[i].prop_id == id)
                {
                    propInfo = m_propInfoList[i];
                    break;
                }
            }

            return propInfo;
        }
    }

    public class PropInfo
    {
        public int prop_id = 0;
        public string prop_name = "";
        public string desc = "";
    }
}
