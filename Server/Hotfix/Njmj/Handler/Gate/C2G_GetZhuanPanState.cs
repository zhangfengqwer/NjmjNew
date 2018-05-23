using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetZhuanPanStateHandler : AMRpcHandler<C2G_GetZhuanPanState, G2C_GetZhuanPanState>
    {
        protected override async void Run(Session session, C2G_GetZhuanPanState message, Action<G2C_GetZhuanPanState> reply)
        {
            G2C_GetZhuanPanState response = new G2C_GetZhuanPanState();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");

                response.LuckyValue = playerBaseInfos[0].LuckyValue;
                response.ZhuanPanCount = playerBaseInfos[0].ZhuanPanCount;

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
