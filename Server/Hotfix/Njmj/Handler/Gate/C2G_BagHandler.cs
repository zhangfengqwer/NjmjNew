using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_BagHandler : AMRpcHandler<C2G_BagOperation, G2C_BagOperation>
    {
        protected override async void Run(Session session, C2G_BagOperation message, Action<G2C_BagOperation> reply)
        {
            G2C_BagOperation response = new G2C_BagOperation();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<ItemInfo> bagInfoList = await proxyComponent.QueryJson<ItemInfo>($"{{UId:{message.UId}}}");
                response.ItemList = new List<Item>();
                List<Item> itemList = new List<Item>();
                for(int i = 0;i< bagInfoList.Count; ++i)
                {
                    Item item = new Item();
                    item.ItemId = bagInfoList[i].BagId;
                    item.Count = bagInfoList[i].Count;
                    itemList.Add(item);
                }
                response.ItemList = itemList;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
