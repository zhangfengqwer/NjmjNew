using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_TipHandler : AMRpcHandler<C2G_Tip, G2C_Tip>
    {
        protected override async void Run(Session session, C2G_Tip message, Action<G2C_Tip> reply)
        {
            G2C_Tip response = new G2C_Tip();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                {
                    //任务
                    List<TaskProgressInfo> taskProgressInfos = await proxyComponent.QueryJson<TaskProgressInfo>($"{{UId:{message.UId}}}");
                    response.IsTaskComplete = false;
                    if (taskProgressInfos.Count > 0)
                    {
                        for (int i = 0; i < taskProgressInfos.Count; ++i)
                        {
                            if (taskProgressInfos[i].IsComplete && (!taskProgressInfos[i].IsGet))
                            {
                                response.IsTaskComplete = true;
                            }
                        }
                    }
                }
                {
                    //成就
                    List<ChengjiuInfo> chengjius = await proxyComponent.QueryJson<ChengjiuInfo>($"{{UId:{message.UId}}}");
                    response.IsChengjiuComplete = false;
                    if (chengjius.Count > 0)
                    {
                        for (int i = 0; i < chengjius.Count; ++i)
                        {
                            if (chengjius[i].IsComplete && (!chengjius[i].IsGet))
                            {
                                response.IsChengjiuComplete = true;
                            }
                        }
                    }
                }
                {
                    //活动
                    List<DuanwuDataBase> activitys = await proxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{message.UId}}}");
                   if(activitys.Count > 0)
                    {
                        string curTime = CommonUtil.getCurTimeNormalFormat();
                        if (string.CompareOrdinal(curTime, activitys[0].StartTime) >= 0
                    && string.CompareOrdinal(curTime, activitys[0].EndTime) < 0)
                        {
                            response.IsInActivity = true;
                        }
                        else
                        {
                            response.IsInActivity = false;
                        }
                    }
                }
                {
                    //转盘
                    List<PlayerBaseInfo> playerInfos = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");
                    if(playerInfos.Count > 0)
                    {
                        if(playerInfos[0].ZhuanPanCount > 0)
                        {
                            response.IsZhuanpan = true;
                        }
                        else
                        {
                            response.IsZhuanpan = false;
                        }
                    }
                }
                {
                    //邮件
                    List<EmailInfo> emails = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.UId}}}");
                    response.IsEmail = false;
                    if (emails.Count > 0)
                    {
                        for(int i = 0;i< emails.Count;++i)
                        {
                            if (emails[i].State == 0)
                            {
                                response.IsEmail = true;
                            }
                        }
                    }
                }
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
