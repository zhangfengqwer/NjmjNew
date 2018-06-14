
namespace ETModel
{
    public class DuanwuDataBase : EntityDB
    {
        public long UId { get; set; }
        public int ZongziCount = 0;
        public string ActivityType = "";
        public int RefreshCount = 3;
        public int CompleteCount = 0;
        public string StartTime = "2018-06-15 00:00:00";
        public string EndTime = "2018-06-20 00:00:00";
    }
}
