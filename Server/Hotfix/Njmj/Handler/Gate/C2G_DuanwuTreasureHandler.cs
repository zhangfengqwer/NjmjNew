using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_DuanwuTreasureHandler : AMRpcHandler<C2G_DuanwuTreasure, G2C_DuanwuTreasure>
    {
        protected override async void Run(Session session, C2G_DuanwuTreasure message, Action<G2C_DuanwuTreasure> reply)
        {
            G2C_DuanwuTreasure response = new G2C_DuanwuTreasure();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
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
                        duanwuInfo.Name = "普通宝箱";
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 2;
                        duanwuInfo.Reward = "1:2000;111:2";
                        duanwuInfo.BuyCountLimit = 10;
                        duanwuInfo.Price = 99;
                        duanwuInfo.Name = "普通宝箱";
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 3;
                        duanwuInfo.Reward = "1:2000;111:2";
                        duanwuInfo.BuyCountLimit = 10;
                        duanwuInfo.Price = 99;
                        duanwuInfo.Name = "普通宝箱";
                        duanwuList.Add(duanwuInfo);
                    }
                    {
                        //精品宝箱
                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 4;
                        duanwuInfo.Reward = "1:9000;111:5;actIcon1:1";
                        duanwuInfo.BuyCountLimit = 5;
                        duanwuInfo.Price = 288;
                        duanwuInfo.Name = "精品宝箱";
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 5;
                        duanwuInfo.Reward = "1:9000;111:5;actIcon2:1";
                        duanwuInfo.BuyCountLimit = 5;
                        duanwuInfo.Price = 288;
                        duanwuInfo.Name = "精品宝箱";
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 6;
                        duanwuInfo.Reward = "1:9000;111:5;actIcon3:1";
                        duanwuInfo.BuyCountLimit = 5;
                        duanwuInfo.Price = 288;
                        duanwuInfo.Name = "精品宝箱";
                        duanwuList.Add(duanwuInfo);
                    }
                    {
                        //豪华宝箱
                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 7;
                        duanwuInfo.Reward = "1:20000;111:8;107:1;actIcon4:1";
                        duanwuInfo.BuyCountLimit = 1;
                        duanwuInfo.Price = 666;
                        duanwuInfo.Name = "豪华宝箱";
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 8;
                        duanwuInfo.Reward = "1:20000;111:8;107:1;actIcon5:1";
                        duanwuInfo.BuyCountLimit = 1;
                        duanwuInfo.Price = 666;
                        duanwuInfo.Name = "豪华宝箱";
                        duanwuList.Add(duanwuInfo);

                        duanwuInfo = new DuanwuRewardInfo();
                        duanwuInfo.TreasureId = 9;
                        duanwuInfo.Reward = "1:20000;111:8;107:1;actIcon6:1";
                        duanwuInfo.BuyCountLimit = 1;
                        duanwuInfo.Price = 666;
                        duanwuInfo.Name = "豪华宝箱";
                        duanwuList.Add(duanwuInfo);
                    }
                    DuanwuRewardData.getInstance().getDataList().AddRange(duanwuList);
                }

                List<DuanwuTreasureInfo> treasures = await proxyComponent.QueryJson<DuanwuTreasureInfo>($"{{UId:{message.UId}}}");

                if(treasures.Count <= 0)
                {
                    for(int i = 0;i< DuanwuRewardData.getInstance().getDataList().Count; ++i)
                    {
                        DuanwuTreasureInfo treasureInfo = ComponentFactory.CreateWithId<DuanwuTreasureInfo>(IdGenerater.GenerateId());
                        treasureInfo.UId = message.UId;
                        treasureInfo.TreasureId = DuanwuRewardData.getInstance().getDataList()[i].TreasureId;
                        await proxyComponent.Save(treasureInfo);
                    }
                }

                List<TreasureInfo> treasureInfoList = new List<TreasureInfo>();
                for (int i = 0; i < DuanwuRewardData.getInstance().getDataList().Count; ++i)
                {
                    DuanwuRewardInfo config = DuanwuRewardData.getInstance().getDataList()[i];
                    TreasureInfo info = new TreasureInfo();
                    info.TreasureId = config.TreasureId;
                    info.Reward = config.Reward;
                    info.Price = config.Price;
                    info.LimitCount = config.BuyCountLimit;
                    info.Name = config.Name;
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
