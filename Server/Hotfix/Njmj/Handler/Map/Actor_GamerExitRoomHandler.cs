using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerExitRoomHandler : AMActorRpcHandler<Gamer,C2M_ActorGamerExitRoom,M2C_ActorGamerExitRoom>
	{

	    protected override async Task Run(Gamer gamer, C2M_ActorGamerExitRoom message, Action<M2C_ActorGamerExitRoom> reply)
	    {
	        M2C_ActorGamerExitRoom response = new M2C_ActorGamerExitRoom();

            try
	        {
	            Log.Info("收到退出房间");
	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);

	            //人满了
	            if (room.seats.Count == 4)
	            {
	                roomComponent.readyRooms.Remove(room.Id);
	                roomComponent.idleRooms.Add(room);
                }

                //房间移除玩家
                room.Remove(gamer.UserID);
	            //消息广播给其他人
	            room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
	            Log.Info($"玩家{gamer.UserID}退出房间");
	            //通知gate
	            ActorProxyComponent proxyComponent = Game.Scene.GetComponent<ActorProxyComponent>();
	            ActorProxy actorProxy = proxyComponent.Get(gamer.PlayerID);
	            actorProxy.Send(new Actor_GamerExitRoom());
                gamer.Dispose();

	            if (room.seats.Count == 0)
	            {
	                room.Dispose();
	            }

                reply(response);
	        }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
            }

	        await Task.CompletedTask;

	    }
	}
}