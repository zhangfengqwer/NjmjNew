using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RealNameHandler : AMRpcHandler<C2G_RealName, G2C_RealName>
    {
        protected override async void Run(Session session, C2G_RealName message, Action<G2C_RealName> reply)
        {
            G2C_RealName response = new G2C_RealName();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<RealName> dailySigns = await proxyComponent.QueryJson<RealName>($"{{Uid:{message.Uid}}}");

                if (dailySigns.Count > 0)
                {
                    // 已经实名过
                    response.Error = ErrorCode.TodayHasSign;
                    response.Message = "您已实名认证，请勿重复认证";
                    reply(response);

                    return;
                }
                else
                {
                    reply(response);

                    RealName realName = ComponentFactory.CreateWithId<RealName>(IdGenerater.GenerateId());
                    realName.Uid = message.Uid;
                    realName.Name = message.Name;
                    realName.IDNumber = message.IDNumber;
                    await proxyComponent.Save(realName);
                    
                    List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");
                    if (playerBaseInfos.Count > 0)
                    {
                        playerBaseInfos[0].IsRealName = true;
                        await proxyComponent.Save(playerBaseInfos[0]);
                    }
                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
