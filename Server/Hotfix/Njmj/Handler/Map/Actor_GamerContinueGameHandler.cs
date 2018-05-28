using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerContinueGameHandler : AMActorHandler<Gamer, Actor_GamerContinueGame>
	{

	    protected override async Task Run(Gamer gamer, Actor_GamerContinueGame message)
	    {
            try
	        {
	            Log.Info($"玩家{gamer.UserID}继续游戏");

	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);
	            gamer.ReadyTimeOut = 0;

                List<GamerInfo> Gamers = new List<GamerInfo>();
	            foreach (var item in room.seats)
	            {
	                GamerInfo gamerInfo = new GamerInfo();
	                gamerInfo.UserID = item.Key;
	                gamerInfo.SeatIndex = item.Value;
	                Gamer temp = room.Get(item.Key);
	                gamerInfo.IsReady = temp.IsReady;
	                Gamers.Add(gamerInfo);
	            }

	            room.Broadcast(new Actor_GamerEnterRoom()
	            {
	                Gamers = Gamers
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