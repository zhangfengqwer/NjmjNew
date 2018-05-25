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
                PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(message.Uid);

                if (playerBaseInfo.ZhuanPanCount <= 0)
                {
                    response.Error = ErrorCode.TodayHasSign;
                    response.Message = "您的抽奖次数不足";
                    reply(response);

                    return;
                }
                else
                {
                    playerBaseInfo.ZhuanPanCount -= 1;
                    playerBaseInfo.LuckyValue += 1;

                    response.itemId = getRewardItemId(playerBaseInfo.LuckyValue);
                    response.reward = getReward(response.itemId, playerBaseInfo.LuckyValue);

                    await DBCommonUtil.changeWealthWithStr(message.Uid, response.reward);

                    // 转盘日志
                    {
                        Log_UseZhuanPan log_UseZhuanPan = ComponentFactory.CreateWithId<Log_UseZhuanPan>(IdGenerater.GenerateId());
                        log_UseZhuanPan.Uid = message.Uid;
                        log_UseZhuanPan.Reward = response.reward;
                        await proxyComponent.Save(log_UseZhuanPan);
                    }

                    // 满99后重置
                    if (playerBaseInfo.LuckyValue >= 99)
                    {
                        playerBaseInfo.LuckyValue = 0;
                    }
                    
                    await proxyComponent.Save(playerBaseInfo);
                    reply(response);
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                ReplyError(response, e, reply);
            }
        }

        int getRewardItemId(int luckyValue)
        {
            int itemId = 1;

            int r = Common_Random.getRandom(1, 10000);
            int temp = 0;

            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
            List<ZhuanpanConfig> shopInfoList = new List<ZhuanpanConfig>();
            for (int i = 1; i < configCom.GetAll(typeof(ZhuanpanConfig)).Length + 1; ++i)
            {
                ZhuanpanConfig config = (ZhuanpanConfig)configCom.Get(typeof(ZhuanpanConfig), i);

                if (r <= (config.Probability + temp))
                {
                    itemId = config.itemId;
                    break;
                }
                else
                {
                    temp += config.Probability;
                }
            }

            return itemId;
        }

        string getReward(int itemId,int LuckyValue)
        {
            string reward = "1:1";

            ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
            List<ZhuanpanConfig> shopInfoList = new List<ZhuanpanConfig>();
            for (int i = 1; i < configCom.GetAll(typeof(ZhuanpanConfig)).Length + 1; ++i)
            {
                ZhuanpanConfig config = (ZhuanpanConfig)configCom.Get(typeof(ZhuanpanConfig), i);

                if (config.itemId == itemId)
                {
                    reward = (config.propId + ":" + config.PropNum);

                    if (LuckyValue >= 99)
                    {
                        reward += ";111:10";
                    }
                }
            }

            return reward;
        }
    }
}
