using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 转盘日志
    public class Log_UseZhuanPan : EntityDB
	{
        public long Uid { set; get; }
        public string Reward { set; get; }
    }
}