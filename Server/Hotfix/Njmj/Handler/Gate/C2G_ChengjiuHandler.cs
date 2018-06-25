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
                    for (int i = 1; i < ChengjiuData.getInstance().getDataList().Count; ++i)
                    {
                        ChengjiuInfo progress = ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
                        progress.IsGet = false;
                        progress.UId = message.Uid;
                        progress.Name = ChengjiuData.getInstance().getDataList()[i].Name;
                        progress.TaskId = (int)ChengjiuData.getInstance().getDataList()[i].Id;
                        progress.IsComplete = false;
                        progress.Target = ChengjiuData.getInstance().getDataList()[i].Target;
                        progress.Reward = ChengjiuData.getInstance().getDataList()[i].Reward;
                        progress.Desc = ChengjiuData.getInstance().getDataList()[i].Desc;
                        progress.CurProgress = 0;
                        await proxyComponent.Save(progress);
                    }
                    chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.Uid}}}");
                }
                else if(chengjiuInfoList.Count < ChengjiuData.getInstance().getDataList().Count)
                {
                    for (int i = 0; i < ChengjiuData.getInstance().getDataList().Count; ++i)
                    {
                        List<ChengjiuInfo> infos = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.Uid},TaskId:{ ChengjiuData.getInstance().getDataList()[i].Id}}}");
                        if (infos.Count <= 0)
                        {
                            ChengjiuConfig config = ChengjiuData.getInstance().GetDataByChengjiuId(ChengjiuData.getInstance().getDataList()[i].Id);
                            ChengjiuInfo info = ComponentFactory.CreateWithId<ChengjiuInfo>(IdGenerater.GenerateId());
                            info.UId = message.Uid;
                            info.Name = config.Name;
                            info.TaskId = (int)config.Id;
                            info.Target = config.Target;
                            info.Reward = config.Reward;
                            info.Desc = config.Desc;
                            info.CurProgress = 0;
                            await proxyComponent.Save(info);
                        }
                    }
                    chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.Uid}}}");
                }

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
