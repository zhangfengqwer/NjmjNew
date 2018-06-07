
namespace ETModel
{
    public class DuanwuActivityInfo : EntityDB
    {
        public long UId { get; set; }
        public string Desc { get; set; }
        public int Reward { get; set; }
        public int TaskId { get; set; }
        public int CurProgress = 0;
        public int Target { get; set; }
        public bool IsComplete = false;
        public bool IsGet = false;
    }
}
