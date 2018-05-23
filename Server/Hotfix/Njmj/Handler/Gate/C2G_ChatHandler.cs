using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ChatHandler: AMRpcHandler<C2G_Chat, G2C_Chat>
    {
        protected override void Run(Session session, C2G_Chat message, Action<G2C_Chat> reply)
        {
            G2C_Chat response = new G2C_Chat();
            try
            {
                session.Send(new Actor_Chat { ChatType = message.ChatType, Value = message.Value, UId = message.UId });
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
