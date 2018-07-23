using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class Log_Game : EntityDB
	{
        public string RoomName { set; get; }
        public long Player1_uid { set; get; }
        public long Player2_uid { set; get; }
        public long Player3_uid { set; get; }
        public long Player4_uid { set; get; }
        public long Winner_uid { set; get; }

        public int RoomNum { set; get; }

        // id;name;goldChange
        public string Player1_info { set; get; }
        public string Player2_info { set; get; }
        public string Player3_info { set; get; }
        public string Player4_info { set; get; }
    }
}