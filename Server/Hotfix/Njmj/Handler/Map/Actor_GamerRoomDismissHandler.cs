using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerApplyRoomDismissHandler : AMActorHandler<Gamer, Actor_GamerApplyRoomDismiss>
	{

	    protected override async Task Run(Gamer gamer, Actor_GamerApplyRoomDismiss message)
	    {
            try
            {
	            Log.Info($"玩家{gamer.UserID}申请解散房间");
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                if (room == null) return;
                if (!room.IsFriendRoom) return;

                FriendComponent friendComponent = room.GetComponent<FriendComponent>();
                if (room.State == RoomState.Idle)
                {
                    if (gamer.UserID != friendComponent.MasterUserId)
                    {
                        Log.Warning($"准备阶段只有房主才能解散，房主:{friendComponent.MasterUserId},gamer：{gamer.UserID}");
                        return;
                    }
                }

                room.Broadcast(new Actor_GamerApplyRoomDismiss());

            }
	        catch (Exception e)
	        {
	            Log.Error(e);
            }

	        await Task.CompletedTask;
        }
    }
}