using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetWeekRewardHandler : AMRpcHandler<C2G_GetWeekReward, G2C_GetWeekReward>
    {
        protected override async void Run(Session session, C2G_GetWeekReward message, Action<G2C_GetWeekReward> reply)
        {
            G2C_GetWeekReward response = new G2C_GetWeekReward();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<WeekRank> weeks = await proxyComponent.QueryJson<WeekRank>($"{{UId:{message.UId}}}");
                if(weeks.Count > 0)
                {
                    if(message.type == 1)
                    {
                        if (weeks[0].IsGetGoldRank)
                        {
                            await DBCommonUtil.changeWealthWithStr(message.UId, GetReward(1, weeks[0].GoldIndex), "领取周财富榜奖励");
                            weeks[0].IsGetGoldRank = false;
                            await proxyComponent.Save(weeks[0]);
                        }
                    }
                    else if(message.type == 2)
                    {
                        if (weeks[0].IsGetGameRank)
                        {
                            await DBCommonUtil.changeWealthWithStr(message.UId, GetReward(2, weeks[0].GameIndex), "领取周财富榜奖励");
                            weeks[0].IsGetGameRank = false;
                            await proxyComponent.Save(weeks[0]);
                        }
                    }
                }
                else
                {
                    response.Error = ErrorCode.ERR_Exception;
                    response.Message = "该用户不存在weekRank记录，请检查";
                }

                response.IsGetGameRank = weeks[0].IsGetGameRank;
                response.IsGetGoldRank = weeks[0].IsGetGoldRank;

                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        protected string GetReward(int type,int index)
        {
            string reward = "";
            if(type == 2)
            {
                if (index == 1)
                {
                    reward = "111:" + 10;
                }
                else if(index == 2)
                {
                    reward = "111:" + 8;
                }
                else if(index == 3)
                {
                    reward = "111:" + 6;
                }
                else if(index > 3 && index <= 10)
                {
                    reward = "111:" + 3;
                }
                else if(index > 10 && index <= 20)
                {
                    reward = "111:" + 2; 
                }
                else if(index > 20 && index <= 50)
                {
                    reward = "111:" + 1;
                }
            }
            else if(type == 1)
            {
                if (index == 1)
                {
                    reward = "2:" + 20;
                }
                else if (index == 2)
                {
                    reward = "2:" + 15;
                }
                else if (index == 3)
                {
                    reward = "2:" + 10;
                }
                else if (index > 3 && index <= 10)
                {
                    reward = "2:" + 5;
                }
                else if (index > 10 && index <= 20)
                {
                    reward = "2:" + 3;
                }
                else if(index > 20 && index <= 50)
                {
                    reward = "2:" + 1;
                }
            }
            return reward;
        }
    }
}
