using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 分享日志
    public class Log_Share : EntityDB
	{
        public long Uid { set; get; }
    }
}