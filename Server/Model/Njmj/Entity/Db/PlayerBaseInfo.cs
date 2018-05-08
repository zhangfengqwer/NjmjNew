
namespace ETModel
{
    public class PlayerBaseInfo : EntityDB
    {
        public long uid { get; set; }
        public string Name { get; set; }
        public int GoldNum { get; set; }
        public int WingNum { get; set; }
        public string Icon { get; set; }
    }
}
