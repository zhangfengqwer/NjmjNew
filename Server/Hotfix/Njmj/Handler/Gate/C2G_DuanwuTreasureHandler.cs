using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_DuanwuTreasureHandler : AMRpcHandler<C2G_DuanwuTreasure, G2C_DuanwuTreasure>
    {
        protected override void Run(Session session, C2G_DuanwuTreasure message, Action<G2C_DuanwuTreasure> reply)
        {
            G2C_DuanwuTreasure response = new G2C_DuanwuTreasure();
            try
            {
                #region duanwuData
                if (DuanwuRewardData.getInstance().getDataList().Count == 0)
                {
                    List<DuanwuRewardInfo> duanwuList = new List<DuanwuRewardInfo>();
                    DuanwuRewardInfo duanwuInfo = new DuanwuRewardInfo();
                    {
                        //普通宝箱
                        duanwuInfo.TreasureId = 1;
                        duanwuInfo.Reward = "1:2000;111:2";
                        duanwuInfo.BuyCountLimit = 10;
                        duanwuInfo.Price = 99;
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 2;
                        duanwuInfo.Reward = "1:2000;111:2";
                        duanwuInfo.BuyCountLimit = 10;
                        duanwuInfo.Price = 99;
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 3;
                        duanwuInfo.Reward = "1:2000;111:2";
                        duanwuInfo.BuyCountLimit = 10;
                        duanwuInfo.Price = 99;
                        duanwuList.Add(duanwuInfo);
                    }
                    {
                        //精品宝箱
                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 4;
                        duanwuInfo.Reward = "1:9000;111:5;actIcon1:1";
                        duanwuInfo.BuyCountLimit = 5;
                        duanwuInfo.Price = 288;
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 5;
                        duanwuInfo.Reward = "1:9000;111:5;actIcon2:1";
                        duanwuInfo.BuyCountLimit = 5;
                        duanwuInfo.Price = 288;
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 6;
                        duanwuInfo.Reward = "1:9000;111:5;activity3:1";
                        duanwuInfo.BuyCountLimit = 5;
                        duanwuInfo.Price = 288;
                        duanwuList.Add(duanwuInfo);
                    }
                    {
                        //豪华宝箱
                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 7;
                        duanwuInfo.Reward = "1:20000;111:8;107:1;actIcon4:1";
                        duanwuInfo.BuyCountLimit = 1;
                        duanwuInfo.Price = 666;
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 8;
                        duanwuInfo.Reward = "1:20000;111:8;107:1;actIcon5:1";
                        duanwuInfo.BuyCountLimit = 1;
                        duanwuInfo.Price = 666;
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 9;
                        duanwuInfo.Reward = "1:20000;111:8;107:1;actIcon6:1";
                        duanwuInfo.BuyCountLimit = 1;
                        duanwuInfo.Price = 666;
                        duanwuList.Add(duanwuInfo);
                    }
                    DuanwuRewardData.getInstance().getDataList().AddRange(duanwuList);
                }

                List<TreasureInfo> treasureInfoList = new List<TreasureInfo>();
                for (int i = 0; i < DuanwuRewardData.getInstance().getDataList().Count; ++i)
                {
                    DuanwuRewardInfo config = DuanwuRewardData.getInstance().getDataList()[i];
                    TreasureInfo info = new TreasureInfo();
                    info.TreasureId = config.TreasureId;
                    info.Reward = config.Reward;
                    info.Price = config.Price;
                    treasureInfoList.Add(info);
                }
                response.TreasureInfoList = treasureInfoList;
                reply(response);
                #endregion
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
