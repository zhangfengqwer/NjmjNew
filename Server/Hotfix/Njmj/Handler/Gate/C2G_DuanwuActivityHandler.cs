using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_DuanwuActivityHandler : AMRpcHandler<C2G_DuanwuActivity, G2C_DuanwuActivity>
    {
        protected override async void Run(Session session, C2G_DuanwuActivity message, Action<G2C_DuanwuActivity> reply)
        {
            G2C_DuanwuActivity response = new G2C_DuanwuActivity();
            try
            {
                List<DuanwuActivity> duanwuActivityList = new List<DuanwuActivity>();
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                List<DuanwuActivityInfo> duanwuInfoList = await proxyComponent.QueryJson<DuanwuActivityInfo>($"{{UId:{message.UId}}}");
                if (duanwuInfoList.Count <= 0)
                {
                    for (int j = 0; j < configCom.GetAll(typeof(DuanwuActivityConfig)).Length; ++j)
                    {
                        int id = 100 + j + 1;
                        DuanwuActivityConfig config = (DuanwuActivityConfig)configCom.Get(typeof(DuanwuActivityConfig), id);
                        DuanwuActivityInfo info = ComponentFactory.CreateWithId<DuanwuActivityInfo>(IdGenerater.GenerateId());
                        info.UId = message.UId;
                        info.TaskId = (int)config.Id;
                        info.Target = config.Target;
                        info.Reward = config.Reward;
                        info.Desc = config.Desc;
                        await proxyComponent.Save(info);
                    }
                }

                duanwuInfoList = await proxyComponent.QueryJson<DuanwuActivityInfo>($"{{UId:{message.UId}}}");
                for (int i = 0; i < duanwuInfoList.Count; ++i)
                {
                    DuanwuActivity duanwuActivity = new DuanwuActivity();
                    DuanwuActivityInfo activity = duanwuInfoList[i];
                    duanwuActivity.TaskId = activity.TaskId;
                    duanwuActivity.Desc = activity.Desc;
                    duanwuActivity.Reward = activity.Reward;
                    duanwuActivity.IsComplete = activity.IsComplete;
                    duanwuActivity.IsGet = activity.IsGet;
                    duanwuActivity.CurProgress = activity.CurProgress;
                    duanwuActivity.Target = activity.Target;
                    duanwuActivityList.Add(duanwuActivity);
                }

                response.DuanwuActivityList = duanwuActivityList;
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
