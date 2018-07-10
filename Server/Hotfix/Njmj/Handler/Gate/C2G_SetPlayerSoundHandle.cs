using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_SetPlayerSoundHandler : AMRpcHandler<C2G_SetPlayerSound, G2C_SetPlayerSound>
    {
        protected override async void Run(Session session, C2G_SetPlayerSound message, Action<G2C_SetPlayerSound> reply)
        {
            G2C_SetPlayerSound response = new G2C_SetPlayerSound();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");
                playerBaseInfos[0].PlayerSound = message.PlayerSound;
                await proxyComponent.Save(playerBaseInfos[0]);

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
