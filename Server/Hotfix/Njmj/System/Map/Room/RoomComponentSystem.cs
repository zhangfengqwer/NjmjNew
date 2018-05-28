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
            self.idleRooms.Add(room);
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

        public static void RemoveRoom(this RoomComponent self,Room room)
        {
            self.rooms.Remove(room.Id);
            self.idleRooms.Remove(room);
        }

        public static async void Awake(this RoomComponent self)
        {
            while (true)
            {
                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);

                foreach (var room in self.idleRooms)
                {
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
                                room.GamerBroadcast(gamer, new Actor_GamerReadyTimeOut());

                                //房间移除玩家
                                room.Remove(gamer.UserID);

                                //消息广播给其他人
                                room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
                                gamer.Dispose();
                                //房间没人就释放
                                if (room.seats.Count == 0)
                                {
                                    Log.Debug($"房间释放:{room.Id}");
                                    room?.Dispose();
                                    self.RemoveRoom(room);
                                }
                            }
                        }
                    }
                }

                foreach (var room in self.readyRooms.Values)
                {
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
                                //人满了
                                if (room.seats.Count == 4)
                                {
                                    self.readyRooms.Remove(room.Id);
                                    self.idleRooms.Add(room);
                                }

                                room.GamerBroadcast(gamer, new Actor_GamerReadyTimeOut());

                                //房间移除玩家
                                room.Remove(gamer.UserID);
                                //消息广播给其他人
                                room.Broadcast(new Actor_GamerExitRoom() { Uid = gamer.UserID });
                                gamer.Dispose();
                              
                            }
                        }
                    }
                }
            }
        }
    }
}