using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using Newtonsoft.Json.Linq;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate>
	{
		protected override async void Run(Session session, C2G_LoginGate message, Action<G2C_LoginGate> reply)
		{
			G2C_LoginGate response = new G2C_LoginGate();
			try
			{
			    long userId = Game.Scene.GetComponent<NjmjGateSessionKeyComponent>().Get(message.Key);
			    if (userId == 0)
			    {
			        response.Error = ErrorCode.ERR_ConnectGateKeyError;
			        response.Message = "Gate key验证失败!";
			        reply(response);
			        return;
			    }

                // 检测是否已存在
                UserComponentSystem.CheckIsExistTheUser(userId);

                //创建User对象
                User user = UserFactory.Create(userId, session);
			    await user.AddComponent<ActorComponent>().AddLocation();

                //添加User对象关联到Session上
                session.AddComponent<SessionUserComponent>().User = user;
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

                if (ShopData.getInstance().getDataList().Count == 0)
                {
                    List<ShopConfig> shopList = new List<ShopConfig>();
                    for (int i = 1; i < configCom.GetAll(typeof(ShopConfig)).Length + 1; ++i)
                    {
                        int id = 1000 + i;
                        ShopConfig config = (ShopConfig)configCom.Get(typeof(ShopConfig), id);
                        shopList.Add(config);
                    }
                    ShopData.getInstance().getDataList().AddRange(shopList);
                }

                //#region AddShopInfo
                List<ShopInfo> shopInfoList = new List<ShopInfo>();
                for (int i = 0; i < ShopData.getInstance().getDataList().Count; ++i)
                {
                    ShopConfig config = ShopData.getInstance().getDataList()[i];
                    ShopInfo info = new ShopInfo();
                    info.Id = config.Id;
                    info.Name = config.Name;
                    info.Price = config.Price;
                    info.ShopType = config.shopType;
                    info.Desc = config.Desc;
                    info.CurrencyType = config.CurrencyType;
                    info.Items = config.Items;
                    info.Icon = config.Icon;
                    info.VipPrice = config.VipPrice;
                    shopInfoList.Add(info);
                }

                #region AddItemInfo
                List<UserBag> itemInfoList = await proxyComponent.QueryJson<UserBag>($"{{UId:{userId}}}");
                if (itemInfoList.Count <= 0)
                {
                    {
                        UserBag item = new UserBag();
                        item.BagId = 104;
                        item.Count = 10;
                        item.UId = userId;
                        DBHelper.AddItemToDB(item);
                    }

                    {
                        UserBag item = new UserBag();
                        item.BagId = 105;
                        item.Count = 10;
                        item.UId = userId;
                        DBHelper.AddItemToDB(item);
                    }

                    {
                        UserBag item = new UserBag();
                        item.BagId = 107;
                        item.Count = 10;
                        item.UId = userId;
                        DBHelper.AddItemToDB(item);
                    }

                    {
                        UserBag item = new UserBag();
                        item.BagId = 108;
                        item.Count = 10;
                        item.UId = userId;
                        DBHelper.AddItemToDB(item);
                    }

                    {
                        UserBag item = new UserBag();
                        item.BagId = 109;
                        item.Count = 10;
                        item.UId = userId;
                        DBHelper.AddItemToDB(item);
                    }

                    {
                        UserBag item = new UserBag();
                        item.BagId = 111;
                        item.Count = 10;
                        item.UId = userId;
                        DBHelper.AddItemToDB(item);
                    }
                }
                #endregion

                //添加消息转发组件
                await session.AddComponent<ActorComponent, string>(ActorType.GateSession).AddLocation();

                response.PlayerId = user.Id;
                response.Uid = userId;
                response.ShopInfoList = shopInfoList;

                List<UserBag> bagInfoList = await proxyComponent.QueryJson<UserBag>($"{{UId:{userId}}}");
                response.BagList = new List<Bag>();
                List<Bag> bagList = new List<Bag>();
                for(int i = 0;i< bagInfoList.Count; ++i)
                {
                    Bag bag = new Bag();
                    bag.ItemId = bagInfoList[i].BagId;
                    bag.Count = bagInfoList[i].Count;
                    bagList.Add(bag);
                }
                response.BagList = bagList;

                List<EmailInfo> emailInfos = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{userId}}}");
                if (emailInfos.Count <= 0)
                {
                    {
                        #region emailTest
                        {
                            for (int i = 1; i <= 30; ++i)
                            {
                                int id = 100 + i;
                                EmailInfo emailInfo = new EmailInfo();
                                emailInfo.EmailId = id;
                                emailInfo.UId = userId;
                                //emailInfo.EmailTitle = "南京麻将官方QQ群:697413923";
                                emailInfo.EmailTitle = "南京麻将假期送好礼！";
                                //emailInfo.Content = "加入南京麻将官方QQ群:697413923，官方客服妹子为您解答各种问题，了解更多游戏首发资讯，南麻资深玩家聚集地，期待您的加入。";
                                emailInfo.Content = "加入南京麻将，就有好礼相送";
                                emailInfo.State = 0;
                                emailInfo.RewardItem = "2:100;1:100";
                                DBHelper.AddEmailInfoToDB(emailInfo);
                            }
                        }

                        {
                            for (int i = 1; i <= 30; ++i)
                            {
                                int id = 100 + i + 30;
                                EmailInfo emailInfo = new EmailInfo();
                                emailInfo.EmailId = id;
                                emailInfo.UId = userId;
                                //emailInfo.EmailTitle = "南京麻将官方QQ群:697413923";
                                emailInfo.EmailTitle = "南京麻将假期送好礼！";
                                //emailInfo.Content = "加入南京麻将官方QQ群:697413923，官方客服妹子为您解答各种问题，了解更多游戏首发资讯，南麻资深玩家聚集地，期待您的加入。";
                                emailInfo.Content = "加入南京麻将，就有好礼相送";
                                emailInfo.State = 1;
                                emailInfo.RewardItem = "2:100;1:100";
                                DBHelper.AddEmailInfoToDB(emailInfo);
                            }
                        }

                    }
                    #endregion
                }
                
                reply(response);
				session.Send(new G2C_TestHotfixMessage() { Info = "recv hotfix message success" });

                PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(userId);

                // vip上线全服广播
                {
                    if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                    {
                        Actor_LaBa actor_LaBa = new Actor_LaBa();
                        actor_LaBa.LaBaContent = "贵族玩家" + playerBaseInfo.Name + "上线啦！";
                        Game.Scene.GetComponent<UserComponent>().BroadCast(actor_LaBa);
                    }
                }

                // 老用户检测
                {
                    try
                    {
                        AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(userId);
                        if (accountInfo.OldAccountState == 1)
                        {
                            string url = "http://fksq.javgame.com:10086/?machine_id=" + accountInfo.MachineId + "&game_id=217";
                            string str = HttpUtil.GetHttp(url);
                            Log.Debug("web地址：" + url);
                            Log.Debug("判断是否是老用户：" + str);

                            JObject result = JObject.Parse(str);
                            string old_uid = (string)result.GetValue("old_uid");

                            // 不是老用户
                            if (string.IsNullOrEmpty(old_uid))
                            {
                                accountInfo.OldAccountState = 3;
                                await proxyComponent.Save(accountInfo);
                            }
                            // 是老用户
                            else
                            {
                                accountInfo.OldAccountState = 2;
                                await proxyComponent.Save(accountInfo);

                                {
                                    playerBaseInfo.GoldNum = 1000;
                                    playerBaseInfo.WingNum = 100;
                                    await proxyComponent.Save(playerBaseInfo);
                                }

                                // 发送老用户广播
                                Actor_OldUser actor_OldUser = new Actor_OldUser();
                                Game.Scene.GetComponent<UserComponent>().BroadCast(actor_OldUser);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("检测是否是老用户出错:" + ex);
                    }
                }
            }
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}

        private void AddShopInfo()
        {
            

        }
	}
}