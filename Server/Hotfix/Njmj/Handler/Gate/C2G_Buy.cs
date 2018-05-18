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
                baseInfo.WingNum -= message.Cost;
                //购买元宝
                if (message.Info.ItemID == 1)
                {
                    baseInfo.GoldNum += message.Info.Count;
                    await proxyComponent.Save(baseInfo);
                    response.Result = true;
                    reply(response);
                }
                else
                {
                    List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{message.Info.ItemID}}}");
                    if (itemInfos.Count > 0)
                    {
                        itemInfos[0].Count += message.Info.Count;
                        await proxyComponent.Save(itemInfos[0]);
                    }
                    else
                    {
                        UserBag itemInfo = new UserBag();
                        itemInfo.BagId = message.Info.ItemID;
                        itemInfo.UId = message.UId;
                        itemInfo.Count = message.Info.Count;
                        DBHelper.AddItemToDB(itemInfo);
                    }
                    response.Result = true;
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
