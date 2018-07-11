using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 好友房钥匙
    public class FriendKey : EntityDB
	{
        public long Uid { set; get; }
        public string endTime { set; get; }
    }
}