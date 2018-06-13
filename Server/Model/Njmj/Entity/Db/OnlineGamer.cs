using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    // 用户账号
    public class OnlineGamer : EntityDB
	{
	    public long RoomId { get; set; }
        
    }
}