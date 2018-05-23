using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ChangeWealthHandler : AMRpcHandler<C2G_ChangeWealth, G2C_ChangeWealth>
    {
        protected override async void Run(Session session, C2G_ChangeWealth message, Action<G2C_ChangeWealth> reply)
        {
            G2C_ChangeWealth response = new G2C_ChangeWealth();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                switch (message.propId)
                {
                    // 金币
                    case 1:
                        {
                            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{UId:{message.UId}}}");
                            playerBaseInfos[0].GoldNum += (int)message.propNum;
                            if (playerBaseInfos[0].GoldNum < 0)
                            {
                                playerBaseInfos[0].GoldNum = 0;
                            }
                            await proxyComponent.Save(playerBaseInfos[0]);
                        }
                        break;

                    // 元宝
                    case 2:
                        {
                            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{UId:{message.UId}}}");
                            playerBaseInfos[0].WingNum += (int)message.propNum;
                            if (playerBaseInfos[0].WingNum < 0)
                            {
                                playerBaseInfos[0].WingNum = 0;
                            }
                            await proxyComponent.Save(playerBaseInfos[0]);
                        }
                        break;

                    // 话费
                    case 3:
                        {
                            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{UId:{message.UId}}}");
                            playerBaseInfos[0].HuaFeiNum += message.propNum;
                            if (playerBaseInfos[0].HuaFeiNum < 0)
                            {
                                playerBaseInfos[0].HuaFeiNum = 0;
                            }
                            await proxyComponent.Save(playerBaseInfos[0]);
                        }
                        break;

                    // 其他道具
                    default:
                        {
                            List<UserBag> userBags = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{message.propId}}}");
                            if (userBags.Count == 0)
                            {
                                UserBag itemInfo = new UserBag();
                                itemInfo.BagId = message.propId;
                                itemInfo.UId = message.UId;
                                itemInfo.Count = (int)message.propNum;
                                DBHelper.AddItemToDB(itemInfo);
                            }
                            else
                            {
                                userBags[0].Count += (int)message.propNum;
                                if (userBags[0].Count < 0)
                                {
                                    userBags[0].Count = 0;
                                }
                                await proxyComponent.Save(userBags[0]);
                            }
                        }
                        break;
                }

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
