using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 登录日志
    public class Log_Login : EntityDB
	{
        public long Uid { set; get; }
    }
}