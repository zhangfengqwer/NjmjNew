
namespace ETModel
{
    public class PlayerBaseInfo : EntityDB
    {
        public long uid { get; set; }
        public string Name { get; set; }
        public long GoldNum { get; set; }
        public long WingNum { get; set; }
        public string Icon { get; set; }
    }
}
