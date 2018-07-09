using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 周排行榜
    public class WeekRank : EntityDB
	{
        public long UId { set; get; }
        public bool IsGetGameRank { set; get; }
        public bool IsGetGoldRank { set; get; }
        public int GameIndex { set; get; }
        public int GoldIndex { get; set; }
    }
}