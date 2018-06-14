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
                for (int i = 0; i < room.GetAll().Length; i++)
                {
                    Gamer _gamer = room.GetAll()[i];
                    if (_gamer == null) continue;
                    GamerInfo gamerInfo = new GamerInfo();
                    gamerInfo.UserID = _gamer.UserID;
                    gamerInfo.SeatIndex = room.GetGamerSeat(_gamer.UserID);
                    gamerInfo.IsReady = _gamer.IsReady;
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