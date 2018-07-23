using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerKickOffHandler : AMActorHandler<Gamer, Actor_GamerKickOff>
	{
	    protected override async Task Run(Gamer gamer, Actor_GamerKickOff message)
        {
	        try
	        {
	            Log.Info($"收到房主{gamer.UserID}踢人 {message.KickedUserId}");
	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);
                if (room == null || room.State == RoomState.Game || !room.IsFriendRoom )
	            {
	                return;
	            }

	            long masterUserId = room.GetComponent<GameControllerComponent>().RoomConfig.MasterUserId;

	            if (masterUserId != gamer.UserID)
	            {
	                Log.Error("只有房主才能踢人");
	                return;
	            }

	            Gamer kickedGamer = room.Get(message.KickedUserId);
	            if (kickedGamer == null) return;

	            room.GamerBroadcast(kickedGamer, new Actor_GamerReadyTimeOut()
	            {
	                Message = "被房主踢出"
	            });

	            //房间移除玩家
	            Log.Info($"{kickedGamer.UserID}被房主踢出,移除玩家");
	            room.Remove(kickedGamer.UserID);
	            //消息广播给其他人
	            room.Broadcast(new Actor_GamerExitRoom() { Uid = kickedGamer.UserID });
	            kickedGamer.Dispose();
            }
	        catch (Exception e)
	        {
	            Log.Error(e);
	        }

	        await Task.CompletedTask;
	    }
	}
}