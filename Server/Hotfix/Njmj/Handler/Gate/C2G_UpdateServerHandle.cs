using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UpdateServerHandler : AMRpcHandler<C2G_UpdateServer, G2C_UpdateServer>
    {
        protected override async void Run(Session session, C2G_UpdateServer message, Action<G2C_UpdateServer> reply)
        {
            G2C_UpdateServer response = new G2C_UpdateServer();
            try
            {
                StartConfigComponent startConfigComponent = Game.Scene.GetComponent<StartConfigComponent>();
                NetInnerComponent netInnerComponent = Game.Scene.GetComponent<NetInnerComponent>();
                foreach (StartConfig startConfig in startConfigComponent.GetAll())
                {
//                    if (!message.AppType.Is(startConfig.AppType))
//                    {
//                        continue;
//                    }
                    InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
                    Session serverSession = netInnerComponent.Get(innerConfig.IPEndPoint);
                    await serverSession.Call(new M2A_Reload());
                }
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
