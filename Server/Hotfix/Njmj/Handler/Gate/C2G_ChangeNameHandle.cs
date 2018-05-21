using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ChangeNameHandler : AMRpcHandler<C2G_ChangeName, G2C_ChangeName>
    {
        protected override async void Run(Session session, C2G_ChangeName message, Action<G2C_ChangeName> reply)
        {
            G2C_ChangeName response = new G2C_ChangeName();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{Name:'{message.Name}'}}");
                if (playerBaseInfos.Count > 0)
                {
                    // 昵称重复
                    response.Error = ErrorCode.TodayHasSign;
                    response.Message = "昵称已被使用，请换个昵称";
                    reply(response);

                    return;
                }

                playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");
                
                playerBaseInfos[0].Name = message.Name;
                await proxyComponent.Save(playerBaseInfos[0]);

                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
