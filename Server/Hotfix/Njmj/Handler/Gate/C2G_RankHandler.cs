using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RankHandler : AMRpcHandler<C2G_Rank, G2C_Rank>
    {
        protected override void Run(Session session, C2G_Rank message, Action<G2C_Rank> reply)
        {
            G2C_Rank response = new G2C_Rank();
            try
            {
                response.RankList = Game.Scene.GetComponent<RankDataComponent>().GetWealthRankData();
                response.GameRankList = Game.Scene.GetComponent<RankDataComponent>().GetGameRankData();
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
