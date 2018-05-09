using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_PlayerEnterRoomHandler : AMRpcHandler<G2M_PlayerEnterRoom, M2G_PlayerEnterRoom>
	{
		protected override async void Run(Session session, G2M_PlayerEnterRoom message, Action<M2G_PlayerEnterRoom> reply)
		{
		    M2G_PlayerEnterRoom response = new M2G_PlayerEnterRoom();
		    Log.Info(JsonHelper.ToJson(message));
            try
			{
			    Gamer gamer = GamerFactory.Create(message.PlayerId, message.UserId);
			    await gamer.AddComponent<ActorComponent>().AddLocation();
			    gamer.AddComponent<UnitGateComponent, long>(message.SessionId);

                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
			    //获得空闲的房间
			    Room idleRoom = roomComponent.GetIdleRoom();
			    if (idleRoom == null)
			    {
			    	idleRoom = ComponentFactory.Create<Room>();
			        idleRoom.AddComponent<DeskComponent>();
//			        idleRoom.AddComponent<DeskCardsCacheComponent>();
			        idleRoom.AddComponent<GameControllerComponent>();
			        idleRoom.AddComponent<OrderControllerComponent>();

                    roomComponent.Add(idleRoom);
			    }

			    idleRoom.Add(gamer);

                //人满了
			    if (idleRoom.seats.Count == 4)
			    {
			        roomComponent.readyRooms.Add(idleRoom.Id,idleRoom);
			        roomComponent.idleRooms.Remove(idleRoom);
			    }

                List<GamerInfo> Gamers = new List<GamerInfo>();
                foreach (var item in idleRoom.seats)
                {
                    GamerInfo gamerInfo = new GamerInfo();
                    gamerInfo.UserID = item.Key;
                    gamerInfo.SeatIndex = item.Value;
                    Gamer temp = idleRoom.Get(item.Key);
                    gamerInfo.IsReady = temp.IsReady;
                    Gamers.Add(gamerInfo);
                }
			    
			    idleRoom.Broadcast(new Actor_GamerEnterRoom()
			    {
			        Gamers = Gamers
			    });

			    Log.Info($"玩家{message.UserId}进入房间");

                response.GameId = gamer.Id;
			    Log.Info(JsonHelper.ToJson(response));
			    reply(response);
            }
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}