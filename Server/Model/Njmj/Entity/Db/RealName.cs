using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 实名认证
    public class RealName : EntityDB
	{
        public long Uid { set; get; }
        public string Name { set; get; }
        public string IDNumber { set; get; }
    }
}