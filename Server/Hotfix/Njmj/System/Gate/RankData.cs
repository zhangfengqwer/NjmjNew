using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    [ObjectSystem]
    public class RankDataSystem : AwakeSystem<RankDataComponent>
    {
        public override void Awake(RankDataComponent self)
        {
            self.Awake();
        }
    }

    public static class RankData
    {
        private static List<WealthRank> rankDataList = new List<WealthRank>();
        private static List<GameRank> gameRankList = new List<GameRank>();

        private static List<WealthRank> fRankDataList = new List<WealthRank>();
        private static List<GameRank> fGameRankList = new List<GameRank>();

        public static void Awake(this RankDataComponent component)
        {
            // 全局定时器
            GlobalTimer.getInstance().start();
            InitWealthRankInfo();
            InitGameRankInfo();
        }

        private static async void InitWealthRankInfo()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Rank> logRanklist = new List<Log_Rank>();
            List<WealthRank> rankList = new List<WealthRank>();
            logRanklist.AddRange(await proxyComponent.QueryJsonRank(1));
            for (int i = 0; i < logRanklist.Count; ++i)
            {
                PlayerBaseInfo info = await DBCommonUtil.getPlayerBaseInfo(logRanklist[i].UId);
                WealthRank rank = new WealthRank();
                rank.PlayerName = info.Name;
                rank.GoldNum = logRanklist[i].Wealth;
                rank.Icon = info.Icon;
                rank.UId = info.Id;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetWealthRankData(rankList);
            logRanklist = null;
            rankList = null;
        }

        private static async void InitGameRankInfo()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<Log_Rank> logRankList = new List<Log_Rank>();
            List<GameRank> rankList = new List<GameRank>();
            logRankList.AddRange(await proxyComponent.QueryJsonRank(2));
            for (int i = 0; i < logRankList.Count; ++i)
            {
                PlayerBaseInfo info = await DBCommonUtil.getPlayerBaseInfo(logRankList[i].UId);
                GameRank rank = new GameRank();
                rank.PlayerName = info.Name;
                rank.WinCount = logRankList[i].WinGameCount;
                rank.TotalCount = info.TotalGameCount;
                rank.Icon = info.Icon;
                rank.UId = info.Id;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetGameRankData(rankList);
            logRankList = null;
            rankList = null;
        }

        public static void SetWealthRankData(this RankDataComponent component, List<WealthRank> rankList)
        {
            rankDataList = rankList;
        }

        public static List<WealthRank> GetWealthRankData(this RankDataComponent component)
        {
            return rankDataList;
        }

        public static void SetGameRankData(this RankDataComponent component, List<GameRank> gmRankList)
        {
            gameRankList = gmRankList;
        }

        public static List<GameRank> GetGameRankData(this RankDataComponent component)
        {
            return gameRankList;
        }

        //设置前一周的记录
        public static void SetFRankData(this RankDataComponent component)
        {
            fRankDataList = rankDataList;
            fGameRankList = gameRankList;
        }

        public static List<WealthRank> GetFWealthRankData(this RankDataComponent component)
        {
            return fRankDataList;
        }

        public static List<GameRank> GetFGameRankData(this RankDataComponent component)
        {
            return fGameRankList;
        }


    }
}
