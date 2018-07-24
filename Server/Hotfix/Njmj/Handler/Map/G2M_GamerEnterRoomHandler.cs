using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_GamerEnterRoomHandler : AMRpcHandler<G2M_PlayerEnterRoom, M2G_PlayerEnterRoom>
	{
		protected override async void Run(Session session, G2M_PlayerEnterRoom message, Action<M2G_PlayerEnterRoom> reply)
		{
		    M2G_PlayerEnterRoom response = new M2G_PlayerEnterRoom();
            try
			{
			    RoomComponent roomCompnent = Game.Scene.GetComponent<RoomComponent>();
			    Gamer gamer = null;
                Room room = null;

			    foreach (var _room in roomCompnent.gameRooms.Values)
			    {
			        room = _room;
			        gamer = room.Get(message.UserId);
			        if (gamer != null)
			        {
			            Log.Info("找到房间:" + _room.Id);
                        break;
			        }
                }
                
                //断线重连
			    if (gamer != null)
			    {
			        DeskComponent deskComponent = room.GetComponent<DeskComponent>();

                    //重新更新actor
			        gamer.PlayerID = message.PlayerId;
			        gamer.GetComponent<UnitGateComponent>().GateSessionActorId = message.SessionId;

                    //短线重连
			        Actor_GamerReconnet reconnet = new Actor_GamerReconnet();
			        foreach (var _gamer in room.GetAll())
			        {
			            if (_gamer == null)
			            {
			                Log.Error($"断线重连后玩家为空");
                            continue;
			            }
			            GamerData gamerData = new GamerData();

                        HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
			            if (handCardsComponent == null)
			            {
			                Log.Error($"{_gamer.UserID}断线重连后玩家的手牌为空,移除玩家");
			                continue;
//                            room.Remove(_gamer.UserID);
//			                //房间没人就释放
//			                if (room.seats.Count == 0)
//			                {
//                                roomCompnent.RemoveRoom(room);
//			                    room.Dispose();
//			                }
//                            return;
			            }
			            List<MahjongInfo> handCards = handCardsComponent.GetAll();

			            gamerData.handCards = handCards;
			            gamerData.faceCards = handCardsComponent.FaceCards;
			            gamerData.playCards = handCardsComponent.PlayCards;

                        //添加碰刚的uid
			            foreach (var pengOrBar in handCardsComponent.PengOrBars)
			            {
                            //碰
			                if (pengOrBar.OperateType == OperateType.Peng)
			                {
			                    gamerData.pengCards.Add(new MahjongInfo() {weight = (byte) pengOrBar.Weight});
			                    gamerData.OperatedPengUserIds.Add(pengOrBar.UserId);
                            }
                            //杠
			                else
			                {
			                    gamerData.gangCards.Add(new MahjongInfo() { weight = (byte)pengOrBar.Weight });
			                    gamerData.OperatedGangUserIds.Add(pengOrBar.UserId);
                            }
			            }
			          
			            gamerData.IsBanker = handCardsComponent.IsBanker;
			            gamerData.UserID = _gamer.UserID;
			            gamerData.SeatIndex = room.GetGamerSeat(_gamer.UserID);
			            gamerData.OnlineSeconds = await DBCommonUtil.GetRestOnlineSeconds(_gamer.UserID);
			            gamerData.IsTrusteeship = gamer.IsTrusteeship;
                        PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(_gamer.UserID);

			            PlayerInfo playerInfo = PlayerInfoFactory.Create(playerBaseInfo);

			            gamerData.playerInfo = playerInfo;

                        reconnet.Gamers.Add(gamerData);
                    }

			        reconnet.RestCount = deskComponent.RestLibrary.Count;
			        reconnet.RoomType = (int)room.GetComponent<GameControllerComponent>().RoomConfig.Id;
			        if (room.IsFriendRoom)
			        {
			            GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
			            reconnet.RoomId = gameControllerComponent.RoomConfig.FriendRoomId;
			            reconnet.MasterUserId = gameControllerComponent.RoomConfig.MasterUserId;
			            reconnet.JuCount = gameControllerComponent.RoomConfig.JuCount;
			            reconnet.Multiples = gameControllerComponent.RoomConfig.Multiples;
			            reconnet.CurrentJuCount = room.CurrentJuCount;
			        }
                    room.GamerReconnect(gamer, reconnet);

                    //等待客户端重连
                    //await Game.Scene.GetComponent<TimerComponent>().WaitAsync(2000);

			        gamer.isOffline = false;
			        gamer.RemoveComponent<TrusteeshipComponent>();
			        Log.Info($"玩家{message.UserId}断线重连");

                    gamer.StartTime = DateTime.Now;
			        //DBCommonUtil.RecordGamerTime(gamer.EndTime, false, gamer.UserID);
                }
                else
			    {
			        Log.Info($"{message.UserId}进入房间");

                    gamer = GamerFactory.Create(message.PlayerId, message.UserId);
			        await gamer.AddComponent<MailBoxComponent>().AddLocation();
			        gamer.AddComponent<UnitGateComponent, long>(message.SessionId);

			        RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                    //获得空闲的房间
			        Room idleRoom;

                    if (message.RoomType == 3)
			        {
			            idleRoom = roomComponent.GetFriendRoomById(message.RoomId);
			            if (idleRoom == null)
			            {
			                response.Error = ErrorCode.ERR_Common;
			                response.Message = "房间号不存在";
                            reply(response);
			                return;
			            }

			            if (idleRoom.Count == 4)
			            {
			                response.Error = ErrorCode.ERR_Common;
			                response.Message = "房间人数已满";
			                reply(response);
			                return;
                        }
                    }
			        else
			        {
			            idleRoom = roomComponent.GetIdleRoomById(message.RoomType);
			            if (idleRoom == null)
			            {
			                idleRoom = RoomFactory.Create(message.RoomType);
			                roomComponent.Add(idleRoom);
			            }
                    }
			        idleRoom.Add(gamer);

			        idleRoom.BroadGamerEnter(gamer.UserID);
                }
			    response.GameId = gamer.Id;
			    reply(response);

			    if (message.RoomType == 3)
			    {
			        await Actor_GamerReadyHandler.GamerReady(gamer, new Actor_GamerReady() { });
			    }
            }
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}

		    await Task.CompletedTask;
        }
	}
}