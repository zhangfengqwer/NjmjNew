using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 每日签到
    public class DailySign : EntityDB
	{
        public long Uid { set; get; }
        public string Reward { set; get; }
    }
}