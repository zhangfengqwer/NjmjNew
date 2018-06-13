using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
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

	    public static async Task<List<PlayerBaseInfo>> QueryJsonPlayerInfo(this DBProxyComponent self)
	    {
	        DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
	        FilterDefinition<PlayerBaseInfo> filterDefinition = new JsonFilterDefinition<PlayerBaseInfo>($"{{}}");
	        List<PlayerBaseInfo> components = await dbComponent.GetPlayerBaseInfoCollection(typeof(PlayerBaseInfo).Name).Find(filterDefinition).SortByDescending(a => a.GoldNum).Limit(30).ToListAsync();
	        return components;
	    }

	    public static async Task<List<PlayerBaseInfo>> QueryJsonGamePlayer(this DBProxyComponent self)
	    {
	        DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
	        FilterDefinition<PlayerBaseInfo> filterDefinition = new JsonFilterDefinition<PlayerBaseInfo>($"{{}}");
	        List<PlayerBaseInfo> components = await dbComponent.GetPlayerBaseInfoCollection(typeof(PlayerBaseInfo).Name).Find(filterDefinition).SortByDescending(a => a.WinGameCount).Limit(30).ToListAsync();
	        return components;
	    }

//        public static async Task<List<T>> QueryJsonPlayerInfo<T>(this DBProxyComponent self, string json) where T : PlayerBaseInfo
//        {
//            List<T> list = new List<T>();
//            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
//            DBQueryJsonPlayerInfoResponse dbQueryJsonResponse = (DBQueryJsonPlayerInfoResponse)await session.Call(new DBQueryJsonPlayerInfoRequest { CollectionName = typeof(T).Name, Json = json });
//            foreach (PlayerBaseInfo component in dbQueryJsonResponse.Components)
//            {
//                list.Add((T)component);
//            }
//            return list;
//        }
//
//        public static async Task<List<T>> QueryJsonGamePlayer<T>(this DBProxyComponent self,string json) where T : PlayerBaseInfo
//        {
//            List<T> list = new List<T>();
//            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
//            DBQueryJsonGamePlayerResponse dbQueryJsonResponse = (DBQueryJsonGamePlayerResponse)await session.Call(new DBQueryJsonGamePlayerRequest { CollectionName = typeof(T).Name, Json = json });
//            foreach (PlayerBaseInfo component in dbQueryJsonResponse.Components)
//            {
//                list.Add((T)component);
//            }
//            return list;
//        }
    }
}