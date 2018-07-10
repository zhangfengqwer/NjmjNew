
namespace ETModel
{
    public class DuanwuDataBase : EntityDB
    {
        public long UId { get; set; }
        public int ZongziCount { get; set; }
        public string ActivityType { get; set; }
        public int RefreshCount { get; set; }
        public int CompleteCount { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public DuanwuDataBase() : base()
        {
            ActivityType = "";
            RefreshCount = 3;
            CompleteCount = 0;
            StartTime = "2018-06-15 00:00:00";
            EndTime = "2018-06-20 00:00:00";
        }
    }
}
