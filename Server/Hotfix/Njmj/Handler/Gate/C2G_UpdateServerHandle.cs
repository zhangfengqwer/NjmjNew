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
                Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
