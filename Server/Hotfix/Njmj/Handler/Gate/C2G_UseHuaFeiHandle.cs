using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UseHuaFeiHandler : AMRpcHandler<C2G_UseHuaFei, G2C_UseHuaFei>
    {
        protected override async void Run(Session session, C2G_UseHuaFei message, Action<G2C_UseHuaFei> reply)
        {
            G2C_UseHuaFei response = new G2C_UseHuaFei();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

                // 兑换话费
                if (message.Type == 1)
                {
                    if (message.HuaFei == 5 * 100)
                    {
                        PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(message.Uid);
                        if (playerBaseInfo.HuaFeiNum >= 5 * 100)
                        {
                            List<UseHuaFei> useHuaFeis = await proxyComponent.QueryJson<UseHuaFei>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.Uid},HuaFei:{message.HuaFei}}}");
                            if (useHuaFeis.Count > 0)
                            {
                                response.Error = ErrorCode.TodayHasSign;
                                response.Message = "您今天的兑换机会已用完";
                                reply(response);

                                return;
                            }
                            else
                            {
                                // 充值话费
                                {
                                    string str = HttpUtil.PhoneFeeRecharge(message.Uid.ToString().Substring(1), "话费", "5", message.Phone, "3", "1");
                                    Log.Debug("=======" + str);

                                    if (!CommonUtil.checkHuaFeiChongZhiResult(str))
                                    {
                                        response.Message = "充值失败";
                                        response.Error = ErrorCode.ERR_PhoneCodeError;
                                        reply(response);
                                        return;
                                    }
                                    // 充值成功
                                    else
                                    {
                                        // 充值话费
                                        UseHuaFei useHuaFei = ComponentFactory.CreateWithId<UseHuaFei>(IdGenerater.GenerateId());
                                        useHuaFei.Uid = message.Uid;
                                        useHuaFei.HuaFei = message.HuaFei;
                                        useHuaFei.Phone = message.Phone;
                                        await proxyComponent.Save(useHuaFei);

                                        await DBCommonUtil.ChangeWealth(message.Uid, 3, -5 * 100, "话费充值");
                                    }
                                }

                                reply(response);
                            }
                        }
                        else
                        {
                            response.Error = ErrorCode.TodayHasSign;
                            response.Message = "您的话费余额不足";
                            reply(response);

                            return;
                        }
                    }
                    else
                    {
                        // 不合法的金额
                        response.Error = ErrorCode.TodayHasSign;
                        response.Message = "您的充值金额不存在";
                        reply(response);

                        return;
                    }
                }
                // 兑换元宝
                else if (message.Type == 2)
                {
                    if (message.HuaFei == 1 * 100)
                    {
                        PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(message.Uid);
                        if (playerBaseInfo.HuaFeiNum >= 1 * 100)
                        {
                            List<UseHuaFei> useHuaFeis = await proxyComponent.QueryJson<UseHuaFei>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.Uid},HuaFei:{message.HuaFei}}}");
                            if (useHuaFeis.Count >= 10)
                            {
                                response.Error = ErrorCode.TodayHasSign;
                                response.Message = "您今天的兑换机会已用完";
                                reply(response);

                                return;
                            }
                            else
                            {
                                // 兑换10个元宝
                                {
                                    await DBCommonUtil.ChangeWealth(message.Uid,2,10, "话费兑换元宝");

                                    {
                                        // 记录日志
                                        UseHuaFei useHuaFei = ComponentFactory.CreateWithId<UseHuaFei>(IdGenerater.GenerateId());
                                        useHuaFei.Uid = message.Uid;
                                        useHuaFei.HuaFei = message.HuaFei;
                                        useHuaFei.Phone = message.Phone;
                                        await proxyComponent.Save(useHuaFei);

                                        await DBCommonUtil.ChangeWealth(message.Uid, 3, -1 * 100, "话费兑换元宝");
                                    }
                                }

                                response.Reward = "2:10";

                                reply(response);
                            }
                        }
                        else
                        {
                            response.Error = ErrorCode.TodayHasSign;
                            response.Message = "您的话费余额不足";
                            reply(response);

                            return;
                        }
                    }
                    else
                    {
                        // 不合法的金额
                        response.Error = ErrorCode.TodayHasSign;
                        response.Message = "您的充值金额不存在";
                        reply(response);

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
