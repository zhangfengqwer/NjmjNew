using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class DBCommonUtil
    {
        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="taskId"></param>
        /// <param name="progress"></param>
        public static async Task UpdateTask(long uid, int taskId, int progress)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskInfo taskInfo = new TaskInfo();
            List<TaskProgressInfo> taskProgressInfoList =
                await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid},TaskId:{taskId}}}");
            if (taskProgressInfoList.Count <= 0)
            {
                TaskProgressInfo info = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
                TaskConfig config = ConfigHelp.Get<TaskConfig>(taskId);
//                TaskConfig config = TaskData.getInstance().GetDataByTaskId(taskId);
                info.IsGet = false;
                info.UId = uid;
                info.Name = config.Name;
                info.TaskId = (int)config.Id;
                info.IsComplete = false;
                info.Target = config.Target;
                info.Reward = config.Reward;
                info.Desc = config.Desc;
                info.CurProgress = 0;

                await proxyComponent.Save(info);
                taskProgressInfoList =
               await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid},TaskId:{taskId}}}");
            }

            if (taskProgressInfoList.Count > 0)
            {
                /*
                 * progress = -1  代表输
                 * progress = 1  代表赢
                 */
                if (taskId == 104)
                {
                    if (!taskProgressInfoList[0].IsComplete)
                    {
                        if (progress == -1)
                        {
                            taskProgressInfoList[0].CurProgress = 0;
                            await proxyComponent.Save(taskProgressInfoList[0]);
                        }
                        else if (progress == 1)
                        {
                            taskProgressInfoList[0].CurProgress += progress;
                            if (taskProgressInfoList[0].CurProgress == taskProgressInfoList[0].Target)
                            {
                                taskProgressInfoList[0].IsComplete = true;
                            }

                            await proxyComponent.Save(taskProgressInfoList[0]);
                        }
                    }
                }
                else
                {
                    taskProgressInfoList[0].CurProgress += progress;
                    if (taskProgressInfoList[0].CurProgress >= taskProgressInfoList[0].Target)
                    {
                        taskProgressInfoList[0].IsComplete = true;
                    }

                    await proxyComponent.Save(taskProgressInfoList[0]);
                }
            }

            // 增加转盘次数
            if (taskId == 101)
            {
                if (taskProgressInfoList[0].CurProgress < 4)
                {
                    PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(uid);
                    ++playerBaseInfo.ZhuanPanCount;
                    await proxyComponent.Save(playerBaseInfo);
                }
            }

//            Log.Debug("UpdateTask111111111111");
        }

        /// <summary>
        /// 更新成就
        /// </summary>
        /// <param name="UId"></param>
        /// <param name="taskId"></param>
        /// <param name="progress"></param>
        public static async Task UpdateChengjiu(long UId, int taskId, int progress)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<ChengjiuInfo> chengjiuInfoList =
                await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{UId},TaskId:{taskId}}}");
