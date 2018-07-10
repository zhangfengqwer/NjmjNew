using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 用户财富变化日志
    public class Log_ChangeWealth : EntityDB
	{
        public long Uid { set; get; }
        public int PropId { get; set; }
        public int PropNum { get; set; }
        public string Reason { get; set; }
    }
}