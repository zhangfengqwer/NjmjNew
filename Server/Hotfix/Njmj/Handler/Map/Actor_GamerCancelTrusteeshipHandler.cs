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
                await Task.CompletedTask;
            }
            catch (Exception e)
	        {
	            Log.Error(e);
	        }
	    }
	}
}