using ETModel;
using System;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_BagHandler : AMRpcHandler<C2G_BagOperation, G2C_BagOperation>
    {
        protected override void Run(Session session, C2G_BagOperation message, Action<G2C_BagOperation> reply)
        {
            G2C_BagOperation response = new G2C_BagOperation();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
