using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class PlayerBaseInfo : EntityDB
    {
        public string Name { get; set; }
        public long GoldNum { get; set; }
        public long WingNum { get; set; }
        public int HuaFeiNum { get; set; }
        public string Icon { get; set; }
        public bool IsRealName { get; set; }
        public int RestChangeNameCount { get; set; }
        public int TotalGameCount { get; set; }
        public int WinGameCount { get; set; }
        public int PlayerSound { get; set; }
        public string VipTime { get; set; }
        public string EmogiTime { get; set; }
        public int ZhuanPanCount { get; set; }
        public int LuckyValue { get; set; }
        public int MaxHua { get; set; }
        public bool IsGiveFriendKey { get; set; }
        public int FriendKeyCount { get; set; }
        public long Score { get; set; }
        public PlayerBaseInfo() : base()
        {
            GoldNum = 30000;
            RestChangeNameCount = 1;
            IsRealName = false;
            PlayerSound = RandomHelper.RandomNumber(1, 5);
            VipTime = "2018-05-18 00:00:00";
            EmogiTime = "2018-05-18 00:00:00";
        }
    }
}