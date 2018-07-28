using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoomComponentSystemEx : AwakeSystem<RoomComponent>
    {
        public override void Awake(RoomComponent self)
        {
            self.Awake();
        }
    }   

    public static class RoomComponentSystem
    {
        /// <summary>
        /// 根据房间id获取房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Room Get(this RoomComponent self, long id)
        {
            Room room;
            self.rooms.TryGetValue(id, out room);
            return room;
        }

        public static void Add(this RoomComponent self, Room room)
        {
            self.rooms.Add(room.Id, room);
            self.idleRooms.Add(room.Id,room);
        }

        /// <summary>
        /// 从空闲集合中随机返回一个列表
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Room GetIdleRoom(this RoomComponent self)
        {
            if (self.IdleRoomCount > 0)
            {
                Room room = self.idleRooms[RandomHelper.RandomNumber(0, self.idleRooms.Count)];
                return room;
            }
            else
            {
                return null;
            }
        }

        public static Room GetIdleRoomById(this RoomComponent self, int roomType)
        {
            if (self.IdleRoomCount > 0)
            {
                foreach (var room in self.idleRooms.Values)
                {
                    if (!room.IsFriendRoom)
                    {
                        GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
                        if (controllerComponent == null)
                        {
                            Log.Warning("room的GameControllerComponent为null");
                            continue;
                        }
                        if (controllerComponent.RoomConfig.Id == roomType && room.Count < 3)
                        {
                            return room;
                        }
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据房间号获得房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static Room GetFriendRoomById(this RoomComponent self, int roomId)
        {
            foreach (var room in self.rooms.Values)
            {
                if (!room.IsFriendRoom) continue;
                GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
                if (gameControllerComponent.RoomConfig.FriendRoomId == roomId)
                {
                    return room;
                }
            }
            return null;
        }

        public static void RemoveRoom(this RoomComponent self,Room room,bool isTimeout = false)
        {
            self.rooms.Remove(room.Id);
            Log.Info("self.rooms:" + self.rooms.Count);
            self.idleRooms.Remove(room.Id);
            Log.Info("idleRooms:" + self.idleRooms.Count);
            self.gameRooms.Remove(room.Id);
            Log.Info("gameRooms:" + self.gameRooms.Count);
        }

        public static async void Awake(this RoomComponent self)
        {
            while (true)
            {
                try
                {
                    await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                    if (self.IsDisposed)
                    {
                        return;
                    }

                    List<Room> rooms = self.idleRooms.Values.ToList();
                    foreach (var room in rooms)
                    {
                        //好友房不提出
                        if (room.IsFriendRoom) continue;
                        foreach (var gamer in room.GetAll())
                        {
                            if (gamer == null)
                                continue;
                            if (!gamer.IsReady)
                            {
                                gamer.ReadyTimeOut++;

                                //超时
                                if (gamer.ReadyTimeOut > 20)
                                {
                                    room.GamerBroadcast(gamer, new Actor_GamerReadyTimeOut()
                                    {
                                        Message = "超时未准备，被踢出"
                                    });

                                    //房间移除玩家
                                    Log.Info($"{gamer.UserID}超时,移除玩家");
                                    room.Remove(gamer.UserID);
                                    //消息广播给其他人
                                    room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
                                    gamer.Dispose();
                                    //房间没人就释放
                                    if (room.seats.Count == 0)
                                    {
                                        Log.Debug($"房间释放:{room.Id}");
                                        self.RemoveRoom(room);
                                        room?.Dispose();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}