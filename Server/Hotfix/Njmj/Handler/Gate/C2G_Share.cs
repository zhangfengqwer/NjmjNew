using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ShareHandler : AMRpcHandler<C2G_Share, G2C_Share>
    {
        protected override async void Run(Session session, C2G_Share message, Action<G2C_Share> reply)
        {
            G2C_Share response = new G2C_Share();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();

                // 分享增加转盘次数
                {
                    List<Log_UseZhuanPan> log_UseZhuanPan = await proxyComponent.QueryJson<Log_UseZhuanPan>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{ message.Uid}}}");

                    PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(message.Uid);

                    // 贵族
                    if (playerBaseInfo.VipTime.CompareTo(CommonUtil.getCurTimeNormalFormat()) > 0)
                    {
                        if (log_UseZhuanPan.Count == 4)
                        {
                            Log.Debug("该用户是贵族，游戏增加的转盘次数已用完，通过分享增加次数");
                            playerBaseInfo.ZhuanPanCount = 1;
                            await proxyComponent.Save(playerBaseInfo);
                        }
                    }
                    else
                    {
                        if (log_UseZhuanPan.Count == 3)
                        {
                            Log.Debug("该用户是普通玩家，游戏增加的转盘次数已用完，通过分享增加次数");
                            playerBaseInfo.ZhuanPanCount = 1;
                            await proxyComponent.Save(playerBaseInfo);
                        }
                    }
                }

                // 分享日志
                {
                    Log_Share log_Share = ComponentFactory.CreateWithId<Log_Share>(IdGenerater.GenerateId());
                    log_Share.Uid = message.Uid;
                    await proxyComponent.Save(log_Share);
                }

                reply(response);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                ReplyError(response, e, reply);
            }
        }
    }
}
