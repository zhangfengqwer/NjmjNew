using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    class DBQueryJsonGamePlayerRequestHandler : AMRpcHandler<DBQueryJsonGamePlayerRequest, DBQueryJsonGamePlayerResponse>
    {
        protected override async void Run(Session session, DBQueryJsonGamePlayerRequest message, Action<DBQueryJsonGamePlayerResponse> reply)
        {
            DBQueryJsonGamePlayerResponse response = new DBQueryJsonGamePlayerResponse();
            try
            {
                DBCacheComponent dbCacheComponent = Game.Scene.GetComponent<DBCacheComponent>();
                List<PlayerBaseInfo> components = await dbCacheComponent.GetGamePlayerJson(message.CollectionName, message.Json);
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
