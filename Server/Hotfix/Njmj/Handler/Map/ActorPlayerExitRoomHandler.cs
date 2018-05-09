using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class ActorPlayerExitRoomHandler : AMActorRpcHandler<Gamer,C2M_ActorGamerExitRoom,M2C_ActorGamerExitRoom>
	{
	    protected override async Task Run(Gamer gamer, C2M_ActorGamerExitRoom message, Action<M2C_ActorGamerExitRoom> reply)
	    {
	        await Task.CompletedTask;
	        M2C_ActorGamerExitRoom response = new M2C_ActorGamerExitRoom();
            try
            {
	            Log.Info("收到退出房间："+JsonHelper.ToJson(message));
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);

                room.Broadcast(new Actor_GamerExitRoom()
                {
                    Uid = gamer.UserID
                });

                room.Remove(gamer.UserID);
                gamer.Dispose();
                reply(response);
            }
            catch (Exception e)
	        {
	            ReplyError(response, e, reply);
            }
	    }
	}
}