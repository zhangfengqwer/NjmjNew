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
                List<TaskInfo> taskInfoList = new List<TaskInfo>();
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid}}}");

                if (taskProgressInfoList.Count <= 0)
                {
                    for (int j = 0; j < configCom.GetAll(typeof(TaskConfig)).Length; ++j)
                    {
                        int id = 100 + j + 1;
                        TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                        TaskProgressInfo info = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
                        info.UId = message.uid;
                        info.Name = config.Name;
                        info.TaskId = (int)config.Id;
                        info.Target = config.Target;
                        info.Reward = config.Reward;
                        info.Desc = config.Desc;
                        info.CurProgress = 0;
                        await proxyComponent.Save(info);
                    }
                }

                taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid}}}");
                for (int i = 0; i < taskProgressInfoList.Count; ++i)
                {
                    TaskInfo taskInfo = new TaskInfo();
                    TaskProgressInfo taskProgress = taskProgressInfoList[i];
                    taskInfo.Id = taskProgress.TaskId;
                    taskInfo.TaskName = taskProgress.Name;
                    taskInfo.Desc = taskProgress.Desc;
                    taskInfo.Reward = taskProgress.Reward;
                    taskInfo.IsComplete = taskProgress.IsComplete;
                    taskInfo.IsGet = taskProgress.IsGet;
                    taskInfo.Progress = taskProgress.CurProgress;
                    taskInfo.Target = taskProgress.Target;
                    taskInfoList.Add(taskInfo);
                }

                response.TaskProgressList = taskInfoList;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
