using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_GamerEnterRoomHandler : AMRpcHandler<G2M_PlayerEnterRoom, M2G_PlayerEnterRoom>
	{
		protected override async void Run(Session session, G2M_PlayerEnterRoom message, Action<M2G_PlayerEnterRoom> reply)
		{
		    M2G_PlayerEnterRoom response = new M2G_PlayerEnterRoom();
		    Log.Info(JsonHelper.ToJson(message));
            try
			{
			    RoomComponent roomCompnent = Game.Scene.GetComponent<RoomComponent>();
			    Gamer gamer = null;
                Room room = null;
                for (int i = 0; i < roomCompnent.gameRooms.Count; i++)
			    {
			        room = roomCompnent.gameRooms[i];
                    Log.Info("找到房间:");
			        gamer = room.Get(message.UserId);
			        if (gamer != null) break;
			    }
                
                //断线重连
			    if (gamer != null)
			    {
			        gamer.isOffline = false;
			        gamer.RemoveComponent<TrusteeshipComponent>();
			        Log.Info($"玩家{message.UserId}断线重连");
			        room.GamerBroadcast(gamer, new Actor_GamerReconnet());
                }
                else
			    {
			        gamer = GamerFactory.Create(message.PlayerId, message.UserId);
			        await gamer.AddComponent<ActorComponent>().AddLocation();
			        gamer.AddComponent<UnitGateComponent, long>(message.SessionId);

			        RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
			        //获得空闲的房间
			        Room idleRoom = roomComponent.GetIdleRoom();
			        if (idleRoom == null)
			        {
			            idleRoom = RoomFactory.Create();
			            Log.Debug("创建房间：" + idleRoom.Id);

			            roomComponent.Add(idleRoom);
			        }

			        idleRoom.Add(gamer);

			        //人满了
			        if (idleRoom.seats.Count == 4)
			        {
			            roomComponent.readyRooms.Add(idleRoom.Id, idleRoom);
			            roomComponent.idleRooms.Remove(idleRoom);
			        }

			        List<GamerInfo> Gamers = new List<GamerInfo>();
                    GamerInfo currentInfo = null;
			        foreach (var item in idleRoom.seats)
			        {
			            GamerInfo gamerInfo = new GamerInfo();
			            gamerInfo.UserID = item.Key;
			            gamerInfo.SeatIndex = item.Value;
			            Gamer temp = idleRoom.Get(item.Key);
			            gamerInfo.IsReady = temp.IsReady;

			            if (gamerInfo.UserID == message.UserId)
			            {
			                currentInfo = gamerInfo;

			            }
			            Gamers.Add(gamerInfo);
			        }

                    foreach (var _gamer in idleRoom.GetAll())
                    {
                        if (_gamer == null || _gamer.isOffline)
                            continue;

                        //第一次进入
                        if (_gamer.UserID == message.UserId)
                        {
                            idleRoom.GamerBroadcast(_gamer,new Actor_GamerEnterRoom()
                            {
                                Gamers = Gamers
                            });
                        }
                        //有人加入
                        else
                        {
                            idleRoom.GamerBroadcast(_gamer, new Actor_GamerJionRoom()
                            {
                                Gamer = currentInfo
                            });
                        }
                    }
//			        idleRoom.Broadcast(new Actor_GamerEnterRoom()
//			        {
//			            Gamers = Gamers
//			        });
			        Log.Info($"玩家{message.UserId}进入房间");
                }
			    response.GameId = gamer.Id;
			    reply(response);
            }
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}