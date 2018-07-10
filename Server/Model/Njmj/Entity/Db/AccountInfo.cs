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
        public string Third_Id { set; get; }
        public string MachineId { set; get; }
        public string ChannelName { set; get; }
        public string ClientVersion { set; get; }
        public int OldAccountState { set; get; }       // 1:未拉取   2:是老用户   3:不是老用户

        public AccountInfo() : base()
        {
            OldAccountState = 1;
        }
    }
}