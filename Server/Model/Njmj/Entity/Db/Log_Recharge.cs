using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 用户充值日志
    public class Log_Recharge : EntityDB
	{
        public long Uid { set; get; }
        public long GoodsId { get; set; }
        public int Price { get; set; }
        public int OrderId { get; set; }
    }
}