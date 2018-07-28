using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class H2G_GamerChargeHandler : AMRpcHandler<H2G_GamerCharge, G2H_GamerCharge>
    {
        protected override void Run(Session session, H2G_GamerCharge message, Action<G2H_GamerCharge> reply)
        {
            G2H_GamerCharge response = new G2H_GamerCharge();

            try
            {
                UserComponent userComponent = Game.Scene.GetComponent<UserComponent>();
                User user = userComponent.Get(message.UId);

                user.session.Call(new Actor_GamerBuyYuanBao()
                {
                    goodsId = message.goodsId
                });
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
