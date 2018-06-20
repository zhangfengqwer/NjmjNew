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
        private static bool locker = true;
        private static System.Object lockerer = new System.Object();

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

                Room idleRoom = null;
                //获得空闲的房间
                foreach (var room in roomComponent.idleRooms.Values)
                {
                    if (room.Id != gamerRoom?.Id & room.Count < 4)
                    {
                        GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
                        if (controllerComponent?.RoomConfig.Id == message.RoomType)
                        {
                            idleRoom = room;
                            break;
                        }
                    }
                }

                if (idleRoom == null)
                {
                    idleRoom = RoomFactory.Create(message.RoomType);
                    roomComponent.Add(idleRoom);
                }

                Log.Info("收到玩家换桌前：" + gamer.RoomID);
                //房间移除玩家
                gamerRoom?.Remove(gamer.UserID);
//                if (gamerRoom?.seats.Count == 3)
//                {
//                    roomComponent.idleRooms.Add(gamerRoom.Id, gamerRoom);
//                    roomComponent.readyRooms.Remove(gamerRoom.Id);
//                }
                //房间没人就释放
                if (gamerRoom?.seats.Count == 0)
                {
                    roomComponent.RemoveRoom(gamerRoom);
                    gamerRoom?.Dispose();
                }

                gamer.IsReady = false;
                gamer.ReadyTimeOut = 0;
                idleRoom.Add(gamer);
                idleRoom.BroadGamerEnter(message.RoomType);

//                if (idleRoom.seats.Count == 4)
//                {
//                    roomComponent.readyRooms.Add(idleRoom.Id, idleRoom);
//                    roomComponent.idleRooms.Remove(idleRoom.Id);
//                }

                Log.Info("收到玩家换桌后：" + gamer.RoomID);
                //消息广播给其他人
//                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(200);
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