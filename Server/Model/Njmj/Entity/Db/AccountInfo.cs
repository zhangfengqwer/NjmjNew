using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 用户账号
    public class AccountInfo : EntityDB
	{
		public string Account { set; get; }
        public string Password { set; get; }
	}
}