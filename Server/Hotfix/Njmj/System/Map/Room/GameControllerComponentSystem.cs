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
            List<MahjongInfo> mahjongInfos1 = gamers[0].GetComponent<HandCardsComponent>().library;
            List<MahjongInfo> mahjongInfos2 = gamers[1].GetComponent<HandCardsComponent>().library;
            List<MahjongInfo> mahjongInfos3 = gamers[2].GetComponent<HandCardsComponent>().library;
            List<MahjongInfo> mahjongInfos4 = gamers[3].GetComponent<HandCardsComponent>().library;

            Logic_NJMJ.getInstance().FaMahjong(mahjongInfos1, mahjongInfos2, mahjongInfos3, mahjongInfos4,deskComponent.RestLibrary);
        }
    }
}
