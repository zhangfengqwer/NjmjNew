using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    public class ClientInfo
    {
        public Session m_session;
        public long m_uid;

        public ClientInfo(Session session, long uid)
        {
            m_session = session;
            m_uid = uid;
        }
    }

    public class ClientManager
    {
        static List<ClientInfo> clientInfoList = new List<ClientInfo>();

        public static void addClientInfo(Session session, long uid)
        {
            for (int i = clientInfoList.Count - 1; i >= 0; i--)
            {
                if (clientInfoList[i].m_uid== uid)
                {
                    // 强制踢下线
                    Actor_ForceOffline actor_ForceOffline = new Actor_ForceOffline();
                    clientInfoList[i].m_session.Send(actor_ForceOffline);

                    clientInfoList.RemoveAt(i);
                }
            }

            ClientInfo clientInfo = new ClientInfo(session,uid);
            clientInfoList.Add(clientInfo);
        }

        public static void deleteClientInfoByUid(long uid)
        {
            for (int i = clientInfoList.Count - 1; i >= 0 ; i--)
            {
                if (clientInfoList[i].m_uid == uid)
                {
                    clientInfoList.RemoveAt(i);
                }
            }
        }
    }
}
