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
        public static async void UpdateTask(long uid,int taskId,int progress)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskInfo taskInfo = new TaskInfo();
            List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid},TaskId:{taskId}}}");
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
            if(taskId == 101)
            {
                if (taskProgressInfoList[0].CurProgress < 4)
                {
                    PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(uid);
                    ++playerBaseInfo.ZhuanPanCount;
                    await proxyComponent.Save(playerBaseInfo);
                }
            }
        }

        public static async void UpdateChengjiu(long UId, int taskId, int progress)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<ChengjiuInfo> chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{UId},TaskId:{taskId}}}");
            // Log.Debug("成就：" + JsonHelper.ToJson(chengjiuInfoList));
            Log.Debug("查询数据库数据:" + taskId + chengjiuInfoList.Count);
            if (chengjiuInfoList.Count > 0)
            {
                Log.Debug("更新数据库，玩家ID：" + UId);
                chengjiuInfoList[0].CurProgress += progress;
                Log.Debug("当前成就：" + taskId +"当前进度:" + chengjiuInfoList[0].CurProgress);
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

        public static async void UpdatePlayerInfo(long uid, int maxHua, bool isWin = false)
        {
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                Log.Debug("更新数据库playerBaseInfo" + playerBaseInfos.Count);
                if (playerBaseInfos.Count > 0)
                {
                    if (isWin)
                    {
                        Log.Debug("游戏胜利+1");
                        playerBaseInfos[0].WinGameCount += 1;
                        Log.Debug("玩家：" + playerBaseInfos[0].Id + "胜利次数为" + playerBaseInfos[0].WinGameCount);
                    }
                    Log.Debug("游戏总局数+1");
                    playerBaseInfos[0].TotalGameCount += 1;
                    Log.Debug("玩家：" + playerBaseInfos[0].Id + "总局数为" + playerBaseInfos[0].TotalGameCount);

                    if (playerBaseInfos[0].MaxHua < maxHua)
                        playerBaseInfos[0].MaxHua = maxHua;
                    proxyComponent.Save(playerBaseInfos[0]);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async Task<PlayerBaseInfo> getPlayerBaseInfo(long uid)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            if (playerBaseInfos.Count > 0)
            {
                return playerBaseInfos[0];
            }

            return null;
        }

        public static async Task<AccountInfo> getAccountInfo(long uid)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{_id:{uid}}}");
            if (accountInfos.Count > 0)
            {
                return accountInfos[0];
            }

            return null;
        }

        public static async Task changeWealthWithStr(long uid, string reward,string reason)
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

                await ChangeWealth(uid,id, num, reason);
            }
        }

        public static async Task ChangeWealth(long uid, int propId, int propNum,string reason)
        {
            //Log.Debug("ChangeWealth: uid = " + uid + "  propId = " + propId + "propNum = " + propNum);
            
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            switch (propId)
            {
                // 金币
                case 1:
                    {
                        List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                        playerBaseInfos[0].GoldNum += propNum;
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
                        List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
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
                        List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
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

        public static async Task Log_Login(long uid)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();

            List<Log_Login> log_Logins = await proxyComponent.QueryJson<Log_Login>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{uid}}}");
            if (log_Logins.Count == 0)
            {
                // 今天第一天登录，做一些处理
                Log.Debug("今天第一天登录");

                // 重置转盘次数
                {
                    List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                    playerBaseInfos[0].ZhuanPanCount = 0;
                    if (playerBaseInfos[0].VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                    {
                        playerBaseInfos[0].ZhuanPanCount = 1;
                    }

                    await proxyComponent.Save(playerBaseInfos[0]);
                }

                // 重置任务
                {
                    List<TaskProgressInfo> progressList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid}}}");
                    if (progressList.Count > 0)
                    {
                        for (int i = 0; i < progressList.Count; ++i)
                        {
                            int id = 100 + i + 1;
                            for (int j = 0; j < configCom.GetAll(typeof(TaskConfig)).Length; ++j)
                            {
                                if (progressList[i].TaskId == id)
                                {
                                    TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                                    progressList[i].IsGet = false;
                                    progressList[i].Name = config.Name;
                                    progressList[i].TaskId = (int)config.Id;
                                    progressList[i].IsComplete = false;
                                    progressList[i].Target = config.Target;
                                    progressList[i].Reward = config.Reward;
                                    progressList[i].Desc = config.Desc;
                                    progressList[i].CurProgress = 0;
                                    break;
                                }
                            }
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
                    }
                }
                //重置端午节活动
                //{
                //    List<DuanwuActivityInfo> duanwuActivityList = await proxyComponent.QueryJson<DuanwuActivityInfo>($"{{UId:{uid}}}");
                //    if (duanwuActivityList.Count > 0)
                //    {
                //        for (int i = 0; i < duanwuActivityList.Count; ++i)
                //        {
                //            int id = 100 + i + 1;
                //            for (int j = 0; j < configCom.GetAll(typeof(DuanwuActivityConfig)).Length; ++j)
                //            {
                //                if (duanwuActivityList[i].TaskId == id)
                //                {
                //                    TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                //                    duanwuActivityList[i].IsGet = false;
                //                    duanwuActivityList[i].TaskId = (int)config.Id;
                //                    duanwuActivityList[i].IsComplete = false;
                //                    duanwuActivityList[i].Target = config.Target;
                //                    duanwuActivityList[i].Reward = config.Reward;
                //                    duanwuActivityList[i].Desc = config.Desc;
                //                    duanwuActivityList[i].CurProgress = 0;
                //                    break;
                //                }
                //            }
                //            await proxyComponent.Save(duanwuActivityList[i]);
                //        }
                //    }
                //}
            }

            Log_Login log_Login = ComponentFactory.CreateWithId<Log_Login>(IdGenerater.GenerateId());
                log_Login.Uid = uid;
                await proxyComponent.Save(log_Login);
            }

            public static async Task Log_ChangeWealth(long uid,int propId, int propNum,string reason)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            Log_ChangeWealth log = ComponentFactory.CreateWithId<Log_ChangeWealth>(IdGenerater.GenerateId());
            log.Uid = uid;
            log.PropId = propId;
            log.PropNum = propNum;
            log.Reason = reason;
            await proxyComponent.Save(log);
        }

        public static async Task<PlayerBaseInfo> addPlayerBaseInfo(long uid, string Phone,string name,string head)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
            AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(uid);

            Log.Debug("增加新用户：" + uid.ToString());
            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            if (playerBaseInfos.Count > 0)
            {
                Log.Debug("异常：此用户uid已存在:" + uid);
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
                List<PlayerBaseInfo> playerBaseInfos_temp = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{Name:'{name}'}}");

                // 昵称已经有人用了
                if (playerBaseInfos_temp.Count > 0)
                {
                    playerBaseInfo.Name = (name + uid.ToString().Substring(6));
                }

                playerBaseInfo.Name = name;
            }

            if (string.IsNullOrEmpty(head))
            {
                playerBaseInfo.Icon = "f_icon1";
            }
            else
            {
                playerBaseInfo.Icon = head;
            }

            accountInfo.Phone = Phone;
            playerBaseInfo.PlayerSound = Common_Random.getRandom(1, 4);
            await proxyComponent.Save(playerBaseInfo);
            await proxyComponent.Save(accountInfo);

            Log.Debug("增加新用户完毕");

            // 插入任务数据
            {
                Log.Debug("增加新用户任务");

                for (int i = 1; i < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++i)
                {
                    int id = 100 + i;
                    TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                    TaskProgressInfo progress = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
                    progress.IsGet = false;
                    progress.UId = uid;
                    progress.Name = config.Name;
                    progress.TaskId = (int)config.Id;
                    progress.IsComplete = false;
                    progress.Target = config.Target;
                    progress.Reward = config.Reward;
                    progress.Desc = config.Desc;
                    progress.CurProgress = 0;

                    await proxyComponent.Save(progress);
                }
                Log.Debug("增加新用户任务完毕");
            }
            //插入新用户成就
            {
                Log.Debug("增加新用户成就");
                for (int i = 1; i < configCom.GetAll(typeof(ChengjiuConfig)).Length + 1; ++i)
                {
                    int id = 100 + i;
                    ChengjiuConfig config = (ChengjiuConfig)configCom.Get(typeof(ChengjiuConfig), id);
                    ChengjiuInfo chengjiu = ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
                    chengjiu.IsGet = false;
                    chengjiu.UId = uid;
                    chengjiu.Name = config.Name;
                    chengjiu.TaskId = (int)config.Id;
                    chengjiu.IsComplete = false;
                    chengjiu.Target = config.Target;
                    chengjiu.Reward = config.Reward;
                    chengjiu.Desc = config.Desc;
                    chengjiu.CurProgress = 0;

                    await proxyComponent.Save(chengjiu);
                }
                Log.Debug("增加新用户成就完毕");
            }

            return playerBaseInfo;
        }

        /// <summary>
        /// 记录在线离线时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="isStart"></param>
        /// <param name="userId"></param>
        public static async void RecordGamerTime(DateTime startTime, bool isStart,long userId)
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
        public static async void RecordGamerInfo(long userId, int totalSeconds)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();

            List<GamerInfoDB> gamerInfos = await proxyComponent.QueryJsonCurrentDayByUid<GamerInfoDB>(userId, DateTime.Now);
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

            TreasureConfig treasureConfig = configComponent.Get(typeof(TreasureConfig), ++gamerInfo.DailyTreasureCount) as TreasureConfig;

            if (gamerInfo.DailyOnlineTime > treasureConfig?.TotalTime)
            {
                gamerInfo.DailyOnlineTime = treasureConfig.TotalTime;
            }

            --gamerInfo.DailyTreasureCount;

            await proxyComponent.Save(gamerInfo);

            //记录玩家在线时长
            DBCommonUtil.UpdateChengjiu(userId, 107, totalSeconds);
            DBCommonUtil.UpdateChengjiu(userId, 108, totalSeconds);
            DBCommonUtil.UpdateChengjiu(userId, 109, totalSeconds);
        }

        /// <summary>
        /// 获取在线时长
        /// </summary>
        /// <param name="userId"></param>
        public static async Task<int> GetRestOnlineSeconds(long userId)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();

            List<GamerInfoDB> gamerInfos = await proxyComponent.QueryJsonCurrentDayByUid<GamerInfoDB>(userId, DateTime.Now);
            if (gamerInfos.Count == 0)
            {
                TreasureConfig config = (TreasureConfig) configComponent.Get(typeof(TreasureConfig), 1);
                return config.TotalTime;
            }
            TreasureConfig treasureConfig = configComponent.Get(typeof(TreasureConfig), ++gamerInfos[0].DailyTreasureCount) as TreasureConfig;

            int i = treasureConfig.TotalTime - gamerInfos[0].DailyOnlineTime;
            Log.Debug("TotalTime" + treasureConfig.TotalTime);
            Log.Debug("gamerInfos[0].DailyOnlineTime" + gamerInfos[0].DailyOnlineTime);
            Log.Debug("还剩" + i);

            return i;
        }
    } 
}
