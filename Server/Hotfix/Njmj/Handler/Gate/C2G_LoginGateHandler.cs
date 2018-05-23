﻿using System;
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

                #region AddShopInfo
                List<ShopInfo> shopInfoList = new List<ShopInfo>();
                for (int i = 1; i< configCom.GetAll(typeof(ShopConfig)).Length + 1; ++i)
                {
                    int id = 1000 + i;
                    ShopConfig config = (ShopConfig)configCom.Get(typeof(ShopConfig), id);
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

                

                List<Chat> chatConfigList = new List<Chat>();
                for (int i = 1; i < configCom.GetAll(typeof(ChatConfig)).Length + 1; ++i)
                {
                    ChatConfig config = (ChatConfig)configCom.Get(typeof(ChatConfig), i);
                    Chat info = new Chat();
                    info.Id = (int)config.Id;
                    info.Content = config.Content;
                    chatConfigList.Add(info);
                }
                response.ChatList = chatConfigList;

                List<NoticeInfo> noticeConfigList = new List<NoticeInfo>();
                for (int i = 1; i < configCom.GetAll(typeof(NoticeConfig)).Length + 1; ++i)
                {
                    int index = 100 + i;
                    NoticeConfig config = (NoticeConfig)configCom.Get(typeof(NoticeConfig), index);
                    NoticeInfo info = new NoticeInfo();
                    info.Id = (int)config.Id;
                    info.Content = config.Content;
                    info.Name = config.Name;
                    noticeConfigList.Add(info);
                }
                response.NoticeInfoList = noticeConfigList;

                #endregion

                #region AddItemInfo
                List<UserBag> itemInfoList = await proxyComponent.QueryJson<UserBag>($"{{UId:{userId}}}");
                if (itemInfoList.Count <= 0)
                {
                    for (int i = 1; i < 9; ++i)
                    {
                        UserBag item = new UserBag();
                        item.BagId = 100 + i;
                        item.Count = 10 + i;
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
                    emailInfo.RewardItem = "2,100;1,100";
                    DBHelper.AddEmailInfoToDB(emailInfo);
                    #endregion
                }
                
                reply(response);
				session.Send(new G2C_TestHotfixMessage() { Info = "recv hotfix message success" });
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