using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 黑名单
    public class BlackList : EntityDB
	{
        public long Uid { set; get; }
        public string Reason { set; get; }
        public string EndTime { set; get; }
    }
}