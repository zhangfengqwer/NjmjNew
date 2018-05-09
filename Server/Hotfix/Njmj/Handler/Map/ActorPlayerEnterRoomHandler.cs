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
	        Log.Info(JsonHelper.ToJson(message));
	        M2C_ActorGamerEnterRoom response = new M2C_ActorGamerEnterRoom();
	        try
	        {
	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);

	            room.Broadcast(new Actor_GamerExitRoom()
	            {
	                Uid = gamer.UserID
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