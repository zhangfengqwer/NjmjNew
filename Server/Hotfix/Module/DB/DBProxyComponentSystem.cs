using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ETHotfix
{
	[ObjectSystem]
	public class DbProxyComponentSystem : AwakeSystem<DBProxyComponent>
	{
		public override void Awake(DBProxyComponent self)
		{
			self.Awake();
		}
	}
	
	/// <summary>
	/// 用来与数据库操作代理
	/// </summary>
	public static class DBProxyComponentEx
	{
		public static void Awake(this DBProxyComponent self)
		{
			StartConfig dbStartConfig = Game.Scene.GetComponent<StartConfigComponent>().DBConfig;
			self.dbAddress = dbStartConfig.GetComponent<InnerConfig>().IPEndPoint;
		}

		public static async Task Save(this DBProxyComponent self, ComponentWithId component, bool needCache = false)
		{
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
			await session.Call(new DBSaveRequest { Component = component, NeedCache = needCache});
		}

		public static async Task SaveBatch(this DBProxyComponent self, List<ComponentWithId> components, bool needCache = false)
		{
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
			await session.Call(new DBSaveBatchRequest { Components = components, NeedCache = needCache});
		}

		public static async Task Save(this DBProxyComponent self, ComponentWithId component, bool needCache, CancellationToken cancellationToken)
		{
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
			await session.Call(new DBSaveRequest { Component = component, NeedCache = needCache}, cancellationToken);
		}

		public static async void SaveLog(this DBProxyComponent self, ComponentWithId component)
		{
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
			await session.Call(new DBSaveRequest { Component = component,  NeedCache = false, CollectionName = "Log" });
		}

		public static async Task<T> Query<T>(this DBProxyComponent self, long id, bool needCache = false) where T: ComponentWithId
		{
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
			DBQueryResponse dbQueryResponse = (DBQueryResponse)await session.Call(new DBQueryRequest { CollectionName = typeof(T).Name, Id = id, NeedCache = needCache });
			return (T)dbQueryResponse.Component;
		}

		public static async Task<List<T>> QueryBatch<T>(this DBProxyComponent self, List<long> ids, bool needCache = false) where T : ComponentWithId
		{
			List<T> list = new List<T>();
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
			DBQueryBatchResponse dbQueryBatchResponse = (DBQueryBatchResponse)await session.Call(new DBQueryBatchRequest { CollectionName = typeof(T).Name, IdList = ids, NeedCache = needCache});
			foreach (ComponentWithId component in dbQueryBatchResponse.Components)
			{
				list.Add((T)component);
			}
			return list;
		}

		public static async Task<List<T>> QueryJson<T>(this DBProxyComponent self, string json) where T : ComponentWithId
		{
			List<T> list = new List<T>();
			Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);

			DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });
			foreach (ComponentWithId component in dbQueryJsonResponse.Components)
			{
				list.Add((T)component);
			}
			return list;
		}

	    /// <summary>
	    /// 根据查询表达式查询
	    /// </summary>
	    /// <param name="self"></param>
	    /// <param name="exp"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public static async Task<List<ComponentWithId>> Query<T>(this DBProxyComponent self, Expression<Func<T, bool>> exp) where T : ComponentWithId
	    {
	        ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
	        IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
	        IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
	        string json = filter.Render(documentSerializer, serializerRegistry).ToJson();
	        return await self.Query<T>(json);
	    }

	    /// <summary>
	    /// 根据json查询条件查询
	    /// </summary>
	    /// <param name="self"></param>
	    /// <param name="json"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public static async Task<List<ComponentWithId>> Query<T>(this DBProxyComponent self, string json) where T : ComponentWithId
	    {
	        Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
	        DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });
	        return dbQueryJsonResponse.Components;
	    }


        public static async Task<List<T>> QueryJsonCurrentDay<T>(this DBProxyComponent self) where T : ComponentWithId
        {
	        List<T> list = new List<T>();
	        string json = $"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/}}";
	        Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
	        DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });
	        foreach (ComponentWithId disposer in dbQueryJsonResponse.Components)
	        {
	            list.Add((T)disposer);
	        }
	        return list;
	    }

        public static async Task<List<T>> QueryJsonByDay<T>(this DBProxyComponent self , string day) where T : ComponentWithId
        {
            List<T> list = new List<T>();
            string json = $"{{CreateTime:/^{day}/}}";
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });
            foreach (ComponentWithId disposer in dbQueryJsonResponse.Components)
            {
                list.Add((T)disposer);
            }
            return list;
        }

        /// <summary>
        /// 根据UId查询指定的日期的数据,查询数据库的字段是:UId,CreateTime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="userId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static async Task<List<T>> QueryJsonCurrentDayByUid<T>(this DBProxyComponent self,long userId,DateTime dateTime) where T : ComponentWithId
	    {
	        List<T> list = new List<T>();
	        string json = $"{{UId:{userId}, CreateTime:/^{dateTime.GetCurrentDay()}/}}";
	        Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
	        DBQueryJsonResponse dbQueryJsonResponse = (DBQueryJsonResponse)await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });
	        foreach (ComponentWithId disposer in dbQueryJsonResponse.Components)
	        {
	            list.Add((T)disposer);
	        }
	        return list;
	    }

        //刷新周排行榜 1,周财富榜，2，周战绩榜
        public static async Task<List<Log_Rank>> QueryJsonRank(this DBProxyComponent self, int type)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            FilterDefinition<Log_Rank> filterDefinition = new JsonFilterDefinition<Log_Rank>($"{{}}");
            List<Log_Rank> components = new List<Log_Rank>();
            if (type == 1)
            {
                components = await dbComponent.GetDBDataCollection<Log_Rank>(typeof(Log_Rank).Name).Find(filterDefinition).SortByDescending(a => a.Wealth).Limit(50).ToListAsync();
            }
            else if (type == 2)
            {
                components = await dbComponent.GetDBDataCollection<Log_Rank>(typeof(Log_Rank).Name).Find(filterDefinition).SortByDescending(a => a.WinGameCount).Limit(50).ToListAsync();
            }
            return components;
        }

	    public static async Task<List<PlayerBaseInfo>> QueryJsonGamePlayer(this DBProxyComponent self)
        {
	        DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
	        FilterDefinition<PlayerBaseInfo> filterDefinition = new JsonFilterDefinition<PlayerBaseInfo>($"{{}}");
	        List<PlayerBaseInfo> components = await dbComponent.GetDBDataCollection<PlayerBaseInfo>(typeof(PlayerBaseInfo).Name).Find(filterDefinition).SortByDescending(a => a.WinGameCount).Limit(50).ToListAsync();
	        return components;
        }

	    public static async Task<List<PlayerBaseInfo>> QueryJsonPlayerInfo(this DBProxyComponent self)
        {
	        DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
	        FilterDefinition<PlayerBaseInfo> filterDefinition = new JsonFilterDefinition<PlayerBaseInfo>($"{{}}");
	        List<PlayerBaseInfo> components = await dbComponent.GetDBDataCollection<PlayerBaseInfo>(typeof(PlayerBaseInfo).Name).Find(filterDefinition).SortByDescending(a => a.GoldNum).Limit(50).ToListAsync();
	        return components;
        }

        public static async Task<List<AccountInfo>> QueryJsonAccounts(this DBProxyComponent self, string time)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            FilterDefinition<AccountInfo> filterDefinition = new JsonFilterDefinition<AccountInfo>($"{{CreateTime:/^{time}/}}");
            List<AccountInfo> components = await dbComponent.GetDBDataCollection<AccountInfo>(typeof(AccountInfo).Name).Find(filterDefinition).SortByDescending(a => a.CreateTime).ToListAsync();
            return components;
        }

		public static async Task<List<T>> QueryJsonDBInfos<T>(this DBProxyComponent self)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            FilterDefinition<T> filterDefinition = new JsonFilterDefinition<T>($"{{}}");
            List<T> components = await dbComponent.GetDBDataCollection<T>(typeof(T).Name).Find(filterDefinition).ToListAsync();
            return components;
        }

        public static async Task<List<T>> QueryJsonDBInfos<T>(this DBProxyComponent self, string time)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            FilterDefinition<T> filterDefinition = new JsonFilterDefinition<T>($"{{CreateTime:/^{time}/}}");
            List<T> components = await dbComponent.GetDBDataCollection<T>(typeof(T).Name).Find(filterDefinition).ToListAsync();
            return components;
        }

		public static async Task<List<T>> QueryJsonDBInfos<T>(this DBProxyComponent self, string time,long uid)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            FilterDefinition<T> filterDefinition = new JsonFilterDefinition<T>($"{{CreateTime:/^{time}/,Uid:{uid}}}");
            List<T> components = await dbComponent.GetDBDataCollection<T>(typeof(T).Name).Find(filterDefinition).ToListAsync();
            return components;
        }

        public static async Task<List<T>> QueryJsonDB<T>(this DBProxyComponent self, string json)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            FilterDefinition<T> filterDefinition = new JsonFilterDefinition<T>(json);
            List<T> components = await dbComponent.GetDBDataCollection<T>(typeof(T).Name).FindAsync(filterDefinition).Result.ToListAsync();
            return components;

        }
        public static async Task Delete<T>(this DBProxyComponent self, long id)
	    {
	        DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            await dbComponent.GetCollection(typeof(T).Name).DeleteOneAsync(i => i.Id == id);
        }

        public static async Task DeleteAll<T>(this DBProxyComponent self)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            var filter = Builders<ComponentWithId>.Filter.Empty;
            await dbComponent.GetCollection(typeof(T).Name).DeleteManyAsync(filter);
        }

        public static long QueryJsonCount<T>(this DBProxyComponent self, string json)
	    {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
	        FilterDefinition<ComponentWithId> filterDefinition = new JsonFilterDefinition<ComponentWithId>(json);
            long count = dbComponent.GetCollection(typeof(T).Name).Count(filterDefinition);
	        return count;
	    }
     
    }
}