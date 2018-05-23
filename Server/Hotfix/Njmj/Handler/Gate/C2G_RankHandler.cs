using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RankHandler : AMRpcHandler<C2G_Rank, G2C_Rank>
    {
        protected override async void Run(Session session, C2G_Rank message, Action<G2C_Rank> reply)
        {
            G2C_Rank response = new G2C_Rank();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                PlayerBaseInfo my = await proxyComponent.Query<PlayerBaseInfo>(message.Uid);
                WealthRank wealthRank = new WealthRank();
                GameRank gameRank = new GameRank();
                wealthRank.Icon = my.Icon;
                wealthRank.PlayerName = my.Name;
                wealthRank.GoldNum = my.GoldNum;
                wealthRank.UId = my.Id;

                gameRank.PlayerName = my.Name;
                gameRank.Icon = my.Icon;
                gameRank.TotalCount = my.TotalGameCount;
                gameRank.UId = my.Id;

                if (message.RankType == 1)
                {
                    response.RankList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
                    response.OwnWealthRank = wealthRank;
                }
                else if (message.RankType == 2)
                {
                    response.GameRankList = Game.Scene.GetComponent<RankDataComponent>().GetGameRankData();
                    response.OwnGameRank = gameRank;
                }
                else
                {
                    response.RankList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
                    response.GameRankList = Game.Scene.GetComponent<RankDataComponent>().GetGameRankData();
                    response.OwnGameRank = gameRank;
                    response.OwnWealthRank = wealthRank;
                }
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
