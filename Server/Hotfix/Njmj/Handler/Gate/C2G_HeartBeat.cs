using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_HeaerBeatHandler : AMRpcHandler<C2G_HeartBeat, G2C_HeartBeat>
    {
        protected override async void Run(Session session, C2G_HeartBeat message, Action<G2C_HeartBeat> reply)
        {
            G2C_HeartBeat response = new G2C_HeartBeat();
            try
            {
                reply(response);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                ReplyError(response, e, reply);
            }
        }
    }
}
