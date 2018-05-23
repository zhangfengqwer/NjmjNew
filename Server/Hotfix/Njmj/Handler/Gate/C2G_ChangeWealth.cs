using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ChangeWealthHandler : AMRpcHandler<C2G_ChangeWealth, G2C_ChangeWealth>
    {
        protected override async void Run(Session session, C2G_ChangeWealth message, Action<G2C_ChangeWealth> reply)
        {
            G2C_ChangeWealth response = new G2C_ChangeWealth();
            try
            {
                DBCommonUtil.ChangeWealth(message.UId, message.propId, message.propNum);

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
