﻿using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class DBCommonUtil
    {
        public static async void UpdateTask(long uid,int taskId,bool isGet = false)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskInfo taskInfo = new TaskInfo();
            TaskProgressInfo progress = new TaskProgressInfo();
            List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid},TaskId:{taskId}}}");

            if (taskProgressInfoList.Count > 0)
            {
                for (int i = 0; i < taskProgressInfoList.Count; ++i)
                {
                    progress = taskProgressInfoList[i];
  					++progress.CurProgress;
                    progress.TaskId = taskId;
                    progress = taskProgressInfoList[i];
                    ++progress.CurProgress;
                    progress.TaskId =taskId;                    if (isGet)
                    {
                        progress.IsGet = true;
                    }
                    else
                    {
                        if (progress.CurProgress == progress.Target)
                        {
                            progress.IsComplete = true;
                        }
                        else
                        {
                            progress.IsComplete = false;
                        }
                    }
                    await proxyComponent.Save(progress);
                }
                taskInfo.Id = progress.TaskId;
                taskInfo.IsGet = progress.IsGet;
                taskInfo.IsComplete = progress.IsComplete;
                taskInfo.Progress = progress.CurProgress;
            }
        }

        public static async Task<TaskInfo> UpdateChengjiu(long UId, int taskId, bool isGet = false)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskInfo taskInfo = new TaskInfo();
            ChengjiuInfo progress = new ChengjiuInfo();
            List<ChengjiuInfo> chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{UId},TaskId:{taskId}}}");

            if (chengjiuInfoList.Count > 0)
            {
                for (int i = 0; i < chengjiuInfoList.Count; ++i)
                {
                    progress = chengjiuInfoList[i];
                    progress.TaskId = taskId;
                    if (isGet)
                    {
                        progress.IsGet = true;
                    }
                    else
                    {
                        ++progress.CurProgress;
                        if (progress.CurProgress == progress.Target)
                        {
                            progress.IsComplete = true;
                        }
                        else
                        {
                            progress.IsComplete = false;
                        }
                    }
                    await proxyComponent.Save(progress);
                }
                taskInfo.Id = progress.TaskId;
                taskInfo.IsGet = progress.IsGet;
                taskInfo.IsComplete = progress.IsComplete;
                taskInfo.Progress = progress.CurProgress;
            }
            return taskInfo;

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

        public static async Task changeWealthWithStr(long uid, string reward)
        {
            List<string> list1 = new List<string>();
            CommonUtil.splitStr(reward, list1, ';');

            for (int i = 0; i < list1.Count; i++)
            {
                List<string> list2 = new List<string>();
                CommonUtil.splitStr(list1[i], list2, ':');

                int id = int.Parse(list2[0]);
                float num = float.Parse(list2[1]);

                await ChangeWealth(uid,id, num);
            }

        }

        public static async Task ChangeWealth(long uid, int propId, float propNum)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            switch (propId)
            {
                // 金币
                case 1:
                    {
                        List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
                        playerBaseInfos[0].GoldNum += (int)propNum;
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
                        playerBaseInfos[0].WingNum += (int)propNum;
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
                            itemInfo.Count = (int)propNum;
                            DBHelper.AddItemToDB(itemInfo);
                        }
                        else
                        {
                            userBags[0].Count += (int)propNum;
                            if (userBags[0].Count < 0)
                            {
                                userBags[0].Count = 0;
                            }
                            await proxyComponent.Save(userBags[0]);
                        }
                    }
                    break;
            }
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
                    playerBaseInfos[0].ZhuanPanCount = 3;
                    await proxyComponent.Save(playerBaseInfos[0]);
                }

                // 重置任务
                {

                    List<TaskProgressInfo> progressList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid}}}");
                    for (int j = 1; j < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++j)
                    {
                        int id = 100 + j;
                        if (progressList[0].TaskId == id)
                        {
                            TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                            progressList[0].IsGet = false;
                            progressList[0].Name = config.Name;
                            progressList[0].TaskId = (int)config.Id;
                            progressList[0].IsComplete = false;
                            progressList[0].Target = config.Target;
                            progressList[0].Reward = config.Reward;
                            progressList[0].Desc = config.Desc;
                            progressList[0].CurProgress = 0;
                            break;
                        }
                    }
                    await proxyComponent.Save(progressList[0]);
                }
            }

            Log_Login log_Login = ComponentFactory.CreateWithId<Log_Login>(IdGenerater.GenerateId());
            log_Login.Uid = uid;
            await proxyComponent.Save(log_Login);
        }

        public static async Task<PlayerBaseInfo> addPlayerBaseInfo(long uid, string Phone)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();

            Log.Debug("增加新用户：" + uid.ToString());
            List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            if (playerBaseInfos.Count > 0)
            {
                Log.Debug("异常：此用户uid已存在:" + uid);
                return playerBaseInfos[0];
            }

            PlayerBaseInfo playerBaseInfo = ComponentFactory.CreateWithId<PlayerBaseInfo>(IdGenerater.GenerateId());
            playerBaseInfo.Id = uid;
            playerBaseInfo.Name = uid.ToString();
            playerBaseInfo.GoldNum = 10;
            playerBaseInfo.WingNum = 0;
            playerBaseInfo.Icon = "f_icon1";
            playerBaseInfo.Phone = Phone;
            playerBaseInfo.IsRealName = false;
            playerBaseInfo.TotalGameCount = 0;
            playerBaseInfo.WingNum = 0;
            playerBaseInfo.VipTime = "2018-05-18 00:00:00";
            playerBaseInfo.PlayerSound = Common_Random.getRandom(1, 4);
            playerBaseInfo.RestChangeNameCount = 1;
            await proxyComponent.Save(playerBaseInfo);

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

            return playerBaseInfo;
        }
    } 
}
