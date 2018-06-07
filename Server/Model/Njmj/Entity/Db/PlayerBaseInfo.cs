﻿
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public class PlayerBaseInfo : EntityDB
    {
        public string Name { get; set; }
        public long GoldNum = 30000;
        public long WingNum = 0;
        public int HuaFeiNum = 0;
        public string Icon { get; set; }
        public bool IsRealName = false;
        public int RestChangeNameCount = 1;
        public int TotalGameCount = 0;
        public int WinGameCount = 0;
        public int PlayerSound = RandomHelper.RandomNumber(1, 5);
        public string VipTime = "2018-05-18 00:00:00";
        public string EmogiTime = "2018-05-18 00:00:00";
        public int ZhuanPanCount = 0;
        public int LuckyValue = 0;
        public int MaxHua = 0;


    }
}
