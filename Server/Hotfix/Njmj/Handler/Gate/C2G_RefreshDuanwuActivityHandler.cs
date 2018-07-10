using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_RefreshDuanwuActivityHandler : AMRpcHandler<C2G_RefreshDuanwuActivity, G2C_RefreshDuanwuActivity>
    {
        protected override async void Run(Session session, C2G_RefreshDuanwuActivity message, Action<G2C_RefreshDuanwuActivity> reply)
        {
            G2C_RefreshDuanwuActivity response = new G2C_RefreshDuanwuActivity();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DuanwuActivity> duanwuActivityList = new List<DuanwuActivity>();
                List<DuanwuActivityInfo> duanwuList = await proxyComponent.QueryJson<DuanwuActivityInfo>($"{{UId:{message.UId}}}");
                List<DuanwuDataBase> duanwuDataBases = await proxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{message.UId}}}");
                string curTime = CommonUtil.getCurTimeNormalFormat();
                if (string.CompareOrdinal(curTime, duanwuDataBases[0].StartTime) >= 0
                    && string.CompareOrdinal(curTime, duanwuDataBases[0].EndTime) < 0)
                {
                    //活动期间
                    //刷新任务
                    {
                        await DBCommonUtil.ChangeWealth(message.UId, 1, -(int)message.GoldCost, "刷新任务");
                        duanwuDataBases[0].RefreshCount -= 1;
                        if (duanwuDataBases[0].RefreshCount <= 0)
                            duanwuDataBases[0].RefreshCount = 0;
                        duanwuDataBases[0].ActivityType = message.ActivityType;
                        DuanwuData data = new DuanwuData();
                        data.ZongziCount = duanwuDataBases[0].ZongziCount;
                        data.ActivityType = duanwuDataBases[0].ActivityType;
                        data.RefreshCount = duanwuDataBases[0].RefreshCount;
                        data.StartTime = duanwuDataBases[0].StartTime;
                        data.EndTime = duanwuDataBases[0].EndTime;
                        await proxyComponent.Save(duanwuDataBases[0]);
                        response.DuanwuData = data;
                    }

                    if (duanwuList.Count > 0)
                    {
                        for (int i = 0; i < duanwuList.Count; ++i)
                        {
                            duanwuList[i].CurProgress = 0;
                            duanwuList[i].IsComplete = false;
                            duanwuList[i].IsGet = false;
                            await proxyComponent.Save(duanwuList[i]);
                        }
                    }

                    for (int i = 0; i < duanwuList.Count; ++i)
                    {
                        DuanwuActivity duanwuActivity = new DuanwuActivity();
                        DuanwuActivityInfo activity = duanwuList[i];
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

                    PlayerBaseInfo info = await DBCommonUtil.getPlayerBaseInfo(message.UId);
                    response.GoldNum = info.GoldNum;
                }
                else
                {
                    response.Message = "活动还未开始";
                    response.Error = ErrorCode.ERR_NotFoundActor;
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
