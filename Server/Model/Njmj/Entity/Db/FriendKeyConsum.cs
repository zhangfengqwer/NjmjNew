using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 好友房钥匙
    public class FriendKeyConsum : EntityDB
	{
        public long UId { set; get; }
        public int ConsumCount { set; get; }
        public int GetCount { set; get; }
    }
}