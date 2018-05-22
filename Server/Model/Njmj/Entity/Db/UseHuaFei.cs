using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 话费兑换
    public class UseHuaFei : EntityDB
	{
        public long Uid { set; get; }
        public int HuaFei { set; get; }
        public string Phone { set; get; }
    }
}