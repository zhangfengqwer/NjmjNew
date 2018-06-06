using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class DuanwuData
    {
        static DuanwuData s_instance = null;

        List<DuanwuRewardInfo> listData = new List<DuanwuRewardInfo>();

        public static DuanwuData getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new DuanwuData();
            }

            return s_instance;
        }

        public List<DuanwuRewardInfo> getDataList()
        {
            return listData;
        }

        public DuanwuRewardInfo GetDataByTreasureId(int treasureId)
        {
            for (int i = 0; i < listData.Count; ++i)
            {
                if (listData[i].TreasureId == treasureId)
                    return listData[i];
            }
            return null;
        }
    }

    public class DuanwuRewardInfo
    {
        public int TreasureId { get; set; }
        public int BuyCountLimit { get; set; }
        public string Reward { get; set; }
        public int Price { get; set; }
    }
}
