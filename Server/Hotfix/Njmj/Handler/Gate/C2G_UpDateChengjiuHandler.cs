using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UpDateChengjiuHandler : AMRpcHandler<C2G_UpdateChengjiu,G2C_UpdateChengjiu>
    {
        protected override async void Run(Session session, C2G_UpdateChengjiu message, Action<G2C_UpdateChengjiu> reply)
        {
            G2C_UpdateChengjiu response = new G2C_UpdateChengjiu();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                TaskInfo taskInfo = new TaskInfo();
                response.TaskPrg = new TaskInfo();
                ChengjiuInfo progress = new ChengjiuInfo();
                List<ChengjiuInfo> chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.UId},TaskId:{message.TaskPrg.Id}}}");

                if (chengjiuInfoList.Count > 0)
                {
                    for (int i = 0; i < chengjiuInfoList.Count; ++i)
                    {
                        progress = chengjiuInfoList[i];
                        progress.TaskId = message.TaskPrg.Id;
                        if (message.TaskPrg.IsGet)
                        {
                            progress.IsGet = true;
                        }
                        else
                        {
                            ++progress.CurProgress;
                            if (progress.CurProgress == progress.Target)
                            {
                                progress.IsComplete = true;
                            }
                            else
                            {
                                progress.IsComplete = false;
                            }
                        }
                        await proxyComponent.Save(progress);
                    }
                    taskInfo.Id = progress.TaskId;
                    taskInfo.IsGet = progress.IsGet;
                    taskInfo.IsComplete = progress.IsComplete;
                    taskInfo.Progress = progress.CurProgress;
                    response.TaskPrg = taskInfo;
                }
                else
                {
                    response.Message = "不存在该任务ID";
                    response.TaskPrg = null;
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
