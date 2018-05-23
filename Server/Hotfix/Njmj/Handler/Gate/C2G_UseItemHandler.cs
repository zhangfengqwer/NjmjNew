using ETModel;
using System;
using System.Collections.Generic;

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
                    response.Message = "数据不一致";
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

                            //使用之后的一些参数暂时未处理
                            response.result = 1;

                            useProp(message.UId, message.ItemId);
                        }
                        else
                        {
                            response.Message = "数据不一致";
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

        async void useProp(long uid, int prop_id)
        {
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
                            playerBaseInfo.EmogiTime = (CommonUtil.timeAddDays(playerBaseInfo.EmogiTime,30));
                        }
                        // 已过期
                        else
                        {
                            playerBaseInfo.EmogiTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 30));
                        }

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // VIP7天体验卡
                case 107:
                    {
                        // 未过期
                        if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            playerBaseInfo.VipTime = (CommonUtil.timeAddDays(playerBaseInfo.VipTime, 7));
                        }
                        // 已过期
                        else
                        {
                            playerBaseInfo.VipTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 7));
                        }

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // VIP月卡
                case 108:
                    {
                        // 未过期
                        if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            playerBaseInfo.VipTime = (CommonUtil.timeAddDays(playerBaseInfo.VipTime, 30));
                        }
                        // 已过期
                        else
                        {
                            playerBaseInfo.VipTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 30));
                        }

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;

                // VIP季卡
                case 109:
                    {
                        // 未过期
                        if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                        {
                            playerBaseInfo.VipTime = (CommonUtil.timeAddDays(playerBaseInfo.VipTime, 90));
                        }
                        // 已过期
                        else
                        {
                            playerBaseInfo.VipTime = (CommonUtil.timeAddDays(CommonUtil.getCurTimeNormalFormat(), 90));
                        }

                        await proxyComponent.Save(playerBaseInfo);
                    }
                    break;
            }
        }
    }
}
