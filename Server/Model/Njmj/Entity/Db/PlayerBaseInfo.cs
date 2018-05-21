
namespace ETModel
{
    public class PlayerBaseInfo : EntityDB
    {
        public string Name { get; set; }
        public long GoldNum { get; set; }
        public long WingNum { get; set; }
        public string Icon { get; set; }
        public bool IsRealName = false;
        public bool IsBindPhone = false;
        public int RestChangeNameCount = 1;
        public int TotalGameCount { get; set; }
        public int WinGameCount { get; set; }
    }
}
