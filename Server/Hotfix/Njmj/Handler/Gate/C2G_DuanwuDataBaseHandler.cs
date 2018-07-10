using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_DuanwuDataBaseHandler : AMRpcHandler<C2G_DuanwuDataBase, G2C_DuanwuDataBase>
    {
        bool IsDataNew;
        protected override async void Run(Session session, C2G_DuanwuDataBase message, Action<G2C_DuanwuDataBase> reply)
        {
            G2C_DuanwuDataBase response = new G2C_DuanwuDataBase();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DuanwuDataBase> duanwuDataBases = await proxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{message.UId}}}");
                if (duanwuDataBases.Count <= 0)
                {
                    IsDataNew = true;
                    //新建一个数据库的表
                    DuanwuDataBase duanwu = ComponentFactory.CreateWithId<DuanwuDataBase>(IdGenerater.GenerateId());
                    duanwu.UId = message.UId;
                    await proxyComponent.Save(duanwu);
                }
                if (IsDataNew)
                {
                    duanwuDataBases = await proxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{message.UId}}}");
                    IsDataNew = false;
                }
                //单纯请求数据
                if (message.Type == 1)
                {
                    DuanwuData data = new DuanwuData();
                    data.ZongziCount = duanwuDataBases[0].ZongziCount;
                    data.ActivityType = duanwuDataBases[0].ActivityType;
                    data.RefreshCount = duanwuDataBases[0].RefreshCount;
                    data.StartTime = duanwuDataBases[0].StartTime;
                    data.EndTime = duanwuDataBases[0].EndTime;
                    response.DuanwuData = data;
                }
                {
                    //更改活动类型
                    if (message.Type == 2)
                    {
                        duanwuDataBases[0].ActivityType = message.ActivityType;
                    }
                    await proxyComponent.Save(duanwuDataBases[0]);
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
