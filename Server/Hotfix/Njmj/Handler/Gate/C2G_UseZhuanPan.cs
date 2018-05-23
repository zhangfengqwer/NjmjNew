using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UseZhuanPanHandler : AMRpcHandler<C2G_UseZhuanPan, G2C_UseZhuanPan>
    {
        protected override async void Run(Session session, C2G_UseZhuanPan message, Action<G2C_UseZhuanPan> reply)
        {
            G2C_UseZhuanPan response = new G2C_UseZhuanPan();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerBaseInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.Uid}}}");

                if (playerBaseInfos[0].ZhuanPanCount <= 0)
                {
                    response.Error = ErrorCode.TodayHasSign;
                    response.Message = "您的次数不足";
                    reply(response);

                    return;
                }
                else
                {
                    --playerBaseInfos[0].ZhuanPanCount;
                    ++playerBaseInfos[0].LuckyValue;

                    response.reward = getReward(playerBaseInfos[0].LuckyValue);

                    // 满98后重置
                    if (playerBaseInfos[0].LuckyValue >= 98)
                    {
                        playerBaseInfos[0].LuckyValue = 0;
                    }

                    await proxyComponent.Save(playerBaseInfos[0]);

                    reply(response);
                }
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        string getReward(int luckyValue)
        {
            string reward = "1:1";

            if (luckyValue >= 98)
            {
                reward = "1:1000";
            }
            else
            {
                reward = "1:100";
            }

            return reward;
        }
    }
}
