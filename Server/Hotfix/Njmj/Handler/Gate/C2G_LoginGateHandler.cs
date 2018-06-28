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

                //添加心跳包
			    session.AddComponent<HeartBeatComponent>().CurrentTime = TimeHelper.ClientNowSeconds();
                //添加User对象关联到Session上
                session.AddComponent<SessionUserComponent>().User = user;

			    //添加消息转发组件
			    await session.AddComponent<ActorComponent, string>(ActorType.GateSession).AddLocation();

			    response.PlayerId = user.Id;
			    response.Uid = userId;

                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

                {
                    //商城
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
                    response.ShopInfoList = shopInfoList;
                }
                {
                    //任务
                    if (TaskData.getInstance().getDataList().Count == 0)
                    {
                        List<TaskConfig> taskList = new List<TaskConfig>();
                        for (int i = 1; i < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++i)
                        {
                            int id = 100 + i;
                            TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                            taskList.Add(config);
                        }
                        TaskData.getInstance().getDataList().AddRange(taskList);
                    }
                }

                {
                    //成就
                    if (ChengjiuData.getInstance().getDataList().Count == 0)
                    {
                        List<ChengjiuConfig> chengjiuList = new List<ChengjiuConfig>();
                        for (int i = 1; i < configCom.GetAll(typeof(ChengjiuConfig)).Length + 1; ++i)
                        {
                            int id = 100 + i;
                            ChengjiuConfig config = (ChengjiuConfig)configCom.Get(typeof(ChengjiuConfig), id);
                            chengjiuList.Add(config);
                        }
                        ChengjiuData.getInstance().getDataList().AddRange(chengjiuList);
                    }
                }

                List<DuanwuActivityInfo> infos = await proxyComponent.QueryJson<DuanwuActivityInfo>($"{{UId:{userId}}}");
			    //
			    if (infos.Count <= 0)
			    {
			        for (int j = 0; j < configCom.GetAll(typeof(DuanwuActivityConfig)).Length; ++j)
			        {
			            int id = 100 + j + 1;
			            DuanwuActivityConfig config =
			                (DuanwuActivityConfig)configCom.Get(typeof(DuanwuActivityConfig), id);
			            DuanwuActivityInfo info =
			                ComponentFactory.CreateWithId<DuanwuActivityInfo>(IdGenerater.GenerateId());
			            info.UId = userId;
			            info.TaskId = (int)config.Id;
			            info.Target = config.Target;
			            info.Reward = config.Reward;
			            info.Desc = config.Desc;
			            await proxyComponent.Save(info);
			        }
			    }

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

                PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(userId);

                // 老用户检测
                {
                    try
                    {
                        AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(userId);
                        if (accountInfo.OldAccountState == 1)
                        {
                            string url = "http://fksq.hy51v.com:10086/CheckIsOldUser?machine_id=" + accountInfo.MachineId + "&game_id=217";
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
                                List<Log_OldUserBind> log_OldUserBinds = await proxyComponent.QueryJson<Log_OldUserBind>($"{{macId:'{accountInfo.MachineId}'}}");
                                if (log_OldUserBinds.Count > 0)
                                {
                                    accountInfo.OldAccountState = 3;
                                    await proxyComponent.Save(accountInfo);
                                }
                                else
                                {
                                    accountInfo.OldAccountState = 2;
                                    await proxyComponent.Save(accountInfo);

                                    // 记录绑定日志
                                    {
                                        Log_OldUserBind log_OldUserBind = ComponentFactory.CreateWithId<Log_OldUserBind>(IdGenerater.GenerateId());
                                        log_OldUserBind.Uid = userId;
                                        log_OldUserBind.OldUid = old_uid;
                                        log_OldUserBind.macId = accountInfo.MachineId;

                                        await proxyComponent.Save(log_OldUserBind);
                                    }

                                    {
                                        url = ("http://fksq.hy51v.com:10086/GetOldNjmjData?UserId=" + old_uid);
                                        str = HttpUtil.GetHttp(url);

                                        result = JObject.Parse(str);
                                        int moneyAmount = (int)result.GetValue("moneyAmount");
                                        int gIngotAmount = (int)result.GetValue("gIngotAmount");

                                        Log.Debug("老用户金币=" + moneyAmount + "   元宝=" + gIngotAmount);

                                        playerBaseInfo.GoldNum = moneyAmount;
                                        playerBaseInfo.WingNum = gIngotAmount;
                                        await proxyComponent.Save(playerBaseInfo);

                                    }

                                    // 发送老用户广播
                                    Actor_OldUser actor_OldUser = new Actor_OldUser();
                                    actor_OldUser.OldAccount = old_uid;
                                    Game.Scene.GetComponent<UserComponent>().BroadCastToSingle(actor_OldUser, userId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("检测是否是老用户出错:" + ex);
                    }
                }

			    {
			        List<DuanwuDataBase> duanwuDataBases = await proxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{userId}}}");
			        if (duanwuDataBases.Count <= 0)
			        {
			            //新建一个数据库的表
			            DuanwuDataBase duanwu = ComponentFactory.CreateWithId<DuanwuDataBase>(IdGenerater.GenerateId());
			            duanwu.UId = userId;
			            string type = GetRandomIndex();
			            duanwu.ActivityType = type;
			            await proxyComponent.Save(duanwu);
			        }
                }

                #region 用户活动所获得的头像数据
                List<OtherData> otherDatas = await proxyComponent.QueryJson<OtherData>($"{{UId:{userId}}}");
                if(otherDatas.Count > 0)
                {
                    response.ownIcon = otherDatas[0].OwnIcon;
                }
                #endregion

                reply(response);
				session.Send(new G2C_TestHotfixMessage() { Info = "recv hotfix message success" });

                // vip上线全服广播
                {
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

	    /// <summary>
	    /// 从12个活动里面随机获取六个活动
	    /// </summary>
	    /// <returns></returns>
	    private string GetRandomIndex()
	    {
	        List<int> randomIndexList = new List<int>();
	        int i = 6;
	        StringBuilder result = new StringBuilder();
	        while (i > 0)
	        {
	            int index = Common_Random.getRandom(0, 11);
	            if (!randomIndexList.Contains(index))
	            {
	                randomIndexList.Add(index);
	                if (randomIndexList.Count == 1)
	                {
	                    result.Append(index);
	                }
	                else
	                {
	                    result.Append(";").Append(index);
	                }
	                --i;
	            }
	        }
	        return result.ToString();
	    }

        private void AddShopInfo()
        {
            

        }
	}
}