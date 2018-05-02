using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    public partial class EntityDB : ComponentWithId
	{
	    public string CreateTime { set; get; }

	    public EntityDB()
	    {
	        CreateTime = DateTime.Now.GetCurrentTime();
        }
	}
}