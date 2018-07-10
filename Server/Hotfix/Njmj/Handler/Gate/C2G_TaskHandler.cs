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
                    for (int j = 0; j < TaskData.getInstance().getDataList().Count; ++j)
                    {
                        TaskProgressInfo info = ComponentFactory.CreateWithId<TaskProgressInfo>(IdGenerater.GenerateId());
                        info.UId = message.uid;
                        info.Name = TaskData.getInstance().getDataList()[j].Name;
                        info.TaskId = (int)TaskData.getInstance().getDataList()[j].Id;
                        info.Target = TaskData.getInstance().getDataList()[j].Target;
                        info.Reward = TaskData.getInstance().getDataList()[j].Reward;
                        info.Desc = TaskData.getInstance().getDataList()[j].Desc;
                        info.CurProgress = 0;
                        await proxyComponent.Save(info);
                    }
                    taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid}}}");
                }
                else if (taskProgressInfoList.Count < TaskData.getInstance().getDataList().Count)
                {
                    for(int i = 0;i< TaskData.getInstance().getDataList().Count; ++i)
                    {
                        List<TaskProgressInfo> infos = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid},TaskId:{ TaskData.getInstance().getDataList()[i].Id}}}");
                        if(infos.Count <= 0)
                        {
                            TaskConfig config = TaskData.getInstance().GetDataByTaskId(TaskData.getInstance().getDataList()[i].Id);
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
                }

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
