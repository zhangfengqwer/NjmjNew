
using ETModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETHotfix
{
    public class DBHelper
    {
        public static async void RefreshRankFromDB()
        {
            RefreshWealthRank();
            RefreshGameRank();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
            //Game.Scene.GetComponent<DBOperatorComponet>().IsStop = true;
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
                List<PlayerBaseInfo> playerbaseinfos = await proxyComponent.QueryJsonDB<PlayerBaseInfo>($"{{_id:{gamePlayerList[i].UId}}}");
                GameRank rank = new GameRank();
                rank.PlayerName = playerbaseinfos[0].Name;
                rank.WinCount = gamePlayerList[i].WinGameCount;
                rank.TotalCount = playerbaseinfos[0].TotalGameCount;
                rank.Icon = playerbaseinfos[0].Icon;
                rank.UId = playerbaseinfos[0].Id;
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
                List<PlayerBaseInfo> playerbaseinfos = await proxyComponent.QueryJsonDB<PlayerBaseInfo>($"{{_id:{playerBaseInfoList[i].UId}}}");
                WealthRank rank = new WealthRank();
                rank.PlayerName = playerbaseinfos[0].Name;
                rank.GoldNum = playerBaseInfoList[i].Wealth;
                rank.Icon = playerbaseinfos[0].Icon;
                rank.UId = playerbaseinfos[0].Id;
                rankList.Add(rank);
            }

            Game.Scene.GetComponent<RankDataComponent>().SetWealthRankData(rankList);
            stopwatch.Stop();
            TimeSpan timespan = stopwatch.Elapsed;
            double sencond = timespan.Seconds;
            double milliseconds = timespan.TotalMilliseconds;
        }
    }
}
