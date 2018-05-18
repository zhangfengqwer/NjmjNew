using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    class DBQueryJsonPlayerInfoRequestHandler : AMRpcHandler<DBQueryJsonPlayerInfoRequest, DBQueryJsonPlayerInfoResponse>
    {
        protected override async void Run(Session session, DBQueryJsonPlayerInfoRequest message, Action<DBQueryJsonPlayerInfoResponse> reply)
        {
            DBQueryJsonPlayerInfoResponse response = new DBQueryJsonPlayerInfoResponse();
            try
            {
                DBCacheComponent dbCacheComponent = Game.Scene.GetComponent<DBCacheComponent>();
                List<PlayerBaseInfo> components = await dbCacheComponent.GetPlayerBaseInfoJson(message.CollectionName, message.Json);
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
