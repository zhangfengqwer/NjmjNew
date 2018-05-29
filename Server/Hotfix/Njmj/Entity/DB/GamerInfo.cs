using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{ 
    public class GamerInfoDB : EntityDB
    {
        //用户id(唯一)
        public long UId { set; get; }
        //当天领取的宝箱次数
        public int DailyTreasureCount { set; get; }
        //当天在线时长
        public int DailyOnlineTime { set; get; }
        //总共在线时长
        public int TotalOnlineTime { set; get; }
    }
}
