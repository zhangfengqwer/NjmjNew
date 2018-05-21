
using ETModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETHotfix
{
    public class DBHelper
    {
        /// <summary>
        /// 添加DB信息
        /// </summary>
        public static async void AddEmailInfoToDB(EmailInfo info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            EmailInfo emailInfo = ComponentFactory.CreateWithId<EmailInfo>(IdGenerater.GenerateId());
            emailInfo = info;
            await proxyComponent.Save(emailInfo);
        }

        public static async void AddTaskProgressInfoToDB(long uId, TaskProgressInfo info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskProgressInfo taskInfo = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
            taskInfo = info;
            await proxyComponent.Save(taskInfo);
        }

        public static async void RefreshDB()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<TaskProgressInfo> allTaskInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{}}");
            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
            for(int i = 0;i< allTaskInfoList.Count; ++i)
            {
                TaskProgressInfo progress = allTaskInfoList[i];
                for (int j = 1; j < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++j)
                {
                    int id = 100 + j;
                    if (progress.TaskId == id)
                    {
                        TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                        progress.IsGet = false;
                        progress.Name = config.Name;
                        progress.TaskId = (int)config.Id;
                        progress.IsComplete = false;
                        progress.Target = config.Target;
                        progress.Reward = config.Reward;
                        progress.Desc = config.Desc;
                        progress.CurProgress = 0;
                        DBHelper.AddTaskProgressInfoToDB(progress.UId, progress);
                        break;
                    }
                }
            }
        }

        static List<PlayerBaseInfo> playerBaseInfoList = new List<PlayerBaseInfo>();
        static List<Rank> rankList = new List<Rank>();
        public static async void RefreshRankFromDB()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            rankList.Clear();
            playerBaseInfoList.Clear();
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            playerBaseInfoList.AddRange(await proxyComponent.QueryJsonPlayerInfo<PlayerBaseInfo>($"{{}}"));
            //发送给客户端刷新数据
            
            for(int i = 0;i< playerBaseInfoList.Count; ++i)
            {
                Rank rank = new Rank();
                rank.PlayerName = playerBaseInfoList[i].Name;
                rank.GoldNum = playerBaseInfoList[i].GoldNum;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetRankData(rankList);
            if (playerBaseInfoList.Count > 0)
                playerBaseInfoList.Clear();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
            Game.Scene.GetComponent<DBOperatorComponet>().IsStop = true;
            stopwatch.Stop();
            TimeSpan timespan = stopwatch.Elapsed;
            double sencond = timespan.Seconds;
            double milliseconds = timespan.TotalMilliseconds;
            Log.Debug(JsonHelper.ToJson(playerBaseInfoList));
        }

        public static async void AddItemToDB(UserBag info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            UserBag itemInfo = ComponentFactory.CreateWithId<UserBag>(IdGenerater.GenerateId());
            itemInfo = info;
            await proxyComponent.Save(itemInfo);
        }

        public static async void RefreshRecord()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            
        }
    }
}
