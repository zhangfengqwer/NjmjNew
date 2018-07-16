using ETHotfix;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    class FriendRoomConfig
    {
        public static FriendRoomConfig s_instance = null;

        public List<int> beilvList = new List<int>();
        public List<int> typeList = new List<int>();
        public List<FriendRoomJuShu> juShuList = new List<FriendRoomJuShu>();
        
        public static FriendRoomConfig getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new FriendRoomConfig();
            }

            return s_instance;
        }
        
        public void init(string jsonData)
        {
            try
            {
                beilvList.Clear();
                typeList.Clear();
                juShuList.Clear();

                JsonData jd = JsonMapper.ToObject(jsonData);

                // 每花
                {
                    for (int i = 0; i < jd["BeiLv"].Count; i++)
                    {
                        beilvList.Add((int)jd["BeiLv"][i]["meihua"]);
                    }
                }

                // 局数
                {
                    for (int i = 0; i < jd["RoomJuShu"].Count; i++)
                    {
                        int jushu = (int)jd["RoomJuShu"][i]["jushu"];
                        int yaoshi = (int)jd["RoomJuShu"][i]["yaoshi"];

                        juShuList.Add(new FriendRoomJuShu(jushu, yaoshi));
                    }
                }

                // 房间类型：1公开   2私密
                {
                    for (int i = 0; i < jd["RoomType"].Count; i++)
                    {
                        typeList.Add((int)jd["RoomType"][i]["type"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }
    }

    public class FriendRoomJuShu
    {
        public int m_jushu;
        public int m_yaoshi;

        public FriendRoomJuShu(int jushu,int yaoshi)
        {
            m_jushu = jushu;
            m_yaoshi = yaoshi;
        }
    }
}