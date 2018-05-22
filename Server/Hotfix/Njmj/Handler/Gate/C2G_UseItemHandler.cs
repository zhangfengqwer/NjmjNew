using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UseItemHandler : AMRpcHandler<C2G_UseItem, G2C_UseItem>
    {
        protected override async void Run(Session session, C2G_UseItem message, Action<G2C_UseItem> reply)
        {
            G2C_UseItem response = new G2C_UseItem();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<UserBag> itemInfos = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{message.ItemId}}}");
                if(itemInfos.Count <0)
                {
                    response.Message = "数据不一致";
                    response.result = 0;
                }
                else
                {
                    for(int i = 0;i< itemInfos.Count; ++i)
                    {
                        if (itemInfos[i].Count > 0)
                        {
                            itemInfos[i].Count--;
                            //使用之后的一些参数暂时未处理
                            response.result = 1;
                        }
                        else if(itemInfos[i].Count == 0)
                        {
                            response.result = 0;
                        }
                        await proxyComponent.Save(itemInfos[i]);
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
