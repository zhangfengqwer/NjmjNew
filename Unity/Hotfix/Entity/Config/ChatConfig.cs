using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class ChatConfig
    {
        public static ChatConfig s_instance = null;

        public List<ChatInfo> m_chatInfoList = new List<ChatInfo>();

        public static ChatConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ChatConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            m_chatInfoList.Clear();
            
            JsonData jd = JsonMapper.ToObject(jsonData);

            for (int i = 0; i < jd.Count; i++)
            {
                ChatInfo temp = new ChatInfo();

                temp.id = (int)jd[i]["chat_id"];
                temp.content = (string)jd[i]["content"];

                m_chatInfoList.Add(temp);
            }
        }

        public List<ChatInfo> getChatInfoList()
        {
            return m_chatInfoList;
        }

        public ChatInfo getChatInfoById(int id)
        {
            ChatInfo chatInfo = null;

            for (int i = 0; i < m_chatInfoList.Count; i++)
            {
                if (m_chatInfoList[i].id == id)
                {
                    chatInfo = m_chatInfoList[i];
                    break;
                }
            }

            return chatInfo;
        }
    }

    public class ChatInfo
    {
        public int id = 0;
        public string content = "";
    }
}