//            Log.Debug("UId:" + UId + " " + "更新成就" + taskId);
//            Log.Debug("用户:" + UId + "成就" + "taskId" + JsonHelper.ToJson(chengjiuInfoList));
            if(chengjiuInfoList.Count <= 0)
            {
//                Log.Debug("成就写入数据库");
                ChengjiuInfo info = ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
                ChengjiuConfig config = ConfigHelp.Get<ChengjiuConfig>(taskId);
                info.IsGet = false;
                info.UId = UId; 
                if(config != null)
                {
                    info.Name = config.Name;
                    info.TaskId = (int)config.Id;
                    info.IsComplete = false;
                    info.Target = config.Target;
                    info.Reward = config.Reward;
                    info.Desc = config.Desc;
                    info.CurProgress = 0;
                }
                else
                {
                    Log.Warning("config：" + taskId + "数据为空");
                }
                await proxyComponent.Save(info);

                chengjiuInfoList =
               await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{UId},TaskId:{taskId}}}");
                //Log.Debug("用户:" + UId + "成就" + "taskId" + JsonHelper.ToJson(chengjiuInfoList));
            }
            if (chengjiuInfoList.Count > 0)
            {
                //Log.Debug("增加成就");
                chengjiuInfoList[0].CurProgress += progress;
                if (chengjiuInfoList[0].CurProgress >= chengjiuInfoList[0].Target)
                {
                    chengjiuInfoList[0].IsComplete = true;
                }
                else
                {
                    chengjiuInfoList[0].IsComplete = false;
                }

                await proxyComponent.Save(chengjiuInfoList[0]);
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="maxHua"></param>
        /// <param name="isWin"></param>
        public static async Task UpdatePlayerInfo(long uid, int maxHua, bool isWin = false)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                if (playerBaseInfos.Count > 0)
                {
                    if (isWin)
                    {
                        playerBaseInfos[0].WinGameCount += 1;
                    }

                    playerBaseInfos[0].TotalGameCount += 1;
                    if (playerBaseInfos[0].MaxHua < maxHua)
                    {
                        playerBaseInfos[0].MaxHua = maxHua;
                    }

                    await proxyComponent.Save(playerBaseInfos[0]);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async Task<PlayerBaseInfo> getPlayerBaseInfo(long uid)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                if (playerBaseInfos.Count > 0)
                {
                    return playerBaseInfos[0];
                }

                Log.Warning($"玩家{uid}PlayerBaseInfo为null,为其新增用户进去");

                PlayerBaseInfo playerBaseInfo = await addPlayerBaseInfo(uid, "", "", "");

                return playerBaseInfo;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        public static async Task<AccountInfo> getAccountInfo(long uid)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{_id:{uid}}}");
            if (accountInfos.Count > 0)
            {
                return accountInfos[0];
            }

            Log.Error("getAccountInfo为空：" + uid);

            return null;
        }

        public static async Task changeWealthWithStr(long uid, string reward, string reason)
        {
            Log.Debug("changeWealthWithStr: uid = " + uid + "  reward = " + reward);

            List<string> list1 = new List<string>();
            CommonUtil.splitStr(reward, list1, ';');

            for (int i = 0; i < list1.Count; i++)
            {
                List<string> list2 = new List<string>();
                CommonUtil.splitStr(list1[i], list2, ':');

                int id = int.Parse(list2[0]);
                int num = int.Parse(list2[1]);

                await ChangeWealth(uid, id, num, reason);
            }
        }

        public static async Task ChangeWealth(long uid, int propId, int propNum, string reason,Room room = null)
        {
            //Log.Debug("ChangeWealth: uid = " + uid + "  propId = " + propId + "propNum = " + propNum);

            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                switch (propId)
                {
                    // 金币
                    case 1:
                        {
                            List<PlayerBaseInfo> playerBaseInfos =
                                await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                            playerBaseInfos[0].GoldNum += propNum;
                            if (playerBaseInfos[0].GoldNum < 0)
                            {
                                playerBaseInfos[0].GoldNum = 0;
                            }

                            bool isSendReliefGold = false;

                            // 救济金
                            if (playerBaseInfos[0].GoldNum < 2000)
                            {
                                List<Log_ReliefGold> log_reliefGolds = await proxyComponent.QueryJson<Log_ReliefGold>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{uid}}}");
                                if (log_reliefGolds.Count < 3)
                                {
                                    isSendReliefGold = true;

                                    int gold = 20000;
                                    playerBaseInfos[0].GoldNum += gold;
                                    Log_ReliefGold log_ReliefGold = ComponentFactory.CreateWithId<Log_ReliefGold>(IdGenerater.GenerateId());
                                    log_ReliefGold.Uid = uid;
                                    log_ReliefGold.reward = "1:" + gold;
                                    await proxyComponent.Save(log_ReliefGold);

                                    // 通知玩家
                                    {
                                        Actor_ReliefGold actor = new Actor_ReliefGold();
                                        if (log_reliefGolds.Count == 0)
                                        {
                                            actor.Reward = "今日第一次赠送金币："+ gold;
                                        }
                                        else if (log_reliefGolds.Count == 1)
                                        {
                                            actor.Reward = "今日第二次赠送金币：" + gold;
                                        }
                                        else if (log_reliefGolds.Count == 2)
                                        {
                                            actor.Reward = "今日最后一次赠送金币：" + gold;
                                        }
                                        if(room != null)
                                        {
                                            Gamer gamer =  room.Get(uid);
                                            room.GamerBroadcast(gamer, actor);
                                        }
                                    }
                                }
                            }

                            await proxyComponent.Save(playerBaseInfos[0]);

                            if (!isSendReliefGold)
                            {
                                await RecordWeekRankLog(uid, propNum, 0);
                            }
                        }
                        break;

                    // 元宝
                    case 2:
                        {
                            List<PlayerBaseInfo> playerBaseInfos =
                                await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                            playerBaseInfos[0].WingNum += propNum;
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
                            List<PlayerBaseInfo> playerBaseInfos =
                                await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                            playerBaseInfos[0].HuaFeiNum += propNum;
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
                            List<UserBag> userBags = await proxyComponent.QueryJson<UserBag>($"{{UId:{uid},BagId:{propId}}}");
                            if (userBags.Count == 0)
                            {
                                UserBag itemInfo = new UserBag();
                                itemInfo.BagId = propId;
                                itemInfo.UId = uid;
                                itemInfo.Count = propNum;
                                DBHelper.AddItemToDB(itemInfo);
                            }
                            else
                            {
                                userBags[0].Count += propNum;
                                if (userBags[0].Count < 0)
                                {
                                    userBags[0].Count = 0;
                                }

                                await proxyComponent.Save(userBags[0]);
                            }
                        }
                        break;
                }

                await Log_ChangeWealth(uid, propId, propNum, reason);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static async Task Log_Login(long uid, Session session,string clientVersion)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();

            List<Log_Login> log_Logins =
                await proxyComponent.QueryJson<Log_Login>(
                    $"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{uid}}}");
            if (log_Logins.Count == 0)
            {
                // 今天第一天登录，做一些处理
                Log.Debug("今天第一天登录");


                // 重置转盘次数
                {
                    List<PlayerBaseInfo> playerBaseInfos =
                        await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");

                    if (playerBaseInfos.Count > 0)
                    {
                        playerBaseInfos[0].ZhuanPanCount = 0;
                        if (playerBaseInfos[0].VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            playerBaseInfos[0].ZhuanPanCount = 1;
                        }

                        //重置赠送好友房钥匙
                        playerBaseInfos[0].IsGiveFriendKey = false;

                        await proxyComponent.Save(playerBaseInfos[0]);
                    }

                    else
                    {
                        Log.Warning($"玩家{uid}的PlayerBaseInfo为null");
                    }
                }

                // 重置任务
                {
                    List<TaskProgressInfo> progressList =
                        await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid}}}");
                    if (progressList.Count > 0)
                    {
                        for (int i = 0; i < progressList.Count; ++i)
                        {
                            progressList[i].IsGet = false;
                            progressList[i].IsComplete = false;
                            progressList[i].CurProgress = 0;
                            await proxyComponent.Save(progressList[i]);
                        }
                    }
                }

                //重置每天在线时长和宝箱次数
                {
                    List<GamerInfoDB> gamerInfo = await proxyComponent.QueryJson<GamerInfoDB>($"{{UId:{uid}}}");
                    if (gamerInfo.Count > 0)
                    {
                        gamerInfo[0].DailyOnlineTime = 0;
                        gamerInfo[0].DailyTreasureCount = 0;
                        await proxyComponent.Save(gamerInfo[0]);
                    }
                }
            }

            Log_Login log_Login = ComponentFactory.CreateWithId<Log_Login>(IdGenerater.GenerateId());
            log_Login.Uid = uid;
            log_Login.ip = session.RemoteAddress.ToString();
            log_Login.clientVersion = clientVersion;
            await proxyComponent.Save(log_Login);
        }

        // 游戏日志
        public static async Task Log_Game(string RoomName, long Player1_uid, long Player2_uid, long Player3_uid,
            long Player4_uid, long winner_uid)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            Log_Game log = ComponentFactory.CreateWithId<Log_Game>(IdGenerater.GenerateId());
            log.RoomName = RoomName;
            log.Player1_uid = Player1_uid;
            log.Player2_uid = Player2_uid;
            log.Player3_uid = Player3_uid;
            log.Player4_uid = Player4_uid;
            log.Winner_uid = winner_uid;
            await proxyComponent.Save(log);
        }

        public static async Task Log_ChangeWealth(long uid, int propId, int propNum, string reason)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            Log_ChangeWealth log = ComponentFactory.CreateWithId<Log_ChangeWealth>(IdGenerater.GenerateId());
            log.Uid = uid;
            log.PropId = propId;
            log.PropNum = propNum;
            log.Reason = reason;
            await proxyComponent.Save(log);
        }

        public static async Task<PlayerBaseInfo> addPlayerBaseInfo(long uid, string Phone, string name, string head)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
            AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(uid);

            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            if (playerBaseInfos.Count > 0)
            {
                return playerBaseInfos[0];
            }

            PlayerBaseInfo playerBaseInfo = ComponentFactory.CreateWithId<PlayerBaseInfo>(IdGenerater.GenerateId());
            playerBaseInfo.Id = uid;

            if (string.IsNullOrEmpty(name))
            {
                playerBaseInfo.Name = uid.ToString();
            }
            else
            {
                List<PlayerBaseInfo> playerBaseInfos_temp =
                    await proxyComponent.QueryJson<PlayerBaseInfo>($"{{Name:'{name}'}}");

                // 昵称已经有人用了
                if (playerBaseInfos_temp.Count > 0)
                {
                    playerBaseInfo.Name = (name + uid.ToString().Substring(6));
                }

                playerBaseInfo.Name = name;
            }

            if (string.IsNullOrEmpty(head))
            {
                int random = Common_Random.getRandom(1, 5);
                //百分之五十的概率随机生成男生或女生头像
                int rate = Common_Random.getRandom(1, 100);
                if(rate <= 50)
                {
                    playerBaseInfo.Icon = $"f_icon{random}";
                }
                else
                {
                    playerBaseInfo.Icon = $"m_icon{random}";
                }
            }
            else
            {
                playerBaseInfo.Icon = head;
            }

            if (accountInfo != null)
            {
                accountInfo.Phone = Phone;
            }
            else
            {
                Log.Error("addPlayerBaseInfo() accountInfo==null  uid = " + uid);
            }

            playerBaseInfo.PlayerSound = Common_Random.getRandom(1, 4);
            await proxyComponent.Save(playerBaseInfo);
            await proxyComponent.Save(accountInfo);

            Log.Debug("增加新用户完毕");

            //// 插入任务数据
            //{
            //    Log.Debug("增加新用户任务");

            //    for (int i = 1; i < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++i)
            //    {
            //        int id = 100 + i;
            //        TaskConfig config = (TaskConfig) configCom.Get(typeof(TaskConfig), id);
            //        TaskProgressInfo progress =
            //            ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
            //        progress.IsGet = false;
            //        progress.UId = uid;
            //        progress.Name = config.Name;
            //        progress.TaskId = (int) config.Id;
            //        progress.IsComplete = false;
            //        progress.Target = config.Target;
            //        progress.Reward = config.Reward;
            //        progress.Desc = config.Desc;
            //        progress.CurProgress = 0;

            //        await proxyComponent.Save(progress);
            //    }

            //    Log.Debug("增加新用户任务完毕");
            //}
            //插入新用户成就
            //{
            //    Log.Debug("增加新用户成就");
            //    for (int i = 1; i < configCom.GetAll(typeof(ChengjiuConfig)).Length + 1; ++i)
            //    {
            //        int id = 100 + i;
            //        ChengjiuConfig config = (ChengjiuConfig) configCom.Get(typeof(ChengjiuConfig), id);
            //        ChengjiuInfo chengjiu = ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
            //        chengjiu.IsGet = false;
            //        chengjiu.UId = uid;
            //        chengjiu.Name = config.Name;
            //        chengjiu.TaskId = (int) config.Id;
            //        chengjiu.IsComplete = false;
            //        chengjiu.Target = config.Target;
            //        chengjiu.Reward = config.Reward;
            //        chengjiu.Desc = config.Desc;
            //        chengjiu.CurProgress = 0;

            //        await proxyComponent.Save(chengjiu);
            //    }

            //    Log.Debug("增加新用户成就完毕");
            //}

            return playerBaseInfo;
        }

        /// <summary>
        /// 记录在线离线时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="isStart"></param>
        /// <param name="userId"></param>
        public static async Task RecordGamerTime(DateTime startTime, bool isStart, long userId)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

            GamerOnlineTime gamerOnlineTime = ComponentFactory.CreateWithId<GamerOnlineTime>(IdGenerater.GenerateId());
            if (isStart)
            {
                gamerOnlineTime.Type = 0;
            }
            else
            {
                gamerOnlineTime.Type = 1;
            }

            gamerOnlineTime.CreateTime = startTime.GetCurrentTime();
            gamerOnlineTime.UId = userId;
            await proxyComponent.Save(gamerOnlineTime);
        }

        /// <summary>
        /// 记录玩家数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
        public static async Task RecordGamerInfo(long userId, int totalSeconds)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();

            List<GamerInfoDB> gamerInfos = await proxyComponent.QueryJson<GamerInfoDB>($"{{UId:{userId}}}");
            GamerInfoDB gamerInfo;
            if (gamerInfos.Count == 0)
            {
                gamerInfo = ComponentFactory.CreateWithId<GamerInfoDB>(IdGenerater.GenerateId());
            }
            else
            {
                gamerInfo = gamerInfos[0];
            }

            gamerInfo.UId = userId;
            gamerInfo.DailyOnlineTime += totalSeconds;
            gamerInfo.TotalOnlineTime += totalSeconds;

            TreasureConfig treasureConfig =
                configComponent.Get(typeof(TreasureConfig), ++gamerInfo.DailyTreasureCount) as TreasureConfig;

            Log.Debug(gamerInfo.DailyTreasureCount + "");
            Log.Debug(JsonHelper.ToJson(treasureConfig));


            if (gamerInfo.DailyOnlineTime > treasureConfig?.TotalTime)
            {
                gamerInfo.DailyOnlineTime = treasureConfig.TotalTime;
            }

            --gamerInfo.DailyTreasureCount;

            await proxyComponent.Save(gamerInfo);

            //记录玩家在线时长
            await DBCommonUtil.UpdateChengjiu(userId, 107, totalSeconds);
            await DBCommonUtil.UpdateChengjiu(userId, 108, totalSeconds);
            await DBCommonUtil.UpdateChengjiu(userId, 109, totalSeconds);
        }

        /// <summary>
        /// 获取在线时长
        /// </summary>
        /// <param name="userId"></param>
        public static async Task<int> GetRestOnlineSeconds(long userId)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();

            List<GamerInfoDB> gamerInfos = await proxyComponent.QueryJson<GamerInfoDB>($"{{UId:{userId}}}");
            if (gamerInfos.Count == 0)
            {
                TreasureConfig config = (TreasureConfig) configComponent.Get(typeof(TreasureConfig), 1);
                return config.TotalTime;
            }
            else
            {
                for (int j = 1; j < gamerInfos.Count; j++)
                {
                    await proxyComponent.Delete<GamerInfoDB>(gamerInfos[j].Id);
                }
            }

            TreasureConfig treasureConfig =
                configComponent.Get(typeof(TreasureConfig), ++gamerInfos[0].DailyTreasureCount) as TreasureConfig;

            //当天的宝箱已领取完
            if (treasureConfig == null)
            {
                return -1;
            }

            int i = treasureConfig.TotalTime - gamerInfos[0].DailyOnlineTime;
            Log.Debug("TotalTime" + treasureConfig.TotalTime);
            Log.Debug("gamerInfos[0].DailyOnlineTime" + gamerInfos[0].DailyOnlineTime);
            Log.Debug("还剩" + i);

            return i;
        }

        /// <summary>
        /// 发货接口
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="goodsId"></param>
        /// <param name="goodsNum"></param>
        /// <param name="price"></param>
        public static async Task UserRecharge(int orderId, long userId, int goodsId, int goodsNum, float price)
        {
            try
            {
                string reward = "";
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                ShopConfig config = ShopData.getInstance().GetDataByShopId(goodsId);

                List<Log_Recharge> log_Recharge = await proxyComponent.QueryJson<Log_Recharge>($"{{Uid:{userId}}}");

                if (log_Recharge.Count == 0)
                {
                    reward = "1:120000;105:20;104:1;107:1";
                    await DBCommonUtil.changeWealthWithStr(userId, reward, "首充奖励");
                }

                reward = config.Items;

                await DBCommonUtil.changeWealthWithStr(userId, reward, "购买元宝");

                UserComponent userComponent = Game.Scene.GetComponent<UserComponent>();
                User user = userComponent.Get(userId);
                //给玩家发送消息
                user?.session?.Send(new Actor_GamerBuyYuanBao()
                {
                    goodsId = goodsId
                });

                // 记录日志
                {
                    Log_Recharge log_recharge = ComponentFactory.CreateWithId<Log_Recharge>(IdGenerater.GenerateId());
                    log_recharge.Uid = userId;
                    log_recharge.GoodsId = config.Id;
                    log_recharge.Price = config.Price;
                    log_recharge.OrderId = orderId;
                    await proxyComponent.Save(log_recharge);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        // 检查是否在黑名单中
        public static async Task<bool> CheckIsInBlackList(long uid,Session session)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

            List<string> list = new List<string>();
            CommonUtil.splitStr(session.RemoteAddress.ToString(), list,':');
            if (list.Count > 0)
            {
                List<BlackList> blackLists = await proxyComponent.QueryJson<BlackList>($"{{ip:'{list[0]}'}}");
                if (blackLists.Count > 0)
                {
                    if (blackLists[0].EndTime.CompareTo(CommonUtil.getCurDataNormalFormat()) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 发送邮件
        public static async Task SendMail(long uid, string EmailTitle, string Content, string RewardItem)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                EmailInfo emailInfo = ComponentFactory.CreateWithId<EmailInfo>(IdGenerater.GenerateId());
                emailInfo.UId = uid;

                /*
                 * ************************************************
                 **************************************************
                 **************************************************
                 * 注意：按照10万人，每人只有2000条邮件的额度
                 * ************************************************
                 * ************************************************
                 * *************************************************
                */
                {
                    long curAllCount = proxyComponent.QueryJsonCount<EmailInfo>("{}");
                    emailInfo.EmailId = (int) ++curAllCount;
                }

                emailInfo.EmailTitle = EmailTitle;
                emailInfo.Content = Content;
                emailInfo.RewardItem = RewardItem;

                await proxyComponent.Save(emailInfo);
            }
            catch (Exception e)
            {
                Log.Error("SendMail异常:" + e);
            }
        }

        // 获取玩家好友房钥匙数量
        public static async Task<int> GetUserFriendKeyNum(long uid)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<FriendKey> listData = await proxyComponent.QueryJsonDB<FriendKey>($"{{Uid:{uid}}}");

                int count = 0;
                
                for (int i = 0; i < listData.Count; i++)
                {
                    if (listData[i].endTime.CompareTo("-1") == 0)
                    {
                        ++count;
                    }
                    else
                    {
                        string date = listData[i].endTime.Substring(0, (listData[i].endTime.LastIndexOf(':') - 5));

                        if ((date.CompareTo(CommonUtil.getCurDataNormalFormat()) >= 0)
                            && String.CompareOrdinal(listData[i].endTime, CommonUtil.getCurTimeNormalFormat()) < 0)
                        {
                            ++count;
                        }
                        else
                        {
                            await proxyComponent.Delete<FriendKey>(listData[i].Id);
                        }
                    }
                }

                return count;
            }
            catch (Exception e)
            {
                Log.Error("GetUserFriendKeyNum异常:" + e);

                return 0;
            }
        }

        // 给玩家发送好友房钥匙
        // endTime为“-1”代表永久有效
        public static async Task AddFriendKey(long uid, int num,string endTime,string reason)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

                for(int i = 0; i < num; i++)
                {
                    FriendKey friendKey = ComponentFactory.CreateWithId<FriendKey>(IdGenerater.GenerateId());
                    friendKey.Uid = uid;
                    friendKey.endTime = endTime;

                    await proxyComponent.Save(friendKey);
                }

                await Log_ChangeWealth(uid, 112, num, reason);

                //Log.Info("修改完后玩家：" + uid + "钥匙数量为：" + await GetUserFriendKeyNum(uid));
            }
            catch (Exception e)
            {
                Log.Error("AddFriendKey异常:" + e);
            }
        }

        // 扣除玩家好友房钥匙
        public static async Task DeleteFriendKey(long uid, int num, string reason)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<FriendKey> listData = await proxyComponent.QueryJsonDB<FriendKey>($"{{Uid:{uid}}}");

                int count = 0;

                // 先删非永久的并且在有效期以内的
                for (int i = 0; i < listData.Count; i++)
                {
                    if (count < num)
                    {
                        if (listData[i].endTime.CompareTo("-1") != 0)
                        {
                            if (listData[i].endTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                            {
                                ++count;
                                await proxyComponent.Delete<FriendKey>(listData[i].Id);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // 后删永久的
                for (int i = 0; i < listData.Count; i++)
                {
                    if (count < num)
                    {
                        if (listData[i].endTime.CompareTo("-1") == 0)
                        {
                            ++count;
                            await proxyComponent.Delete<FriendKey>(listData[i].Id);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                await Log_ChangeWealth(uid, 112, -num, reason);

                //Log.Info("修改完后玩家：" + uid + "钥匙数量为：" + await GetUserFriendKeyNum(uid));
            }
            catch (Exception e)
            {
                Log.Error("DeleteFriendKey异常:" + e);
            }
        }

        public static void AccountWeekData()
        {
            //结算是否上榜
            Game.Scene.GetComponent<RankDataComponent>().SetFRankData();
        }

        //记录一周获胜记录
        //如果只变化财富或胜场数 对应的另外一个输入时为0
        public static async Task RecordWeekRankLog(long uid, long wealth, int count)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_Rank> logs = await proxyComponent.QueryJson<Log_Rank>($"{{UId:{uid}}}");
                if (logs.Count <= 0)
                {
                    Log_Rank info = ComponentFactory.CreateWithId<Log_Rank>(IdGenerater.GenerateId());
                    info.UId = uid;
                    info.WinGameCount += count;
                    info.Wealth += wealth;
                    await proxyComponent.Save(info);
                }
                else
                {
                    logs[0].WinGameCount += count;
                    logs[0].Wealth += wealth;
                    await proxyComponent.Save(logs[0]);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}