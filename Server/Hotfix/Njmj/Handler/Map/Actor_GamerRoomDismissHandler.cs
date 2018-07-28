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

                GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
                if (room.State == RoomState.Idle)
                {
                    if (gamer.UserID != gameControllerComponent.RoomConfig.MasterUserId)
                    {
                        Log.Warning($"准备阶段只有房主才能解散，房主:{gameControllerComponent.RoomConfig.MasterUserId},gamer：{gamer.UserID}");
                        return;
                    }
                    else
                    {
                        if (room.CurrentJuCount > 0)
                        {
                            return;
                        }
                        room.Broadcast(new Actor_GamerReadyTimeOut()
                        {
                            Message = "房主解散房间"
                        });
                        GameHelp.RoomDispose(room);
                        return;
                    }
                }

                room.Broadcast(new Actor_GamerApplyRoomDismiss());
                room.WaitDismiss(60);
            }
	        catch (Exception e)
	        {
	            Log.Error(e);
            }

	        await Task.CompletedTask;
        }
    }
}