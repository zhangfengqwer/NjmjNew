using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Gate)]
	public class M2G_Actor_GamerExitRoomHandler : AMActorHandler<User, M2G_Actor_GamerExitRoom>
	{
	    protected override async Task Run(User user, M2G_Actor_GamerExitRoom message)
	    {
	        Log.Info("Gate收到Map");
            user.ActorID = 0;
	        await Task.CompletedTask;
	    }
	}
}