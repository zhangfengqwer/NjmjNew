using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETModel
{
    public class RoomComponent : ComponentWithId
    {
        //所有房间列表
        public readonly Dictionary<long, Room> rooms = new Dictionary<long, Room>();
        
        //游戏中房间列表
        public readonly Dictionary<long, Room> gameRooms = new Dictionary<long, Room>();

        //等待中房间列表（满4个）
        public readonly Dictionary<long, Room> readyRooms = new Dictionary<long, Room>();

        //空闲房间列表(不满4个)
        public readonly List<Room> idleRooms = new List<Room>();

        //房间总数
        public int TotalCount { get { return this.rooms.Count; } }

        //游戏中房间数
        public int GameRoomCount { get { return gameRooms.Count; } }

        //等待中房间数
        public int ReadyRoomCount { get { return readyRooms.Where(p => p.Value.Count < 4).Count(); } }

        //空闲房间数
        public int IdleRoomCount { get { return idleRooms.Count; } }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (var room in this.rooms.Values)
            {
                room.Dispose();
            }
        }


    }
}
