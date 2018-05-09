using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public static class GameControllerComponentSystem
    {
        /// <summary>
        /// 发牌
        /// </summary>
        /// <param name="self"></param>
        public static void DealCards(this GameControllerComponent self)
        {
            Room room = self.GetParent<Room>();
            Gamer[] gamers = room.GetAll();
            DeskComponent deskComponent = room.GetComponent<DeskComponent>();

//            Logic_NJMJ.getInstance().FaMahjong(deskComponent.RestLibrary);
        }
    }
}
