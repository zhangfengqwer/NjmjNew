﻿using System;
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
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid}}}");
                if (taskProgressInfoList.Count <= 0)
                {
                    for (int i = 1; i < configCom.GetAll(typeof(TaskConfig)).Length + 1; ++i)
                    {
                        int id = 100 + i;
                        TaskConfig config = (TaskConfig)configCom.Get(typeof(TaskConfig), id);
                        TaskProgressInfo progress = new TaskProgressInfo();
                        progress.IsGet = false;
                        progress.UId = message.uid;
                        progress.Name = config.Name;
                        progress.TaskId = (int)config.Id;
                        progress.IsComplete = false;
                        progress.Target = config.Target;
                        progress.Reward = config.Reward;
                        progress.Desc = config.Desc;
                        progress.CurProgress = 0;
                        DBHelper.AddTaskProgressInfoToDB(message.uid, progress);
                    }
                }
                taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.uid}}}");
                if (taskProgressInfoList.Count <= 0)
                    Log.Warning("数据未写进数据库");
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
