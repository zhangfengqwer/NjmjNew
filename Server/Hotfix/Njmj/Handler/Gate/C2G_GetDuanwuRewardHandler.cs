using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_GetDuanwuRewardHandler : AMRpcHandler<C2G_GetDuanwuReward, G2C_GetDuanwuReward>
    {
        protected override async void Run(Session session, C2G_GetDuanwuReward message, Action<G2C_GetDuanwuReward> reply)
        {
            G2C_GetDuanwuReward response = new G2C_GetDuanwuReward();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DuanwuActivity> activity = new List<DuanwuActivity>();
                List<DuanwuActivityInfo> infos = await proxyComponent.QueryJson<DuanwuActivityInfo>($"{{UId:{message.UId},TaskId:{message.TaskId}}}");
                if(infos.Count > 0)
                {
                    DuanwuDataBase data = await DBCommonUtil.GetDuanwuDataBase(message.UId);
                    data.ZongziCount += message.Reward;
                    response.ZongziCount = data.ZongziCount;
                    data.CompleteCount += 1;
                    await proxyComponent.Save(data);
                    infos[0].IsGet = true;
                    response.IsGet = infos[0].IsGet;

                    await proxyComponent.Save(infos[0]);
                }
                else
                {
                    Log.Error($"用户{message.UId}的任务{message.TaskId}数据为空");
                }
                reply(response);   
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
