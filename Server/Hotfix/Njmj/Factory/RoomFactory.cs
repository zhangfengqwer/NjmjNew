using System;
using ETModel;

namespace ETHotfix
{
    public static class RoomFactory
    {
        /// <summary>
        /// 创建Room对象
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public static Room Create(int roomType)
        {
            try
            {
                Room idleRoom = ComponentFactory.Create<Room>();
                idleRoom.AddComponent<DeskComponent>();
                idleRoom.AddComponent<OrderControllerComponent>();
                idleRoom.IsFriendRoom = false;
                GameControllerComponent controllerComponent = idleRoom.AddComponent<GameControllerComponent>();
                controllerComponent.RoomConfig = ConfigHelp.Get<RoomConfig>(roomType);
                controllerComponent.RoomName = (RoomName)roomType;

                Log.Debug("创建房间：" + JsonHelper.ToJson(controllerComponent.RoomConfig));
                return idleRoom;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return null;
        }

        public static Room CreateFriendRoom(G2M_CreateFriendRoom message)
        {
            try
            {
                FriendRoomInfo friendRoomInfo = message.FriendRoomInfo;
                Room room = ComponentFactory.Create<Room>();
                room.AddComponent<DeskComponent>();
                room.AddComponent<OrderControllerComponent>();
                room.AddComponent<GameControllerComponent>();
                room.IsFriendRoom = true;

                GameControllerComponent controllerComponent = room.AddComponent<GameControllerComponent>();
                RoomConfig roomConfig = new RoomConfig();
                roomConfig.FriendRoomId = RandomHelper.RandomNumber(100000,1000000);
                roomConfig.JuCount = friendRoomInfo.Ju;
                roomConfig.Multiples = friendRoomInfo.Hua;
                roomConfig.MinThreshold = 500;
                roomConfig.IsPublic = friendRoomInfo.IsPublic == 1;
                roomConfig.MasterUserId = message.UserId;
                roomConfig.Id = 3;
                controllerComponent.RoomConfig = roomConfig;
                controllerComponent.RoomName = RoomName.Friend;

                return room;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return null;
        }
    }
}
