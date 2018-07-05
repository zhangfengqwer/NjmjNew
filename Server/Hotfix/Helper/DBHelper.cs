
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

        public static async void AddTaskProgressInfoToDB( TaskProgressInfo info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskProgressInfo taskInfo = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
            taskInfo = info;
            await proxyComponent.Save(taskInfo);
        }

        public static async void AddChengjiuInfoToDB(long uid,ChengjiuInfo info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            ChengjiuInfo chengjiuInfo = ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
            chengjiuInfo = info;
            await proxyComponent.Save(chengjiuInfo);
        }

        public static async void RefreshRankFromDB()
        {
            RefreshWealthRank();
            RefreshGameRank();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
            //Game.Scene.GetComponent<DBOperatorComponet>().IsStop = true;
        }

        public static async void AddItemToDB(UserBag info)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            UserBag itemInfo = ComponentFactory.CreateWithId<UserBag>(IdGenerater.GenerateId());
            itemInfo = info;
            await proxyComponent.Save(itemInfo);
        }

        private static List<Log_Rank> gamePlayerList = new List<Log_Rank>();
        static List<GameRank> gameRankList = new List<GameRank>();
        public static async void RefreshGameRank()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            gamePlayerList.Clear();
            gameRankList.Clear();
            gamePlayerList.AddRange(await proxyComponent.QueryJsonRank(2));
            for (int i = 0; i < gamePlayerList.Count; ++i)
            {
                PlayerBaseInfo info = await DBCommonUtil.getPlayerBaseInfo(gamePlayerList[i].UId);
                GameRank rank = new GameRank();
                rank.PlayerName = info.Name;
                rank.WinCount = gamePlayerList[i].WinGameCount;
                rank.TotalCount = info.TotalGameCount;
                rank.Icon = info.Icon;
                rank.UId = info.Id;
                gameRankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetGameRankData(gameRankList);
        }

        static List<Log_Rank> playerBaseInfoList = new List<Log_Rank>();
        static List<WealthRank> rankList = new List<WealthRank>();
        public async static void RefreshWealthRank()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            rankList.Clear();
            playerBaseInfoList.Clear();
            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            playerBaseInfoList.AddRange(await proxyComponent.QueryJsonRank(1));
            List<WealthRank> wealthList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
            for (int i = 0; i < playerBaseInfoList.Count; ++i)
            {
                PlayerBaseInfo info = await DBCommonUtil.getPlayerBaseInfo(gamePlayerList[i].UId);
                WealthRank rank = new WealthRank();
                rank.PlayerName = info.Name;
                rank.GoldNum = playerBaseInfoList[i].Wealth;
                rank.Icon = info.Icon;
                rank.UId = info.Id;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetWealthRankData(rankList);
        }
    }
}
