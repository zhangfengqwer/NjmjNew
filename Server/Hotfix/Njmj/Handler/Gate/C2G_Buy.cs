using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_Buy:AMRpcHandler<C2G_BuyItem, G2C_BuyItem>
    {
        protected override async void Run(Session session, C2G_BuyItem message, Action<G2C_BuyItem> reply)
        {
            G2C_BuyItem response = new G2C_BuyItem();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                await DBCommonUtil.ChangeWealth(message.UId, 2, -message.Cost, $"花费元宝购买商品{message.Info.ItemID}");
                //购买金币
                if (message.Info.ItemID == 1)
                {
                    await DBCommonUtil.ChangeWealth(message.UId, 1, message.Info.Count, "商城购买金币");
                    response.Count = message.Info.Count;
                    reply(response);
                }
                else
                {
                    List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{message.Info.ItemID}}}");
                    await DBCommonUtil.ChangeWealth(message.UId, message.Info.ItemID, message.Info.Count, $"商城购买道具{message.Info.ItemID}");
                    response.Count = message.Info.Count;
                    reply(response);
                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
