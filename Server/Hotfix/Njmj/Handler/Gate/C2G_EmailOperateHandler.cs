using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EmailOperateHandler : AMRpcHandler<C2G_EmailOperate, G2C_EmailOperate>
    {
        protected override async void Run(Session session, C2G_EmailOperate message, Action<G2C_EmailOperate> reply)
        {
            G2C_EmailOperate response = new G2C_EmailOperate();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<EmailInfo> emailInfoList = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.Uid},EmailId:{message.EmailId}}}");
                if (emailInfoList.Count > 0)
                {
                    if (message.state == 1)
                    {
                        //领取奖励
                        PlayerBaseInfo baseInfo = await proxyComponent.Query<PlayerBaseInfo>(message.Uid);
                        PlayerInfo playerInfo = new PlayerInfo();
                        for (int i = 0; i < message.InfoList.Count; ++i)
                        {
                            GetItemInfo getItem = message.InfoList[i];
                            if (getItem.ItemID == 1 || getItem.ItemID == 2)
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
                                List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.Uid},BagId:{getItem.ItemID}}}");
                                if (itemInfos.Count > 0)
                                {
                                    itemInfos[0].Count += getItem.Count;
                                    await proxyComponent.Save(itemInfos[0]);
                                }
                                else
                                {
                                    UserBag itemInfo = ComponentFactory.CreateWithId<UserBag>(IdGenerater.GenerateId());
                                    itemInfo.BagId = getItem.ItemID;
                                    itemInfo.UId = message.Uid;
                                    itemInfo.Count = getItem.Count;
                                    await proxyComponent.Save(itemInfo);
                                }
                            }
                        }
                        emailInfoList[0].State = 1;
                    }
                    else if (message.state == 2)
                    {
                        //删除邮件
                        emailInfoList[0].State = 2;//1,已领取 2,删除
                    }
                    await proxyComponent.Save(emailInfoList[0]);
                }
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
