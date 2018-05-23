using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class DBCommonUtil
    {
        public static async void UpdateTask(long uid,int taskId,bool isGet = false)
        {
            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            TaskInfo taskInfo = new TaskInfo();
            TaskProgressInfo progress = new TaskProgressInfo();
            List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{uid},TaskId:{taskId}}}");

            if (taskProgressInfoList.Count > 0)
            {
                for (int i = 0; i < taskProgressInfoList.Count; ++i)
                {
                    progress = taskProgressInfoList[i];
                    ++progress.CurProgress;
                    progress.TaskId = taskId;
                    progress = taskProgressInfoList[i];
                    ++progress.CurProgress;
                    progress.TaskId =taskId;
                    if (isGet)
                    {
                        progress.IsGet = true;
                    }
                    else
                    {
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
            }
        }
    }
        
}
