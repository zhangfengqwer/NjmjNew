using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_WeekRankHandler : AMRpcHandler<C2G_WeekRank, G2C_WeekRank>
    {
        protected override async void Run(Session session, C2G_WeekRank message, Action<G2C_WeekRank> reply)
        {
            G2C_WeekRank response = new G2C_WeekRank();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> info = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");
                List<WeekRank> weekRank = await proxyComponent.QueryJson<WeekRank>($"{{UId:{message.UId}}}");

                if(weekRank.Count <= 0)
                {
                    WeekRank weekInfo = ComponentFactory.CreateWithId<WeekRank>(IdGenerater.GenerateId());
                    weekInfo.UId = message.UId;
                    weekInfo.IsGetGameRank = false;
                    weekInfo.IsGetGoldRank = false;
                    weekInfo.GameIndex = -1;
                    weekInfo.GoldIndex = -1;
                    await proxyComponent.Save(weekInfo);
                }
                else
                {
                    response.IsGetGameRank = weekRank[0].IsGetGameRank;
                    response.IsGetGoldRank = weekRank[0].IsGetGameRank;
                    response.GameIndex = weekRank[0].GameIndex;
                    response.WealthIndex = weekRank[0].GoldIndex;
                    reply(response);
                    return;
                }

                weekRank = await proxyComponent.QueryJson<WeekRank>($"{{UId:{message.UId}}}");

                if (CommonUtil.IsMonday())
                {
                    for (int i = 0; i < Game.Scene.GetComponent<RankDataComponent>().GetFWealthRankData().Count; ++i)
                    {
                        if (message.UId == Game.Scene.GetComponent<RankDataComponent>().GetFWealthRankData()[i].UId)
                        {
                            weekRank[0].IsGetGoldRank = true;
                            weekRank[0].GoldIndex = i + 1;
                            await proxyComponent.Save(weekRank[0]);
                            break;
                        }
                        else
                        {
                            weekRank[0].IsGetGoldRank = false;
                            weekRank[0].GoldIndex = -1;
                            await proxyComponent.Save(weekRank[0]);
                        }
                    }

                    for (int i = 0; i < Game.Scene.GetComponent<RankDataComponent>().GetFGameRankData().Count; ++i)
                    {
                        if (message.UId == Game.Scene.GetComponent<RankDataComponent>().GetFGameRankData()[i].UId)
                        {
                            weekRank[0].IsGetGameRank = true;
                            weekRank[0].GameIndex = i + 1;
                            await proxyComponent.Save(weekRank[0]);
                            break;
                        }
                        else
                        {
                            weekRank[0].IsGetGameRank = false;
                            weekRank[0].GameIndex = -1;
                            await proxyComponent.Save(weekRank[0]);
                        }
                        
                    }

                    response.IsGetGameRank = weekRank[0].IsGetGameRank;
                    response.IsGetGoldRank = weekRank[0].IsGetGoldRank;
                    response.GameIndex = weekRank[0].GameIndex;
                    response.WealthIndex = weekRank[0].GoldIndex;

                }
                else
                {
                    weekRank[0].IsGetGameRank = false;
                    weekRank[0].IsGetGoldRank = false;
                    weekRank[0].GameIndex = -1;
                    weekRank[0].GoldIndex = -1;
                    await proxyComponent.Save(weekRank[0]);
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
