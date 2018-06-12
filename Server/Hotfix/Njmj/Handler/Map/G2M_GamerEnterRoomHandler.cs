﻿using System;
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

			    foreach (var _room in roomCompnent.gameRooms.Values)
			    {
			        room = _room;
			        Log.Info("找到房间:"+ _room.Id);
			        gamer = room.Get(message.UserId);
			        if (gamer != null) break;
                }
                
                //断线重连
			    if (gamer != null)
			    {
			        DeskComponent deskComponent = room.GetComponent<DeskComponent>();


                    //重新更新actor
			        gamer.PlayerID = message.PlayerId;
			        gamer.GetComponent<UnitGateComponent>().GateSessionId = message.SessionId;

                    //短线重连
			        Actor_GamerReconnet reconnet = new Actor_GamerReconnet();
			        foreach (var _gamer in room.GetAll())
			        {
			            GamerData gamerData = new GamerData();

                        HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
			            List<MahjongInfo> handCards = handCardsComponent.GetAll();

			            gamerData.handCards = handCards;
			            gamerData.faceCards = handCardsComponent.FaceCards;
			            gamerData.playCards = handCardsComponent.PlayCards;
			            gamerData.pengCards = handCardsComponent.PengCards;
			            gamerData.gangCards = handCardsComponent.GangCards;
			            gamerData.IsBanker = handCardsComponent.IsBanker;
			            gamerData.UserID = _gamer.UserID;
			            gamerData.SeatIndex = room.GetGamerSeat(_gamer.UserID);
			            gamerData.OnlineSeconds = await DBCommonUtil.GetRestOnlineSeconds(_gamer.UserID);

			            PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(_gamer.UserID);

			            PlayerInfo playerInfo = new PlayerInfo();
			            playerInfo.Icon = playerBaseInfo.Icon;
			            playerInfo.Name = playerBaseInfo.Name;
			            playerInfo.GoldNum = playerBaseInfo.GoldNum;
			            playerInfo.WinGameCount = playerBaseInfo.WinGameCount;
			            playerInfo.TotalGameCount = playerBaseInfo.TotalGameCount;
			            playerInfo.VipTime = playerBaseInfo.VipTime;
			            playerInfo.PlayerSound = playerBaseInfo.PlayerSound;
			            playerInfo.RestChangeNameCount = playerBaseInfo.RestChangeNameCount;
			            playerInfo.EmogiTime = playerBaseInfo.EmogiTime;
			            playerInfo.MaxHua = playerBaseInfo.MaxHua;

			            gamerData.playerInfo = playerInfo;


                        reconnet.Gamers.Add(gamerData);
                    }

			        reconnet.RestCount = deskComponent.RestLibrary.Count;
			        reconnet.RoomType = (int)room.GetComponent<GameControllerComponent>().RoomConfig.Id;
                    room.GamerReconnect(gamer, reconnet);

                    //等待客户端重连
			        await Game.Scene.GetComponent<TimerComponent>().WaitAsync(2000);

			        gamer.isOffline = false;
			        gamer.RemoveComponent<TrusteeshipComponent>();
			        Log.Info($"玩家{message.UserId}断线重连");

                    gamer.StartTime = DateTime.Now;
			        DBCommonUtil.RecordGamerTime(gamer.EndTime, false, gamer.UserID);
                }
                else
			    {
			        gamer = GamerFactory.Create(message.PlayerId, message.UserId);
			        await gamer.AddComponent<ActorComponent>().AddLocation();
			        gamer.AddComponent<UnitGateComponent, long>(message.SessionId);

			        RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
			        //获得空闲的房间
			        Room idleRoom = roomComponent.GetIdleRoomById(message.RoomType);
			        if (idleRoom == null)
			        {
			            idleRoom = RoomFactory.Create(message.RoomType);
			            roomComponent.Add(idleRoom);
			        }

			        idleRoom.Add(gamer);

			        //人满了
			        if (idleRoom.seats.Count == 4)
			        {
                        roomComponent.readyRooms.Add(idleRoom.Id, idleRoom);
                        roomComponent.idleRooms.Remove(idleRoom);
                        //有key重复添加的问题
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

			            PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(gamerInfo.UserID);

			            PlayerInfo playerInfo = new PlayerInfo();
			            playerInfo.Icon = playerBaseInfo.Icon;
			            playerInfo.Name = playerBaseInfo.Name;
			            playerInfo.GoldNum = playerBaseInfo.GoldNum;
			            playerInfo.WinGameCount = playerBaseInfo.WinGameCount;
			            playerInfo.TotalGameCount = playerBaseInfo.TotalGameCount;
			            playerInfo.VipTime = playerBaseInfo.VipTime;
			            playerInfo.PlayerSound = playerBaseInfo.PlayerSound;
			            playerInfo.RestChangeNameCount = playerBaseInfo.RestChangeNameCount;
			            playerInfo.EmogiTime = playerBaseInfo.EmogiTime;
			            playerInfo.MaxHua = playerBaseInfo.MaxHua;

                        gamerInfo.playerInfo = playerInfo;

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
                            Actor_GamerEnterRoom actorGamerEnterRoom = new Actor_GamerEnterRoom()
                            {
                                RoomType = message.RoomType,
                                Gamers = Gamers
                            };
                            idleRoom.GamerBroadcast(_gamer, actorGamerEnterRoom);

                            idleRoom.reconnectList.Add(actorGamerEnterRoom);
                        }
                        //有人加入
                        else
                        {
                            Actor_GamerJionRoom actorGamerJionRoom = new Actor_GamerJionRoom()
                            {
                                Gamer = currentInfo
                            };

                            idleRoom.GamerBroadcast(_gamer, actorGamerJionRoom);

                            idleRoom.reconnectList.Add(actorGamerJionRoom);
                        }
                    }

			        Log.Info($"玩家{message.UserId}进入房间:{idleRoom.Id}");
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