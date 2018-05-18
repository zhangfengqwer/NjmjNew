using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetItemHandler : AMRpcHandler<C2G_GetItem, G2C_GetItem>
    {
        protected override async void Run(Session session, C2G_GetItem message, Action<G2C_GetItem> reply)
        {
            G2C_GetItem response = new G2C_GetItem();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                PlayerBaseInfo baseInfo = await proxyComponent.Query<PlayerBaseInfo>(message.UId);
                PlayerInfo playerInfo = new PlayerInfo();
                for (int i = 0;i< message.InfoList.Count; ++i)
                {
                    GetItemInfo getItem = message.InfoList[i];
                    if(getItem.ItemID == 1 || getItem.ItemID == 2)
                    {
                        //如果奖励是元宝和金币
                        if (getItem.ItemID == 1)
                            baseInfo.WingNum += getItem.Count;
                        if (getItem.ItemID == 2)
                            baseInfo.GoldNum += getItem.Count;
                        await proxyComponent.Save(baseInfo);
                    }
                    else
                    {
                        //物品道具
                        List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{getItem.ItemID}}}");
                        if (itemInfos.Count > 0)
                        {
                            itemInfos[0].Count += getItem.Count;
                            await proxyComponent.Save(itemInfos[0]);
                        }
                        else
                        {
                            UserBag itemInfo = new UserBag();
                            itemInfo.BagId = getItem.ItemID;
                            itemInfo.UId = message.UId;
                            itemInfo.Count = getItem.Count;
                            DBHelper.AddItemToDB(itemInfo);
                        }
                    }
                }
                List<EmailInfo> emailInfoList = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.UId},_id:{message.MailId}}}");
                if(emailInfoList.Count > 0)
                {
                    emailInfoList[0].State = 1;
                    await proxyComponent.Save(emailInfoList[0]);
                }
                response.Result = true;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
