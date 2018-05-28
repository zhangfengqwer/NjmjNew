using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

            Logic_NJMJ.getInstance().FaMahjong(mahjongInfos1, mahjongInfos2, mahjongInfos3, mahjongInfos4,
                deskComponent.RestLibrary);

            foreach (var card in deskComponent.RestLibrary)
            {
                card.weight = (byte) card.m_weight;
            }
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="self"></param>
        /// <param name="huaCount"></param>
        public static async void GameOver(this GameControllerComponent self, int huaCount)
        {
            Room room = self.GetParent<Room>();
            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
            room.IsGameOver = true;
            room.State = RoomState.Ready;
            room.tokenSource.Cancel();
            self.Multiples = 100;
            //自摸

            Log.Debug("改变财富:" + huaCount * self.Multiples);
            await ChangeWeath(self, huaCount, room);
           

            foreach (var gamer in room.GetAll())
            {
                if (gamer == null)
                    continue;
                gamer.IsReady = false;
                gamer.ReadyTimeOut = 0;
                gamer.isGangFaWanPai = false;
                gamer.isFaWanPaiTingPai = false;
                gamer.isGangEndBuPai = false;
                gamer.isGetYingHuaBuPai = false;
                gamer.IsCanPeng = false;
                gamer.IsCanGang = false;
                gamer.IsCanHu = false;
                gamer.IsWinner = false;
                if (gamer.isOffline)
                {
                    //人满了
                    if (room.seats.Count == 4)
                    {
                        roomComponent.readyRooms.Remove(room.Id);
                        roomComponent.idleRooms.Add(room);
                    }

                    room.Remove(gamer.UserID);
                    gamer.isOffline = !gamer.isOffline;
                }

                //传数据
                //完成一局游戏
                UpdateTask();
            }

            //完成一局游戏
            foreach (var gamer in room.GetAll())
            {
                DBCommonUtil.UpdateChengjiu(gamer.UserID, 101,1);
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
        }

        private static void UpdateTask(Room room)
        {
            //101  新的征程	完成一局游戏	100	1
            foreach (var gamer in room.GetAll())
            {
                //胜利
                if (gamer.UserID == room.ziMoUid)
                {

                }

                DBCommonUtil.UpdateTask(gamer.UserID, 100, 1);
            }


        }


        /// <summary>
        /// 改变玩家财富
        /// </summary>
        /// <param name="self"></param>
        /// <param name="huaCount"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        private static async Task ChangeWeath(GameControllerComponent self, int huaCount, Room room)
        {
            if (huaCount > 0)
            {
                //改变财富
                foreach (var gamer in room.GetAll())
                {
                    if (gamer.UserID == room.ziMoUid)
                    {
                        await DBCommonUtil.ChangeWealth(gamer.UserID, 1, huaCount * self.Multiples);
                    }
                    else
                    {
                        if (room.fangPaoUid != 0)
                        {
                            if (gamer.UserID == room.fangPaoUid)
                            {
                                await DBCommonUtil.ChangeWealth(gamer.UserID, 1, -huaCount * self.Multiples);
                            }
                        }
                        else
                        {
                            await DBCommonUtil.ChangeWealth(gamer.UserID, 1, -huaCount * self.Multiples);
                        }
                    }
                }
            }
        }
    }
}