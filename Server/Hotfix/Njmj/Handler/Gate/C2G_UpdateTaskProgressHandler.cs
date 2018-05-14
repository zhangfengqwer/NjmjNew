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
                List<TaskProgressInfo> taskProgressList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.UId}}}");
                if(taskProgressList.Count <= 0)
                {
                    response.Message = "任务正在进行中";
                    response.TaskPrg = null;
                    reply(response);
                }
                else
                {
                    TaskProgressInfo progress = new TaskProgressInfo();
                    TaskProgress taskProgress = new TaskProgress();
                    progress.UId = message.UId;
                    progress.TaskId = message.TaskPrg.TaskId;
                    progress.CurProgress = message.TaskPrg.Progress;

                    DBHelper.AddTaskProgressInfoToDB(message.UId, progress);
                    response.TaskPrg.TaskId = progress.TaskId;
                    response.TaskPrg.Progress = progress.CurProgress;
                    reply(response);
                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
