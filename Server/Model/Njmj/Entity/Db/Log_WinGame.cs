using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class Log_Rank : EntityDB
	{
        public long UId { set; get; }
        public int WinGameCount { set; get; }
        public long Wealth { set; get; }
    }
}