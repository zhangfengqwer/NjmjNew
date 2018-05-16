﻿using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UpdateTaskProgressHandler : AMRpcHandler<C2G_UpdateTaskProgress, G2C_UpdateTaskProgress>
    {
        protected override async void Run(Session session, C2G_UpdateTaskProgress message, Action<G2C_UpdateTaskProgress> reply)
        {
            G2C_UpdateTaskProgress response = new G2C_UpdateTaskProgress();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                TaskInfo taskInfo = new TaskInfo();
                response.TaskPrg = new TaskInfo();
                TaskProgressInfo progress = new TaskProgressInfo();
                List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.UId},TaskId:{message.TaskPrg.Id}}}");
                
                if (taskProgressInfoList.Count > 0)
                {
                    for(int i = 0;i< taskProgressInfoList.Count; ++i)
                    {
                        progress = taskProgressInfoList[i];
                        progress.TaskId = message.TaskPrg.Id;
                        progress.CurProgress = message.TaskPrg.Progress;
                        if (message.TaskPrg.IsGet)
                        {
                            progress.IsGet = true;
                        }
                        else
                        {
                            if (message.TaskPrg.Progress == progress.Target)
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