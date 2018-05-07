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
			    RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
			    //获得空闲的房间
			    Room idleRoom = roomComponent.GetIdleRoom();
			    if (idleRoom == null)
			    {
			    	idleRoom = ComponentFactory.Create<Room>();
			    	roomComponent.Add(idleRoom);
			    }
			    Gamer gamer = ComponentFactory.CreateWithId<Gamer, long>(IdGenerater.GenerateId(), message.UserId);
			    gamer.PlayerID = message.PlayId;
			    gamer.GateSessionID = message.SessionId;

			    idleRoom.Add(gamer);

                //人满了
			    if (idleRoom.seats.Count == 4)
			    {
			        roomComponent.readyRooms.Add(idleRoom.Id,idleRoom);
			        roomComponent.idleRooms.Remove(idleRoom);
			    }

                foreach (var item in idleRoom.seats)
                {
                    GamerInfo gamerInfo = new GamerInfo();
                    gamerInfo.UserID = item.Key;
                    gamerInfo.SeatIndex = item.Value;
                    Gamer temp = idleRoom.Get(item.Key);
                    gamerInfo.IsReady = temp.IsReady;
                    response.Gamers.Add(gamerInfo);
                }

			    ActorProxyComponent proxyComponent = Game.Scene.GetComponent<ActorProxyComponent>();

			    ActorProxy actorProxy = proxyComponent.Get(message.SessionId);

			    actorProxy.Send(new Actor_GamerEnterRoom()
			    {
                    Gamers = response.Gamers
                });

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