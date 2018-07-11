using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_IsCanRechargeHandle : AMRpcHandler<C2G_IsCanRecharge, G2C_IsCanRecharge>
    {
        protected override async void Run(Session session, C2G_IsCanRecharge message, Action<G2C_IsCanRecharge> reply)
        {
            G2C_IsCanRecharge response = new G2C_IsCanRecharge();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<Log_Recharge> listData = await proxyComponent.QueryJsonDB<Log_Recharge>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.UId}}}");
                int todayAllRecharge = 0;
                for (int i = 0; i < listData.Count; i++)
                {
                    todayAllRecharge += listData[i].Price;
                }

                if (todayAllRecharge < 5000)
                {
                    reply(response);
                }
                // 超过5000禁止充值
                else
                {
                    response.Error = ErrorCode.TodayHasSign;
                    response.Message = "今日充值已达上限";
                    reply(response);
                }
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
