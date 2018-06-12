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
                response.RankList = new List<WealthRank>();
                response.GameRankList = new List<GameRank>();
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
                    GetWealthRank(response, wealthRank);
                    response.OwnWealthRank = wealthRank;
                }
                else if (message.RankType == 2)
                {
                    GetGameRank(response, gameRank);
                    response.OwnGameRank = gameRank;
                }
                else
                {
                    GetWealthRank(response, wealthRank);
                    GetGameRank(response, gameRank);
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

        protected void GetWealthRank(G2C_Rank response, WealthRank wealthRank)
        {
            if (Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData().Count <= 0)
            {
                response.RankList.Add(wealthRank);
            }
            else
            {
                response.RankList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
            }
        }

        protected void GetGameRank(G2C_Rank response,GameRank gameRank)
        {
            if (Game.Scene.GetComponent<RankDataComponent>().GetGameRankData().Count <= 0)
            {
                response.GameRankList.Add(gameRank);
            }
            else
            {
                response.GameRankList = Game.Scene.GetComponent<RankDataComponent>().GetGameRankData();
            }
        }
    }
}
