using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UseLaBaHandler : AMRpcHandler<C2G_UseLaBa, G2C_UseLaBa>
    {
        protected override async void Run(Session session, C2G_UseLaBa message, Action<G2C_UseLaBa> reply)
        {
            G2C_UseLaBa response = new G2C_UseLaBa();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<UserBag> userBags = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.Uid},BagId:{105}}}");
                if (userBags.Count == 0)
                {
                    response.Error = ErrorCode.TodayHasSign;
                    response.Message = "您没有喇叭可以使用";
                    reply(response);

                    return;
                }
                else
                {
                    // 扣除喇叭
                    --userBags[0].Count;
                    await proxyComponent.Save(userBags[0]);

                    reply(response);

                    // 全服广播
                    {
                        Actor_LaBa actor_LaBa = new Actor_LaBa();
                        actor_LaBa.LaBaContent = message.Content;
                        Game.Scene.GetComponent<UserComponent>().BroadCast(actor_LaBa);
                    }
                }
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
