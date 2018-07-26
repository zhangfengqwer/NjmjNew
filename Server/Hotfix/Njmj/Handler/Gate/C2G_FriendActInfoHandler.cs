using ETModel;
using System;
using System.Collections.Generic;
using System.Net;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_FriendActInfoHandler : AMRpcHandler<C2G_FriendActInfo, G2C_FriendActInfo>
    {
        protected override async void Run(Session session, C2G_FriendActInfo message, Action<G2C_FriendActInfo> reply)
        {
            G2C_FriendActInfo response = new G2C_FriendActInfo();
            try
            {
                //获取房间信息
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<FriendKeyConsum> consums = await proxyComponent.QueryJson<FriendKeyConsum>($"{{UId:{message.UId},CreateTime:/^{DateTime.Now.GetCurrentDay()}/}}");
                if(consums.Count > 0)
                {
                    response.GetCount = consums[0].GetCount;
                    response.ConsumCount = consums[0].ConsumCount;
                }
                else
                {
                    response.ConsumCount = 0;
                    response.GetCount = 0;
                }
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
