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
                
                ShopConfig config = ConfigHelp.Get<ShopConfig>(message.ShopId);
                int shopId = CommonUtil.splitStr_Start(config.Items.ToString(), ':');
                int count = CommonUtil.splitStr_End(config.Items.ToString(), ':');
                int cost = 0;
                if (await DBCommonUtil.IsVIP(message.UId))
                {
                    cost = config.VipPrice;
                }
                else
                {
                    cost = config.Price;
                }

                await DBCommonUtil.ChangeWealth(message.UId, config.CurrencyType, -cost, $"花费{cost}元宝购买商品{config.Name}");

                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");

                switch (config.CurrencyType)
                {
                    //金币
                    case 1:
                        response.Wealth = playerBaseInfos[0].GoldNum;
                        response.CurrencyType = 1;
                        break;
                    //元宝
                    case 2:
                        response.Wealth = playerBaseInfos[0].WingNum;
                        response.CurrencyType = 2;
                        break;
                }
                //购买金币
                if (shopId == 1)
                {
                    await DBCommonUtil.ChangeWealth(message.UId, 1,count, $"商城购买{count}金币");
                    response.Count = count;
                    reply(response);
                }
                else
                {
                    List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{shopId}}}");
                    await DBCommonUtil.ChangeWealth(message.UId, shopId, count, $"商城购买{count}个{config.Name}道具");
                    response.Count = count;
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
