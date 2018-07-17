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

                FriendComponent friendComponent = room.AddComponent<FriendComponent>();
                friendComponent.FriendRoomId = 12343;
                friendComponent.JuCount = friendRoomInfo.Ju;
                friendComponent.Multiples = friendRoomInfo.Hua;
                friendComponent.MinThreshold = 500;

                friendComponent.MasterUserId = message.UserId;


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
