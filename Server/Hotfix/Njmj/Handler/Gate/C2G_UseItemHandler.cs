using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UseItemHandler : AMRpcHandler<C2G_UseItem, G2C_UseItem>
    {
        protected override async void Run(Session session, C2G_UseItem message, Action<G2C_UseItem> reply)
        {
            G2C_UseItem response = new G2C_UseItem();

            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{message.ItemId}}}");
                if(itemInfos.Count <= 0)
                {
                    response.Message = "您的道具数量不足";
                    response.result = 0;
                }
                else
                {
                    for(int i = 0;i< itemInfos.Count; ++i)
                    {
                        if (itemInfos[i].Count > 0)
                        {
                            --itemInfos[i].Count;
                            await proxyComponent.Save(itemInfos[i]);
                            
                            response.result = 1;

                            await useProp(response,message.UId, message.ItemId);
                        }
                        else
                        {
                            response.Message = "您的道具数量不足";
                            response.result = 0;
                        }
                    }
                }

                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        public async Task useProp(G2C_UseItem response,long uid, int prop_id)
        {
            string endTime = "";
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            List<PlayerBaseInfo> playerBaseInfoList = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{uid}}}");
            PlayerBaseInfo playerBaseInfo = playerBaseInfoList[0];

            switch (prop_id)
            {
                // 表情包
                case 104:
                    {
                        // 未过期
                        if (playerBaseInfo.EmogiTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            endTime = (CommonUtil.timeAddDays(playerBaseInfo.EmogiTime, 30));
                        }
                        // 已过期
                        else
                        {
                            endTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 30));
                        }

                        playerBaseInfo.EmogiTime = endTime;
                        response.time = endTime;

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // VIP7天体验卡
                case 107:
                    {
                        // 未过期
                        if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            endTime = (CommonUtil.timeAddDays(playerBaseInfo.VipTime, 7));
                        }
                        // 已过期
                        else
                        {
                            endTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 7));
                        }

                        playerBaseInfo.VipTime = endTime;
                        response.time = endTime;

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // VIP月卡
                case 108:
                    {
                        // 未过期
                        if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            endTime = (CommonUtil.timeAddDays(playerBaseInfo.VipTime, 30));
                        }
                        // 已过期
                        else
                        {
                            endTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 30));
                        }

                        playerBaseInfo.VipTime = endTime;
                        response.time = endTime;

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // VIP季卡
                case 109:
                    {
                        // 未过期
                        if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            endTime = (CommonUtil.timeAddDays(playerBaseInfo.VipTime, 90));
                        }
                        // 已过期
                        else
                        {
                            endTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 90));
                        }

                        playerBaseInfo.VipTime = endTime;
                        response.time = endTime;

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // 话费礼包
                case 111:
                    {
                        int r = Common_Random.getRandom(1,100);
                        float huafei = r / 100.0f;
                        string reward = ("3:" + huafei);
                        response.reward = reward;
                        Log.Debug("话费礼包：" + reward);
                        await DBCommonUtil.changeWealthWithStr(playerBaseInfo.Id, reward);
                    }
                    break;
            }
        }
    }
}
