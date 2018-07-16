using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_GetRoomInfoHandler : AMRpcHandler<G2M_GetRoomInfo, M2G_GetRoomInfo>
	{
		protected override async void Run(Session session, G2M_GetRoomInfo message, Action<M2G_GetRoomInfo> reply)
		{
		    M2G_GetRoomInfo response = new M2G_GetRoomInfo();
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                int newRoomCount = 0;
                int jingRoomCount = 0;

                foreach (var gameRoom in roomComponent.gameRooms.Values)
                {
                    GameControllerComponent gameControllerComponent = gameRoom.GetComponent<GameControllerComponent>();
                    if (gameControllerComponent == null)
                    {
                        continue;
                    }
                    RoomConfig roomConfig = gameControllerComponent.RoomConfig;
                    if (roomConfig.Id == 1)
                    {
                        newRoomCount++;
                    }
                    else if(roomConfig.Id == 2)
                    {
                        jingRoomCount++;
                    }
                }

                int newGamerCount = 0;
                int jingGamerCount = 0;
                foreach (var gameRoom in roomComponent.idleRooms.Values)
                {
                    GameControllerComponent gameControllerComponent = gameRoom.GetComponent<GameControllerComponent>();
                    if (gameControllerComponent == null)
                    {
                        continue;
                    }
                    RoomConfig roomConfig = gameControllerComponent.RoomConfig;
                    if (roomConfig.Id == 1)
                    {
                        newGamerCount += gameRoom.seats.Count;
                    }
                    else if (roomConfig.Id == 2)
                    {
                        jingGamerCount += gameRoom.seats.Count;
                    }
                }

                response.JingRoomCount = jingRoomCount * 4;
                response.NewRoomCount = newRoomCount * 4;
                response.JingTotalPlayerInGameCount = jingRoomCount * 4 + jingGamerCount;
                response.NewTotalPlayerInGameCount = newRoomCount * 4 + newGamerCount;

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