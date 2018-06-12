using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ChengjiuHandler : AMRpcHandler<C2G_Chengjiu,G2C_Chengjiu>
    {
        protected override async void Run(Session session, C2G_Chengjiu message, Action<G2C_Chengjiu> reply)
        {
            G2C_Chengjiu response = new G2C_Chengjiu();
            try
            {
                List<TaskInfo> taskInfoList = new List<TaskInfo>();
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<ChengjiuInfo> chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.Uid}}}");

                if (chengjiuInfoList.Count <= 0)
                {
                    for (int i = 1; i < configCom.GetAll(typeof(ChengjiuConfig)).Length + 1; ++i)
                    {
                        int id = 100 + i;
                        ChengjiuConfig config = (ChengjiuConfig)configCom.Get(typeof(ChengjiuConfig), id);
                        ChengjiuInfo progress = new ChengjiuInfo();
                        progress.IsGet = false;
                        progress.UId = message.Uid;
                        progress.Name = config.Name;
                        progress.TaskId = (int)config.Id;
                        progress.IsComplete = false;
                        progress.Target = config.Target;
                        progress.Reward = config.Reward;
                        progress.Desc = config.Desc;
                        progress.CurProgress = 0;
                        DBHelper.AddChengjiuInfoToDB(message.Uid, progress);
                    }
                }
                chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.Uid}}}");

                for (int i = 0; i < chengjiuInfoList.Count; ++i)
                {
                    TaskInfo taskInfo = new TaskInfo();
                    ChengjiuInfo chengjiu = chengjiuInfoList[i];
                    taskInfo.Id = chengjiu.TaskId;
                    taskInfo.TaskName = chengjiu.Name;
                    taskInfo.Desc = chengjiu.Desc;
                    taskInfo.Reward = chengjiu.Reward;
                    taskInfo.IsComplete = chengjiu.IsComplete;
                    taskInfo.IsGet = chengjiu.IsGet;
                    taskInfo.Progress = chengjiu.CurProgress;
                    taskInfo.Target = chengjiu.Target;
                    taskInfoList.Add(taskInfo);
                }
                response.ChengjiuList = taskInfoList;
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
