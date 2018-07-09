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
                GameControllerComponent controllerComponent = idleRoom.AddComponent<GameControllerComponent>();
                controllerComponent.RoomConfig = ConfigHelp.Get<RoomConfig>(roomType);
                controllerComponent.RoomName = (RoomName)roomType;

                Log.Debug("创建房间：" + JsonHelper.ToJson(controllerComponent.RoomConfig));

                idleRoom.AddComponent<OrderControllerComponent>();
                return idleRoom;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return null;
        }
    }
}
