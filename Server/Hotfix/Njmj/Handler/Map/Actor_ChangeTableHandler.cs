using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_ChangeTableHandler : AMActorHandler<Gamer, Actor_ChangeTable>
    {
        protected override async Task Run(Gamer gamer, Actor_ChangeTable message)
        {
            await ChangeTable(gamer, message);
        }

        private static async Task ChangeTable(Gamer gamer, Actor_ChangeTable message)
        {
            try
            {
//                Log.Info("收到换桌:" + JsonHelper.ToJson(message));
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room gamerRoom = roomComponent.Get(gamer.RoomID);
                if (gamerRoom.State == RoomState.Game)
                {
                    Log.Warning($"玩家{gamer.UserID}打牌过程中不能换桌");
                    return;
                }

                GameControllerComponent gameControllerComponent = gamerRoom.GetComponent<GameControllerComponent>();
                long roomType = gameControllerComponent.RoomConfig.Id;

                Room idleRoom = null;
                //获得空闲的房间
                foreach (var room in roomComponent.idleRooms.Values)
                {
                    if (room.Id != gamerRoom?.Id && room.Count < 4)
                    {
                        GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
                        if (controllerComponent.RoomConfig.Id == roomType)
                        {
                            idleRoom = room;
                            break;
                        }
                    }
                }

                if (idleRoom == null)
                {
                    idleRoom = RoomFactory.Create((int) roomType);
                    roomComponent.Add(idleRoom);
                }

                Log.Info($"收到玩家{gamer.UserID}换桌前：" + gamer.RoomID);
                //房间移除玩家
                gamerRoom?.Remove(gamer.UserID);
                //房间没人就释放
                if (gamerRoom?.seats.Count == 0)
                {
                    roomComponent.RemoveRoom(gamerRoom);
                    gamerRoom?.Dispose();
                }

                gamer.IsReady = false;
                gamer.ReadyTimeOut = 0;
                idleRoom.Add(gamer);
                idleRoom.BroadGamerEnter(gamer.UserID);
                Log.Info($"收到玩家{gamer.UserID}换桌后：" + gamer.RoomID);

                gamerRoom?.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            await Task.CompletedTask;
        }
    }
}