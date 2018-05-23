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
            if (self == null)
            {
                Log.Error("当前为null:GameControllerComponent.DealCards");
                return;
            }
            Room room = self.GetParent<Room>();
            Gamer[] gamers = room.GetAll();

            DeskComponent deskComponent = room.GetComponent<DeskComponent>();
            List<MahjongInfo> mahjongInfos1 = gamers[0].GetComponent<HandCardsComponent>().library;
            List<MahjongInfo> mahjongInfos2 = gamers[1].GetComponent<HandCardsComponent>().library;
            List<MahjongInfo> mahjongInfos3 = gamers[2].GetComponent<HandCardsComponent>().library;
            List<MahjongInfo> mahjongInfos4 = gamers[3].GetComponent<HandCardsComponent>().library;

            Logic_NJMJ.getInstance().FaMahjong(mahjongInfos1, mahjongInfos2, mahjongInfos3, mahjongInfos4,deskComponent.RestLibrary);

            foreach (var card in deskComponent.RestLibrary)
            {
                card.weight = (byte) card.m_weight;
            }
        }
        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="self"></param>
        public static void GameOver(this GameControllerComponent self)
        {
            Room room = self.GetParent<Room>();
            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
            room.IsGameOver = true;
            room.State = RoomState.Ready;
            room.tokenSource.Cancel();

            foreach (var gamer in room.GetAll())
            {
                if (gamer == null)
                    continue;
                gamer.IsReady = false;
                gamer.isGangFaWanPai = false;
                gamer.isFaWanPaiTingPai = false;
                gamer.isGangEndBuPai = false;
                gamer.isGetYingHuaBuPai = false;
                gamer.IsCanPeng = false;
                gamer.IsCanGang = false;
                gamer.IsCanHu = false;

                if (gamer.isOffline)
                {
                    room.Remove(gamer.UserID);
                    gamer.isOffline = !gamer.isOffline;
                }
            }

            roomComponent.gameRooms.Remove(room.Id);
            roomComponent.readyRooms.Add(room.Id, room);

            //人不足4人
            if (room.seats.Count < 4)
            {
                roomComponent.readyRooms.Remove(room.Id);
                roomComponent.idleRooms.Add(room);
            }

            //房间没人就释放
            if (room.seats.Count == 0)
            {
                Log.Debug($"房间释放:{room.Id}");
                room?.Dispose();
                roomComponent.RemoveRoom(room);
            }

            //传数据


            
        }
    }
}
