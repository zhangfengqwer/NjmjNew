using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.DB)]
	public class DBQueryJsonRequestHandler : AMRpcHandler<DBQueryJsonRequest, DBQueryJsonResponse>
	{
		protected override async void Run(Session session, DBQueryJsonRequest message, Action<DBQueryJsonResponse> reply)
		{
			DBQueryJsonResponse response = new DBQueryJsonResponse();
			try
			{
			    Log.Info("33333");
                DBCacheComponent dbCacheComponent = Game.Scene.GetComponent<DBCacheComponent>();
			    Log.Info("33333");
                List<ComponentWithId> components = await dbCacheComponent.GetJson(message.CollectionName, message.Json);
				response.Components = components;
				reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}