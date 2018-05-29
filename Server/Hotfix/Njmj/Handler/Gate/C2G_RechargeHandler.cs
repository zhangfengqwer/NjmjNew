using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RechargeHandler : AMRpcHandler<C2G_Recharge, G2C_Recharge>
    {
        protected override async void Run(Session session, C2G_Recharge message, Action<G2C_Recharge> reply)
        {
            G2C_Recharge response = new G2C_Recharge();
            try
            {
                string reward = "";
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                ShopConfig config = ShopData.getInstance().GetDataByShopId(message.GoodsId);

                List<Log_Recharge> log_Recharge = await proxyComponent.QueryJson<Log_Recharge>($"{{Uid:{message.UId}}}");
                if (log_Recharge.Count == 0)
                {
                    reward = "1:120000;105:20;104:1;107:1;";
                }

                reward += config.Items;

                PlayerBaseInfo baseInfo = await DBCommonUtil.getPlayerBaseInfo(message.UId);
                await DBCommonUtil.changeWealthWithStr(message.UId, reward);

                // 记录日志
                {
                    Log_Recharge log_recharge = ComponentFactory.CreateWithId<Log_Recharge>(IdGenerater.GenerateId());
                    log_recharge.Uid = message.UId;
                    log_recharge.GoodsId = config.Id;
                    log_recharge.Price = config.Price;
                    await proxyComponent.Save(log_recharge);
                }

                response.GoodsId = config.Id;
                response.Reward = reward;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
