
namespace ETModel
{
    public class DuanwuDataBase : EntityDB
    {
        public long UId { get; set; }
        public int ZongziCount = 0;
        public string ActivityType = "";
        public int RefreshCount = 3;
    }
}
