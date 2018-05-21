using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    //[ObjectSystem]
    //public class RankDataSystem : AwakeSystem<RankDataComponent>
    //{
    //    public override void Awake(RankDataComponent self)
    //    {
            
    //    }
    //}

    public static class RankData
    {
        private static List<Rank> rankDataList = new List<Rank>();

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
