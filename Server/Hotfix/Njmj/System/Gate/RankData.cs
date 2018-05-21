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
        private static List<Rank> rankDataList = new List<Rank>();

        public static async void Awake(this RankDataComponent component)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfoList = new List<PlayerBaseInfo>();
            List<Rank> rankList = new List<Rank>();
            playerBaseInfoList.AddRange(await proxyComponent.QueryJsonPlayerInfo<PlayerBaseInfo>($"{{}}"));
            for (int i = 0; i < playerBaseInfoList.Count; ++i)
            {
                Rank rank = new Rank();
                rank.PlayerName = playerBaseInfoList[i].Name;
                rank.GoldNum = playerBaseInfoList[i].GoldNum;
                rankList.Add(rank);
            }
            Game.Scene.GetComponent<RankDataComponent>().SetRankData(rankList);
            playerBaseInfoList = null;
            rankList = null;
        }

        public static void SetRankData(this RankDataComponent component,List<Rank> rankList)
        {
            rankDataList = rankList;
        }

        public static List<Rank> GetRankData(this RankDataComponent component)
        {
            return rankDataList;
        }
    }
}
