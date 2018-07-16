using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 救济金日志
    public class Log_ReliefGold : EntityDB
	{
        public long Uid { set; get; }
        public string reward { set; get; }
    }
}