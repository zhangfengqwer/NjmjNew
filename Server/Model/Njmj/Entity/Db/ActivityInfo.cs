using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class ActivityInfo :EntityDB
    {
        public int ActivityID { get; set; }
        public string Title { get; set; }
    }
}
