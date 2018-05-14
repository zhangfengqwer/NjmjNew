using System;
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
                List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.UId},TaskId:{message.TaskPrg.TaskId}}}");
                TaskProgressInfo progress = new TaskProgressInfo();
                if (taskProgressInfoList.Count > 0)
                {
                    for(int i = 0;i < taskProgressInfoList.Count; ++i)
                    {
                        progress = taskProgressInfoList[i];
                        if (progress.CurProgress == message.TaskPrg.Target)
                            progress.IsComplete = true;
                        else
                            progress.IsComplete = false;
                    }
                }
                TaskProgress taskProgress = new TaskProgress();
                progress.UId = message.UId;
                progress.TaskId = message.TaskPrg.TaskId;
                progress.CurProgress = message.TaskPrg.Progress;
                progress.Target = message.TaskPrg.Target;

                DBHelper.AddTaskProgressInfoToDB(message.UId, progress);
                response.TaskPrg = new TaskProgress();
                response.TaskPrg.TaskId = progress.TaskId;
                response.TaskPrg.Progress = progress.CurProgress;
                response.TaskPrg.Target = progress.Target;
                response.TaskPrg.IsComplete = progress.IsComplete;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
