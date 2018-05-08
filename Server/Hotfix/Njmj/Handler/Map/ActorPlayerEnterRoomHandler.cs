using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class ActorPlayerEnterRoomHandler : AMActorRpcHandler<Gamer,C2M_ActorGamerEnterRoom,M2C_ActorGamerEnterRoom>
	{
	    protected override async Task Run(Gamer gamer, C2M_ActorGamerEnterRoom message, Action<M2C_ActorGamerEnterRoom> reply)
	    {
	        await Task.CompletedTask;

	        Log.Info(JsonHelper.ToJson(message));

	        M2C_ActorGamerEnterRoom response = new M2C_ActorGamerEnterRoom();

	        reply(response);
	    }
	}
}