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
        public static void Awake(this RankDataComponent component)
        {
            InitWealthRankInfo();
            InitGameRankInfo();
        }

        private static async void InitWealthRankInfo()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfoList = new List<PlayerBaseInfo>();
            List<WealthRank> rankList = new List<WealthRank>();
            playerBaseInfoList.AddRange(await proxyComponent.QueryJsonPlayerInfo<PlayerBaseInfo>($"{{}}"));
            for (int i = 0; i < playerBaseInfoList.Count; ++i)
            {
                WealthRank rank = new WealthRank();
                rank.PlayerName = playerBaseInfoList[i].Name;
                rank.GoldNum = playerBaseInfoList[i].GoldNum;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetWealthRankData(rankList);
            playerBaseInfoList = null;
            rankList = null;
        }

        private static async void InitGameRankInfo()
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfoList = new List<PlayerBaseInfo>();
            List<GameRank> rankList = new List<GameRank>();
            playerBaseInfoList.AddRange(await proxyComponent.QueryJsonGamePlayer<PlayerBaseInfo>($"{{}}"));
            for (int i = 0; i < playerBaseInfoList.Count; ++i)
            {
                GameRank rank = new GameRank();
                rank.PlayerName = playerBaseInfoList[i].Name;
                rank.WinCount = playerBaseInfoList[i].WinGameCount;
                rank.TotalCount = playerBaseInfoList[i].TotalGameCount;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetGameRankData(rankList);
            playerBaseInfoList = null;
            rankList = null;
        }

        public static void SetWealthRankData(this RankDataComponent component,List<WealthRank> rankList)
        {
            rankDataList = rankList;
        }

        public static List<WealthRank> GetWealthRankData(this RankDataComponent component)
        {
            return rankDataList;
        }

        public static void SetGameRankData(this RankDataComponent component,List<GameRank> gmRankList)
        {
            gameRankList = gmRankList;
        }

        public static List<GameRank> GetGameRankData(this RankDataComponent component)
        {
            return gameRankList;
        }
    }
}
