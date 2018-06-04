using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_CheatHandler : AMActorHandler<Gamer, Actor_GamerCheat>
    {
        protected override async Task Run(Gamer gamer, Actor_GamerCheat message)
        {
            try
            {
                Log.Info("收到作弊：" + JsonHelper.ToJson(message));
             
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            await Task.CompletedTask;
        }
    }
}
