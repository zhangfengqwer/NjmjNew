using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Gate)]
	public class M2G_GamerExitRoomHandler : AMActorHandler<User, Actor_GamerExitRoom>
	{
	    protected override async Task Run(User user, Actor_GamerExitRoom message)
	    {
	        Log.Info("Gate收到Map");
            user.ActorID = 0;
	        await Task.CompletedTask;
	    }
	}
}