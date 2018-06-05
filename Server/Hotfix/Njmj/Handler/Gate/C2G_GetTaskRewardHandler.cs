using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetTaskRewardHandler : AMRpcHandler<C2G_GetTaskReward, G2C_GetTaskReward>
    {
        protected override async void Run(Session session, C2G_GetTaskReward message, Action<G2C_GetTaskReward> reply)
        {
            G2C_GetTaskReward response = new G2C_GetTaskReward();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                //1,任务；2,成就
                if (message.GetType == 1)
                {
                    List<TaskProgressInfo> taskProgressInfoList = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.UId},TaskId:{message.TaskInfo.Id}}}");

                    if (taskProgressInfoList.Count > 0)
                    {
                        if (!taskProgressInfoList[0].IsComplete)
                        {
                            response.Error = ErrorCode.TaskNotComplete;
                            response.Message = "任务奖励还未完成，不能领取奖励";
                            taskProgressInfoList[0].IsGet = false;
                            await proxyComponent.Save(taskProgressInfoList[0]);
                        }
                        else
                        {
                            taskProgressInfoList[0].IsGet = true;
                            await DBCommonUtil.ChangeWealth(message.UId,1,message.TaskInfo.Reward);
                            await proxyComponent.Save(taskProgressInfoList[0]);
                        }
                    }
                }
                else if(message.GetType == 2)
                {
                    List<ChengjiuInfo> chengjiuInfoList = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.UId},TaskId:{message.TaskInfo.Id}}}");

                    if (chengjiuInfoList.Count > 0)
                    {
                        if (!chengjiuInfoList[0].IsComplete)
                        {
                            response.Error = ErrorCode.TaskNotComplete;
                            response.Message = "任务奖励还未完成，不能领取奖励";
                            chengjiuInfoList[0].IsGet = false;
                            await proxyComponent.Save(chengjiuInfoList[0]);
                        }
                        else
                        {
                            chengjiuInfoList[0].IsGet = true;
                            await DBCommonUtil.ChangeWealth(message.UId, 1, message.TaskInfo.Reward);
                            await proxyComponent.Save(chengjiuInfoList[0]);
                        }
                    }
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
