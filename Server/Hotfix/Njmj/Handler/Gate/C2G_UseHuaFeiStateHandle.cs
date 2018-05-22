using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UseHuaFeiStateHandler : AMRpcHandler<C2G_UseHuaFeiState, G2C_UseHuaFeiState>
    {
        protected override async void Run(Session session, C2G_UseHuaFeiState message, Action<G2C_UseHuaFeiState> reply)
        {
            G2C_UseHuaFeiState response = new G2C_UseHuaFeiState();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

                // 1元
                List<UseHuaFei> useHuaFeis = await proxyComponent.QueryJson<UseHuaFei>($"{{Uid:{message.Uid},HuaFei:{1}}}");
                if (useHuaFeis.Count > 0)
                {
                    response.HuaFei_1_RestCount = 0;
                }
                else
                {
                    response.HuaFei_1_RestCount = 1;
                }
                
                // 5元
                useHuaFeis = await proxyComponent.QueryJson<UseHuaFei>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.Uid},HuaFei:{5}}}");
                if (useHuaFeis.Count > 0)
                {
                    response.HuaFei_5_RestCount = 0;
                }
                else
                {
                    response.HuaFei_5_RestCount = 1;
                }

                // 10元
                useHuaFeis = await proxyComponent.QueryJson<UseHuaFei>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.Uid},HuaFei:{10}}}");
                if (useHuaFeis.Count > 0)
                {
                    response.HuaFei_10_RestCount = 0;
                }
                else
                {
                    response.HuaFei_10_RestCount = 1;
                }

                // 20元
                useHuaFeis = await proxyComponent.QueryJson<UseHuaFei>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.Uid},HuaFei:{20}}}");
                if (useHuaFeis.Count > 0)
                {
                    response.HuaFei_20_RestCount = 0;
                }
                else
                {
                    response.HuaFei_20_RestCount = 1;
                }

                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
