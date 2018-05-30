using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

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

			    //创建User对象
			    User user = UserFactory.Create(userId, session.Id);
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
                    #region emailTest
                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo.EmailId = 101;
                    emailInfo.UId = userId;
                    //emailInfo.EmailTitle = "南京麻将官方QQ群:697413923";
                    emailInfo.EmailTitle = "南京麻将假期送好礼！";
                    emailInfo.Date = new StringBuilder()
                                    .Append(CommonUtil.getCurYear())
                                    .Append("-")
                                    .Append(CommonUtil.getCurMonth())
                                    .Append("-")
                                    .Append(CommonUtil.getCurDay()).ToString();
                    //emailInfo.Content = "加入南京麻将官方QQ群:697413923，官方客服妹子为您解答各种问题，了解更多游戏首发资讯，南麻资深玩家聚集地，期待您的加入。";
                    emailInfo.Content = "加入南京麻将，就有好礼相送";
                    emailInfo.State = 0;
                    emailInfo.RewardItem = "2:100;1:100";
                    DBHelper.AddEmailInfoToDB(emailInfo);
                    #endregion
                }
                
                reply(response);
				session.Send(new G2C_TestHotfixMessage() { Info = "recv hotfix message success" });

                // vip上线全服广播
                {
                    PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(userId);
                    
                    if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                    {
                        Actor_LaBa actor_LaBa = new Actor_LaBa();
                        actor_LaBa.LaBaContent = "贵族玩家" + playerBaseInfo.Name + "上线啦！";
                        Game.Scene.GetComponent<UserComponent>().BroadCast(actor_LaBa);
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