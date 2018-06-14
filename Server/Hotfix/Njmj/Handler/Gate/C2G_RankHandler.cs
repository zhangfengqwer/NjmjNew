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
                List<PlayerBaseInfo> mys = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");
                if (mys.Count > 0)
                {
                    response.RankList = new List<WealthRank>();
                    response.GameRankList = new List<GameRank>();
                    WealthRank wealthRank = new WealthRank();
                    GameRank gameRank = new GameRank();
                    wealthRank.Icon = mys[0].Icon;
                    wealthRank.PlayerName = mys[0].Name;
                    wealthRank.GoldNum = mys[0].GoldNum;
                    wealthRank.UId = mys[0].Id;

                    gameRank.PlayerName = mys[0].Name;
                    gameRank.Icon = mys[0].Icon;
                    gameRank.TotalCount = mys[0].TotalGameCount;
                    gameRank.UId = mys[0].Id;


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
                }
                else
                {
                    Log.Error($"玩家{message.Uid}playerbaseinfo为空");
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
