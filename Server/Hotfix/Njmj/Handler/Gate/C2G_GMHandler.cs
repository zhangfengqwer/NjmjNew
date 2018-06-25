using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GMHandler : AMRpcHandler<C2G_GM, G2C_GM>
    {
        //1,刷新所有配置表 2,发送邮件 3,解散房间 4,增减黑名单 5,生成报表 6,查看用户信息 7,强制离线 8,修改用户信息 9,查看游戏内信息
        protected override async void Run(Session session, C2G_GM message, Action<G2C_GM> reply)
        {
            G2C_GM response = new G2C_GM();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                if(Game.Scene.GetComponent<ConfigComponent>() != null)
                {
                    Game.Scene.RemoveComponent<ConfigComponent>();
                }
                Game.Scene.AddComponent<ConfigComponent>();
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                
                switch (message.Type)
                {
                    case 1:
                        {
                            //刷新所有配置表
                            List<PlayerBaseInfo> allInfos = await proxyComponent.QueryJsonDBInfos<PlayerBaseInfo>();
                            {
                                //商城
                                List<ShopConfig> shopList = new List<ShopConfig>();
                                for (int i = 1; i < configCom.GetAll(typeof(ShopConfig)).Length + 1; ++i)
                                {
                                    int id = 1000 + i;
                                    ShopConfig config = (ShopConfig)configCom.Get(typeof(ShopConfig), id);
                                    shopList.Add(config);
                                }
                                ShopData.getInstance().getDataList().Clear();
                                ShopData.getInstance().getDataList().AddRange(shopList);
                            }
                            {
                                //任务
                                List<TaskConfig> taskList = new List<TaskConfig>();
                                for (int i = 1; i < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++i)
                                {
                                    int id = 100 + i;
                                    TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                                    taskList.Add(config);
                                }
                                TaskData.getInstance().getDataList().Clear();
                                TaskData.getInstance().getDataList().AddRange(taskList);
                                for (int i = 0; i < allInfos.Count; ++i)
                                {
                                    List<TaskProgressInfo> taskInfos = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{allInfos[i].Id}}}");
                                    //无论删减任务 都要刷新一下任务的数据库
                                    for (int j = 0; j < taskInfos.Count; ++j)
                                    {
                                        TaskConfig config = TaskData.getInstance().GetDataByTaskId(taskInfos[j].TaskId);
                                        if(config != null)
                                        {
                                            taskInfos[j].Name = config.Name;
                                            taskInfos[j].Target = config.Target;
                                            taskInfos[j].Reward = config.Reward;
                                            taskInfos[j].Desc = config.Desc;
                                            await proxyComponent.Save(taskInfos[j]);
                                        }
                                    }

                                    #region 增加任务先不管 update时会重新添加
                                    //if (TaskData.getInstance().getDataList().Count > taskInfos.Count)
                                    //{
                                    //    //增加了新任务
                                    //    for (int j = 0; j < TaskData.getInstance().getDataList().Count; ++j)
                                    //    {
                                    //        List<TaskProgressInfo> progresses = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{allInfos[i].Id},TaskId:{TaskData.getInstance().getDataList()[j].Id}}}");
                                    //        if (progresses.Count <= 0)
                                    //        {
                                    //            TaskProgressInfo progress =
                                    //          ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
                                    //            progress.IsGet = false;
                                    //            progress.UId = allInfos[i].Id;
                                    //            progress.Name = TaskData.getInstance().getDataList()[j].Name;
                                    //            progress.TaskId = (int)TaskData.getInstance().getDataList()[j].Id;
                                    //            progress.IsComplete = false;
                                    //            progress.Target = TaskData.getInstance().getDataList()[j].Target;
                                    //            progress.Reward = TaskData.getInstance().getDataList()[j].Reward;
                                    //            progress.Desc = TaskData.getInstance().getDataList()[j].Desc;
                                    //            progress.CurProgress = 0;

                                    //            await proxyComponent.Save(progress);
                                    //        }
                                    //    }
                                    //}
                                    #endregion

                                    if (TaskData.getInstance().getDataList().Count < taskInfos.Count)
                                    {
                                        for (int j = 0; j < taskInfos.Count; ++j)
                                        {
                                            TaskConfig config = TaskData.getInstance().GetDataByTaskId(taskInfos[j].TaskId);
                                            List<TaskProgressInfo> progresses = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{allInfos[i].Id},TaskId:{taskInfos[j].TaskId}}}");
                                            if (config == null)
                                            {
                                                if (progresses.Count > 0)
                                                {
                                                    //删除该任务
                                                    await proxyComponent.Delete<TaskProgressInfo>(progresses[0].Id);
                                                }
                                            }
                                        }
                                    }
                                }
                                {
                                    //成就
                                    List<ChengjiuConfig> chengjiuList = new List<ChengjiuConfig>();
                                    for (int i = 1; i < configCom.GetAll(typeof(ChengjiuConfig)).Length + 1; ++i)
                                    {
                                        int id = 100 + i;
                                        ChengjiuConfig config = (ChengjiuConfig)configCom.Get(typeof(ChengjiuConfig), id);
                                        chengjiuList.Add(config);
                                    }
                                    ChengjiuData.getInstance().getDataList().Clear();
                                    ChengjiuData.getInstance().getDataList().AddRange(chengjiuList);
                                    for (int i = 0; i < allInfos.Count; ++i)
                                    {
                                        List<ChengjiuInfo> chengjiuInfos = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{allInfos[i].Id}}}");
                                        //无论删减任务 都要刷新一下任务的数据库
                                        for (int j = 0; j < chengjiuInfos.Count; ++j)
                                        {
                                            ChengjiuConfig config = ChengjiuData.getInstance().GetDataByChengjiuId(chengjiuInfos[j].TaskId);
                                            if(config != null)
                                            {
                                                chengjiuInfos[j].Name = config.Name;
                                                chengjiuInfos[j].Target = config.Target;
                                                chengjiuInfos[j].Reward = config.Reward;
                                                chengjiuInfos[j].Desc = config.Desc;
                                                await proxyComponent.Save(chengjiuInfos[j]);
                                            }
                                        }

                                        #region 增加（暂时先不管 Update时会新加）
                                        //if (ChengjiuData.getInstance().getDataList().Count > chengjiuInfos.Count)
                                        //{
                                        //    //增加了新成就
                                        //    for (int j = chengjiuInfos.Count; j < ChengjiuData.getInstance().getDataList().Count; ++j)
                                        //    {
                                        //        List<ChengjiuInfo> progresses = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{allInfos[i].Id},TaskId:{ChengjiuData.getInstance().getDataList()[j].Id}}}");
                                        //        if (progresses.Count <= 0)
                                        //        {
                                        //            Log.Debug("增加新成就");
                                        //            ChengjiuInfo progress =
                                        //          ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
                                        //            progress.IsGet = false;
                                        //            progress.UId = allInfos[i].Id;
                                        //            progress.Name = ChengjiuData.getInstance().getDataList()[j].Name;
                                        //            progress.TaskId = (int)ChengjiuData.getInstance().getDataList()[j].Id;
                                        //            progress.IsComplete = false;
                                        //            progress.Target = ChengjiuData.getInstance().getDataList()[j].Target;
                                        //            progress.Reward = ChengjiuData.getInstance().getDataList()[j].Reward;
                                        //            progress.Desc = ChengjiuData.getInstance().getDataList()[j].Desc;
                                        //            progress.CurProgress = 0;

                                        //            await proxyComponent.Save(progress);
                                        //        }
                                        //    }
                                        //}
                                        #endregion

                                        if (ChengjiuData.getInstance().getDataList().Count < chengjiuInfos.Count)
                                        {
                                            for (int j = 0; j < chengjiuInfos.Count; ++j)
                                            {
                                                ChengjiuConfig config = ChengjiuData.getInstance().GetDataByChengjiuId(chengjiuInfos[j].TaskId);
                                                List<ChengjiuInfo> progresses = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{allInfos[i].Id},TaskId:{chengjiuInfos[j].TaskId}}}");
                                                if (config == null)
                                                {
                                                    if (progresses.Count > 0)
                                                    {
                                                        //删除该成就
                                                        await proxyComponent.Delete<ChengjiuInfo>(progresses[0].Id);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            //发送邮件 uid为空则为给全部玩家发送邮件
                            if(message.UId != 0)
                            {
                                Log.Debug("Mail" + message.UId + message.Title + message.Content + message.Reward);
                                await DBCommonUtil.SendMail(message.UId, message.Title, message.Content, message.Reward);
                            }
                            else
                            {
                                //分批发还是全部一次性发
                                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJsonDBInfos<PlayerBaseInfo>();
                                if(playerBaseInfos.Count > 0)
                                {
                                    for(int i = 0;i< playerBaseInfos.Count; ++i)
                                    {
                                        await DBCommonUtil.SendMail(playerBaseInfos[i].Id, message.Title, message.Content, message.Reward);

                                    }
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            //解散房间
                        }
                        break;
                    case 4:
                        {
                            //增减黑名单（根据玩家IP）
                            List<BlackList> blackList = await proxyComponent.QueryJson<BlackList>($"{{ip:'{message.IP}'}}");
                            if (string.IsNullOrEmpty(message.EndTime))
                            {
                                //删除黑名单
                                if(blackList.Count > 0)
                                {
                                    blackList[0].EndTime = "2000-01-01 00:00:00";
                                    await proxyComponent.Save(blackList[0]);
                                }
                            }
                            else
                            {
                                if(blackList.Count > 0)
                                {
                                    blackList[0].EndTime = message.EndTime;
                                    await proxyComponent.Save(blackList[0]);
                                }   
                                else
                                {
                                    BlackList list = ComponentFactory.CreateWithId<BlackList>(IdGenerater.GenerateId());
                                    list.Uid = message.UId;
                                    list.ip = message.IP;
                                    list.EndTime = message.EndTime;
                                    list.Reason = message.Reason;
                                    await proxyComponent.Save(list);
                                }
                            }
                        }
                        break;
                    case 5:
                        {
                            //生成报表
                            string logData = await DataStatistics.Start(message.CreateBaobiaoTime);
                            response.LogData = logData;
                        }
                        break;
                    case 6:
                        //查看用户信息
                        List<PlayerBaseInfo> infos = new List<PlayerBaseInfo>();
                        AccountInfo accountInfo = null;
                        List<Log_Login> logLogins = new List<Log_Login>();
                        if (message.UId != 0)
                        {
                            infos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");
                            accountInfo = await DBCommonUtil.getAccountInfo(message.UId);
                            logLogins = await proxyComponent.QueryJson<Log_Login>($"{{Uid:{message.UId}}}");
                        }
                        else if (!string.IsNullOrEmpty(message.Name))
                        {
                            infos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{Name:'{message.Name}'}}");
                            if (infos.Count > 0)
                            {
                                accountInfo = await DBCommonUtil.getAccountInfo(infos[0].Id);
                                logLogins = await proxyComponent.QueryJson<Log_Login>($"{{Uid:{infos[0].Id}}}");
                            }
                        }

                        if(infos.Count > 0)
                        {
                            PlayerInfo info = new PlayerInfo();
                            info.Name = infos[0].Name;
                            info.GoldNum = infos[0].GoldNum;
                            info.WingNum = infos[0].WingNum;
                            info.HuaFeiNum = infos[0].HuaFeiNum;
                            info.IsRealName = infos[0].IsRealName;
                            info.VipTime = infos[0].VipTime;
                            info.EmogiTime = infos[0].EmogiTime;
                            info.MaxHua = infos[0].MaxHua;
                            info.TotalGameCount = infos[0].TotalGameCount;
                            info.WinGameCount = infos[0].WinGameCount;
                            response.UId = infos[0].Id;
                            if(logLogins.Count > 0)
                            {
                                response.LastOnlineTime = logLogins[logLogins.Count - 1].CreateTime;
                            }
                            if(accountInfo != null)
                            {
                                info.Phone = accountInfo.Phone;
                                response.RegisterTime = accountInfo.CreateTime;
                                response.Channel = accountInfo.ChannelName;
                            }
                            response.Info = info;
                            response.Ip = logLogins[logLogins.Count - 1].ip;
                        }
                        else
                        {
                            response.Message = "不存在该用户信息";
                            response.Error = ErrorCode.ERR_Exception;
                            response.Info = null;
                        }
                        break;
                    case 7:
                        {
                            //强制玩家离线
                            UserComponentSystem.ForceOffline(message.UId,message.Reason);
                        }
                        break;
                    case 8:
                        {
                            //更改用户信息
                            List<PlayerBaseInfo> playerInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");
                            if (playerInfos.Count > 0)
                            {
                                if (!message.Icon.Equals("0"))
                                {
                                    playerInfos[0].Icon = message.Icon;
                                    await proxyComponent.Save(playerInfos[0]);
                                }
                                if (message.RestChangeNameCount != 0)
                                {
                                    playerInfos[0].RestChangeNameCount = message.RestChangeNameCount;
                                    await proxyComponent.Save(playerInfos[0]);
                                }
                                if (!message.Prop.Equals("0"))
                                {
                                    await DBCommonUtil.changeWealthWithStr(message.UId,message.Prop,"GM中增加玩家道具");
                                }
                            }
                            else
                            {
                                response.Error = ErrorCode.ERR_Exception;
                                response.Message = "用户不存在，请检查UID是否正确";
                            }
                        }
                        break;
                    case 9:
                        {
                            //查看游戏内信息

                            StartConfigComponent _config = Game.Scene.GetComponent<StartConfigComponent>();
                            IPEndPoint mapIPEndPoint = _config.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                            Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapIPEndPoint);

                            M2G_GetRoomInfo getRoomInfo = (M2G_GetRoomInfo)await mapSession.Call(new G2M_GetRoomInfo());
                            response.Room = new RoomInfo();
                            response.Room.NewRoomCount = getRoomInfo.NewRoomCount;
                            response.Room.NewTotalPlayerInGameCount = getRoomInfo.NewTotalPlayerInGameCount;
                            response.Room.JingRoomCount = getRoomInfo.JingRoomCount;
                            response.Room.JingTotalPlayerInGameCount = getRoomInfo.JingTotalPlayerInGameCount;
                        }
                        break;

                    case 10:
                        {
                            // 发送通知
                            UserComponentSystem.EmergencyNotice(message.UId, message.Content);
                        }
                        break;
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
