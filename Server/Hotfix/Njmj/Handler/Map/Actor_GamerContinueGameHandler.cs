﻿using System;
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
            await GamerContinue(gamer);
        }

        public static async Task GamerContinue(Gamer gamer)
        {
            try
            {
                Log.Info($"玩家{gamer.UserID}继续游戏");

                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                if (room == null)
                {
                    return;
                }

                GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
                OrderControllerComponent orderControllerComponent = room.GetComponent<OrderControllerComponent>();

                gamer.ReadyTimeOut = 0;
                List<GamerInfo> Gamers = new List<GamerInfo>();
                for (int i = 0; i < room.GetAll().Length; i++)
                {
                    Gamer _gamer = room.GetAll()[i];
                    if (_gamer == null) continue;
                    GamerInfo gamerInfo = new GamerInfo();
                    gamerInfo.UserID = _gamer.UserID;
                    gamerInfo.SeatIndex = room.GetGamerSeat(_gamer.UserID);
                    gamerInfo.IsReady = _gamer.IsReady;
                    PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(gamerInfo.UserID);

                    //判断金币是否不够
                    if (!room.IsFriendRoom)
                    {
                        if (playerBaseInfo.GoldNum < gameControllerComponent.RoomConfig.MinThreshold)
                        {
                            room.GamerBroadcast(_gamer, new Actor_GamerReadyTimeOut()
                            {
                                Message = "金币不足"
                            });
                            room.Remove(_gamer.UserID);
                            _gamer.Dispose();
                            if (room.Count == 0)
                            {
                                GameHelp.RoomDispose(room);
                                return;
                            }
                            continue;
                        }
                    }

                    PlayerInfo playerInfo = PlayerInfoFactory.Create(playerBaseInfo);
                    gamerInfo.playerInfo = playerInfo;

                    Gamers.Add(gamerInfo);
                }

                Actor_GamerEnterRoom actorGamerEnterRoom = new Actor_GamerEnterRoom()
                {
                    RoomType = (int) gameControllerComponent.RoomConfig.Id,
                    Gamers = Gamers
                };
                if (room.IsFriendRoom)
                {
                    actorGamerEnterRoom.RoomId = gameControllerComponent.RoomConfig.FriendRoomId;
                    actorGamerEnterRoom.MasterUserId = gameControllerComponent.RoomConfig.MasterUserId;
                    actorGamerEnterRoom.JuCount = gameControllerComponent.RoomConfig.JuCount;
                    actorGamerEnterRoom.Multiples = gameControllerComponent.RoomConfig.Multiples;
                    actorGamerEnterRoom.CurrentJuCount = room.CurrentJuCount;
                }

                room.Broadcast(actorGamerEnterRoom);

//                if (room.IsFriendRoom)
//                {
                await Actor_GamerReadyHandler.GamerReady(gamer, new Actor_GamerReady() { });
//                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await Task.CompletedTask;
        }
    }
}