using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_MyFriendRankHandler : AMRpcHandler<C2G_MyFriendRank, G2C_MyFriendRank>
    {
        protected override async void Run(Session session, C2G_MyFriendRank message, Action<G2C_MyFriendRank> reply)
        {
            G2C_MyFriendRank response = new G2C_MyFriendRank();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                string result = await FriendRoomRecord.getRecord(message.UId);
                response.Data = result;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
