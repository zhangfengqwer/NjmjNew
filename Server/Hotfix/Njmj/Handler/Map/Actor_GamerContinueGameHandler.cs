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
                if (room == null)
                {
                    return;
                }
                GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
                OrderControllerComponent orderControllerComponent = room.GetComponent<OrderControllerComponent>();

                gamer.ReadyTimeOut = 0;
//                gamer.IsReady = true;
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

//                    //判断金币是否不够
//                    if (playerBaseInfo.GoldNum < gameControllerComponent.RoomConfig.MinThreshold)
//                    {
//                        room.GamerBroadcast(_gamer, new Actor_GamerReadyTimeOut()
//                        {
//                            Message = "金币不足"
//                        });
//                        room.Remove(_gamer.UserID);
//                        continue;
//                    }

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
                }
                room.Broadcast(actorGamerEnterRoom);

                if (room.IsFriendRoom)
                {
                    Actor_GamerReadyHandler.GamerReady(gamer, new Actor_GamerReady() { });
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await Task.CompletedTask;
        }
    }
}