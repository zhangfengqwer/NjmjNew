using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class TaskProgressInfo : EntityDB
    {
        public long UId { get; set; }
        public int TaskId { get; set; }
        public int CurProgress { get; set; }
    }
}
