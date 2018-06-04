using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 老用户绑定日志
    public class Log_OldUserBind : EntityDB
	{
        public long Uid { set; get; }
        public string OldUid { set; get; }
        public string macId { set; get; }
    }
}