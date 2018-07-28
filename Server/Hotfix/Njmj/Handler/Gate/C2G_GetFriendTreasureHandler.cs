using ETModel;
using System;
using System.Collections.Generic;
using System.Net;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_GetFriendTreasureHandler : AMRpcHandler<C2G_GetFriendTreasure, G2C_GetFriendTreasure>
    {
        protected override async void Run(Session session, C2G_GetFriendTreasure message, Action<G2C_GetFriendTreasure> reply)
        {
            G2C_GetFriendTreasure response = new G2C_GetFriendTreasure();
            try
            {
                //获取房间信息
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<FriendKeyConsum> consums = await proxyComponent.QueryJson<FriendKeyConsum>($"{{UId:{message.UId},CreateTime:/^{DateTime.Now.GetCurrentDay()}/}}");
                if(consums.Count > 0)
                {
                    if(consums[0].ConsumCount >= 5)
                    {
                        if(consums[0].GetCount >= 5)
                        {
                            response.Error = ErrorCode.ERR_Common;
                            response.Message = "今日领取次数已经满5次，不能领取";
                            reply(response);
                            return;
                        }
                        else
                        {
                            //可以领取
                            consums[0].GetCount += 1;
                            consums[0].ConsumCount -= 5;
                            await proxyComponent.Save(consums[0]);
                            int rd = Common_Random.getRandom(1, 100);
                            if(rd > 69)
                            {
                                //一个话费红包和五千金币
                                await DBCommonUtil.changeWealthWithStr(message.UId, "111:1;1:5000","好友房开启宝箱获得");
                            }
                            else if(rd >= 69 && rd < 99)
                            {
                                //二个话费红包和一千金币
                                await DBCommonUtil.changeWealthWithStr(message.UId, "111:2;1:1000", "好友房开启宝箱获得");
                            }
                            else 
                            {
                                //五个话费红包和一千金币
                                await DBCommonUtil.changeWealthWithStr(message.UId, "111:5;1:5000", "好友房开启宝箱获得");
                            }

                            response.AlGetCount = consums[0].GetCount;
                            response.KeyCount = consums[0].ConsumCount;
                        }
                    }
                    else
                    {
                        response.Error = ErrorCode.ERR_Common;
                        response.Message = "今日消耗的钥匙不足五个，不能领取";
                        reply(response);
                        return;
                    }
                }
                else
                {
                    response.Error = ErrorCode.ERR_Common;
                    response.Message = "数据库不存在使用钥匙的记录,请检查数据";
                    reply(response);
                    return;
                }
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
