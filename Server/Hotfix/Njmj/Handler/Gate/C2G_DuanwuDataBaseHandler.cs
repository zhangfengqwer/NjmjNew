using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_DuanwuDataBaseHandler : AMRpcHandler<C2G_DuanwuDataBase, G2C_DuanwuDataBase>
    {
        protected override async void Run(Session session, C2G_DuanwuDataBase message, Action<G2C_DuanwuDataBase> reply)
        {
            G2C_DuanwuDataBase response = new G2C_DuanwuDataBase();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                DuanwuDataBase duanwuDataBase = await proxyComponent.Query<DuanwuDataBase>(message.UId);
                if(duanwuDataBase != null)
                {
                    //单纯请求数据
                    if(message.Type == 1)
                    {
                        DuanwuData data = new DuanwuData();
                        data.ZongziCount = duanwuDataBase.ZongziCount;
                        data.ActivityType = duanwuDataBase.ActivityType;
                        response.DuanwuData = data;
                    }
                    //更改活动类型
                    if(message.Type == 2)
                    {
                        duanwuDataBase.ActivityType = message.ActivityType;
                    }
                    //刷新任务
                    if(message.Type == 3)
                    {
                        await DBCommonUtil.ChangeWealth(message.UId, 1, -(int)message.GoldCost, "刷新任务");
                        duanwuDataBase.RefreshCount -= 1;
                        if (duanwuDataBase.RefreshCount <= 0)
                            duanwuDataBase.RefreshCount = 0;
                        duanwuDataBase.ActivityType = message.ActivityType;
                    }
                    await proxyComponent.Save(duanwuDataBase);
                }
                else
                {
                    //新建一个数据库的表
                    {
                        duanwuDataBase = ComponentFactory.CreateWithId<DuanwuDataBase>(IdGenerater.GenerateId());
                        duanwuDataBase.UId = message.UId;
                        await proxyComponent.Save(duanwuDataBase);
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
