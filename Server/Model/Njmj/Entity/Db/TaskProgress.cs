using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class TaskProgressInfo : EntityDB
    {
        public long UId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Reward { get; set; }
        public int TaskId { get; set; }
        public int CurProgress { get; set; }
        public int Target { get; set; }
        public bool IsComplete = false;
        public bool IsGet = false;
    }
}
