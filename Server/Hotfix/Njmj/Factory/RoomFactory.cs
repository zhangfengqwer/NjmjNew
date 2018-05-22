using ETModel;

namespace ETHotfix
{
    public static class RoomFactory
    {
        /// <summary>
        /// 创建Room对象
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static Room Create()
        {
            Room idleRoom = ComponentFactory.Create<Room>();
            idleRoom.AddComponent<DeskComponent>();
//            idleRoom.AddComponent<DeskCardsCacheComponent>();
            idleRoom.AddComponent<GameControllerComponent>();
            idleRoom.AddComponent<OrderControllerComponent>();
            return idleRoom;
        }
    }
}
