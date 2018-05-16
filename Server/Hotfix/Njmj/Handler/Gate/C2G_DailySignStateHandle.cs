using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_DailySignStateHandler : AMRpcHandler<C2G_DailySignState, G2C_DailySignState>
    {
        protected override async void Run(Session session, C2G_DailySignState message, Action<G2C_DailySignState> reply)
        {
            G2C_DailySignState response = new G2C_DailySignState();
            try
            {
                // 当前连续签到天数
                int curLianXuSignDays = 0;
                bool TodayIsSign = false;

                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DailySign> dailySigns = await proxyComponent.QueryJson<DailySign>($"{{Uid:{message.Uid}}}");
                dailySigns.Sort(delegate (DailySign x, DailySign y)
                {
                    return x.CreateTime.CompareTo(y.CreateTime);
                });

                if (dailySigns.Count > 0)
                {
                    int tianshucha = CommonUtil.tianshucha(dailySigns[dailySigns.Count - 1].CreateTime, CommonUtil.getCurTimeNormalFormat());
                    if (tianshucha == 0)
                    {
                        TodayIsSign = true;
                        curLianXuSignDays = 1;

                        if (dailySigns.Count >= 2)
                        {
                            for (int i = dailySigns.Count - 1; i >= 1; i--)
                            {
                                int temp = CommonUtil.tianshucha(dailySigns[i - 1].CreateTime, dailySigns[i].CreateTime);
                                if (temp > 1)
                                {
                                    break;
                                }
                                else if (temp == 1)
                                {
                                    ++curLianXuSignDays;
                                }
                            }
                        }
                    }
                }

                {
                    response.TodayIsSign = TodayIsSign;
                    response.TodayReward = C2G_DailySignHandler.getReward(curLianXuSignDays);
                    response.TomorrowReward = C2G_DailySignHandler.getReward(curLianXuSignDays + 1);
                    reply(response);
                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
