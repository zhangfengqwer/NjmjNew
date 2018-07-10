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
                gamer.IsTrusteeship = false;
                room.StartTime();

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