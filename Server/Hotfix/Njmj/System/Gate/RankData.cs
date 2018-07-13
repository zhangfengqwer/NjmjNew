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
                List<PlayerBaseInfo> info = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{logRanklist[i].UId}}}");
                WealthRank rank = new WealthRank();
                rank.PlayerName = info[0].Name;
                rank.GoldNum = logRanklist[i].Wealth;
                rank.Icon = info[0].Icon;
                rank.UId = info[0].Id;
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
            logRankList.Clear();
            List<GameRank> rankList = new List<GameRank>();
            logRankList.AddRange(await proxyComponent.QueryJsonRank(2));
            for (int i = 0; i < logRankList.Count; ++i)
            {
                List<PlayerBaseInfo> info = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{logRankList[i].UId}}}");
                GameRank rank = new GameRank();
                rank.PlayerName = info[0].Name;
                rank.WinCount = logRankList[i].WinGameCount;
                rank.TotalCount = info[0].TotalGameCount;
                rank.Icon = info[0].Icon;
                rank.UId = info[0].Id;
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
        public static async void SetFRankData(this RankDataComponent component)
        {
            fRankDataList.Clear();
            fGameRankList.Clear();
            fRankDataList.AddRange(rankDataList);
            fGameRankList.AddRange(gameRankList);
            Log.Info("==============================================================rankDataList\r\n" + JsonHelper.ToJson(rankDataList) + "\r\n=========================================================");
            Log.Info("==============================================================gameRankList\r\n" + JsonHelper.ToJson(gameRankList) + "\r\n=========================================================");
            //结算后数据清零
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            await proxyComponent.DeleteAll<Log_Rank>();
            await proxyComponent.DeleteAll<WeekRank>();
            rankDataList.Clear();
            gameRankList.Clear();
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
