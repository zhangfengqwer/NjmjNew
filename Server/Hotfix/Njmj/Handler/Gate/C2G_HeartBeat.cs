using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_HeartBeatHandler : AMRpcHandler<C2G_HeartBeat, G2C_HeartBeat>
    {
        protected override async void Run(Session session, C2G_HeartBeat message, Action<G2C_HeartBeat> reply)
        {
            G2C_HeartBeat response = new G2C_HeartBeat();
            try
            {
                if (session.GetComponent<HeartBeatComponent>() != null)
                {
                    session.GetComponent<HeartBeatComponent>().CurrentTime = TimeHelper.ClientNowSeconds();
                }
//                Log.Info("服务端发送心跳包");
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
