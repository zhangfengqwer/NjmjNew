using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 用户账号
    public class AccountInfo : EntityDB
	{
		public string Account { set; get; }
        public string Password { set; get; }
        public string Phone { set; get; }
        public string Token { set; get; }       // 手机号+年月日时分秒 1762561899720180509102703
    }
}