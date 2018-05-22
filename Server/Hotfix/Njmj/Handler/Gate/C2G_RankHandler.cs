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
                PlayerInfo myPlayer = new PlayerInfo();
                myPlayer.GoldNum = my.GoldNum;
                myPlayer.Icon = my.Icon;
                myPlayer.TotalGameCount = my.TotalGameCount;
                myPlayer.Name = my.Name;
                response.PlayerInfo = myPlayer;
                if (message.RankType == 1)
                {
                    response.RankList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
                }
                else if (message.RankType == 2)
                    response.GameRankList = Game.Scene.GetComponent<RankDataComponent>().GetGameRankData();
                else
                {
                    response.RankList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
                    response.GameRankList = Game.Scene.GetComponent<RankDataComponent>().GetGameRankData();
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
