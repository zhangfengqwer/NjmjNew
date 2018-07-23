using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerCancelTrusteeshipHandler : AMActorHandler<Gamer, Actor_GamerCancelTrusteeship>
	{
	    protected override async Task Run(Gamer gamer, Actor_GamerCancelTrusteeship message)
	    {
	        Log.Info($"玩家{gamer.UserID}取消托管");
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                if (room == null)
                {
                    return;
                }
                gamer.IsTrusteeship = false;
                //在自己出牌的时候取消托管
                OrderControllerComponent orderControllerComponent = room.GetComponent<OrderControllerComponent>();
                //当前出牌是自己，并且外面没有碰刚
                Log.Info($"IsNeedWaitOperate:{room.IsNeedWaitOperate}");
                if (orderControllerComponent.CurrentAuthority == gamer.UserID && !room.IsNeedWaitOperate)
                {
                    room.StartTime(8);
                }
                room.Broadcast(new Actor_GamerCancelTrusteeship()
                {
                    Uid = gamer.UserID
                });
            }
            catch (Exception e)
	        {
	            Log.Error(e);
	        }
	        await Task.CompletedTask;
        }
	}
}