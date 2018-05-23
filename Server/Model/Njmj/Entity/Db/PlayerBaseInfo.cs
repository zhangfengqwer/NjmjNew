
namespace ETModel
{
    public class PlayerBaseInfo : EntityDB
    {
        public string Name { get; set; }
        public long GoldNum { get; set; }
        public long WingNum { get; set; }
        public float HuaFeiNum = 0;
        public string Icon { get; set; }
        public bool IsRealName = false;
        public int RestChangeNameCount = 1;
        public int TotalGameCount { get; set; }
        public int WinGameCount { get; set; }
        public string Phone { get; set; }
        public int PlayerSound { get; set; }
        public string VipTime = "2018-05-18 00:00:00";
        public string EmogiTime = "2018-05-18 00:00:00";
        public int ZhuanPanCount = 0;
        public int LuckyValue = 0;
    }
}
