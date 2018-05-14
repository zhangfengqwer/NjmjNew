using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_TaskHandler : AMRpcHandler<C2G_Task,G2C_Task>
    {
        protected override async void Run(Session session, C2G_Task message, Action<G2C_Task> reply)
        {
            G2C_Task response = new G2C_Task();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid}}}");
                if (taskProgressInfoList.Count <= 0)
                {
                    response.Message = "任务正在进行中";
                    response.TaskProgressList = null;
                    reply(response);
                }
                else
                {
                    List<TaskProgress> taskProgressList = new List<TaskProgress>();
                    for(int i = 0;i< taskProgressInfoList.Count; ++i)
                    {
                        TaskProgress taskProgress = new TaskProgress();
                        taskProgress.TaskId = taskProgressInfoList[i].TaskId;
                        taskProgress.Progress = taskProgressInfoList[i].CurProgress;
                        taskProgress.IsComplete = taskProgressInfoList[i].IsComplete;
                        taskProgressList.Add(taskProgress);
                    }
                    response.TaskProgressList = taskProgressList;
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
