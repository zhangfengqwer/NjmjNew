using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_DailySignHandler : AMRpcHandler<C2G_DailySign, G2C_DailySign>
    {
        protected override async void Run(Session session, C2G_DailySign message, Action<G2C_DailySign> reply)
        {
            G2C_DailySign response = new G2C_DailySign();
            try
            {
                // 当前连续签到天数
                int curLianXuSignDays = 1;

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
                        // 今天已经签到过
                        response.Error = ErrorCode.TodayHasSign;
                        response.Message = "今天已签到,请明天再试";
                        reply(response);

                        return;
                    }
                    else if (tianshucha == 1)
                    {
                        curLianXuSignDays = 2;

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
                    string reward = getReward(curLianXuSignDays);

                    // 更新用户数据
                    {
                        List<PlayerBaseInfo> playerBaseInfo = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");
                        if (playerBaseInfo.Count > 0)
                        {
                            playerBaseInfo[0].GoldNum += getRewardNum(curLianXuSignDays);
                            await proxyComponent.Save(playerBaseInfo[0]);
                        }
                    }

                    response.Reward = reward;
                    response.TomorrowReward = getReward(curLianXuSignDays + 1);
                    reply(response);

                    DailySign dailySign = ComponentFactory.CreateWithId<DailySign>(IdGenerater.GenerateId());
                    dailySign.Uid = message.Uid;
                    dailySign.Reward = reward;
                    await proxyComponent.Save(dailySign);
                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        static public string getReward(int day)
        {
            return "1:" + getRewardNum(day).ToString();
        }

        static public int getRewardNum(int day)
        {
            int minNum = 100;
            int maxNum = 500;

            int num = minNum * day;
            if (num > maxNum)
            {
                num = maxNum;
            }

            if (num == 0)
            {
                num = minNum;
            }

            return num;
        }
    }
}
