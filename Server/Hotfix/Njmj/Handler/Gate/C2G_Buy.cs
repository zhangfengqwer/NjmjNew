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
                PlayerBaseInfo baseInfo = await proxyComponent.Query<PlayerBaseInfo>(message.UId);
                PlayerInfo playerInfo = new PlayerInfo();
                baseInfo.WingNum -= message.Info.Cost;
                //购买元宝
                if (message.Info.ShopId == 1)
                {
                    baseInfo.GoldNum += message.Info.Count;
                }
                else
                {
                    List<ItemInfo> itemInfos = await proxyComponent.QueryJson<ItemInfo>($"{{UId:{message.UId},BagId:{message.Info.ShopId}}}");
                    if (itemInfos.Count > 0)
                    {
                        itemInfos[0].Count += message.Info.Count;
                        await proxyComponent.Save(itemInfos[0]);
                    }
                    else
                    {
                        ItemInfo itemInfo = new ItemInfo();
                        itemInfo.BagId = message.Info.ShopId;
                        itemInfo.UId = message.UId;
                        itemInfo.Count = message.Info.Count;
                        DBHelper.AddItemToDB(itemInfo);
                    }
                }
                response.Result = true;
                await proxyComponent.Save(baseInfo);

                playerInfo.GoldNum = baseInfo.GoldNum;
                playerInfo.WingNum = baseInfo.WingNum;
                playerInfo.Icon = baseInfo.Icon;
                playerInfo.Name = baseInfo.Name;

                reply(response);
                session.Send(new Actor_UpDateData { playerInfo = playerInfo });
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
