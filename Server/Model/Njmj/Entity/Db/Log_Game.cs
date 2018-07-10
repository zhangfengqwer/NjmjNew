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
    }
}