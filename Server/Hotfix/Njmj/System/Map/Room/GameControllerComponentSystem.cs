using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    public static class GameControllerComponentSystem
    {
        private static List<List<MahjongInfo>> temp =  new List<List<MahjongInfo>>();
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

            temp.Clear();

            Room room = self.GetParent<Room>();
            Gamer[] gamers = room.GetAll();

            DeskComponent deskComponent = room.GetComponent<DeskComponent>();
            foreach (var gamer in gamers)
            {
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

                //发牌前有拍了
                if (handCardsComponent.GetAll().Count > 0)
                {
                    Log.Debug("发牌前有牌了：" + handCardsComponent.GetAll().Count);
                    temp.Add(null);
                }
                else
                {
                    Log.Debug("发牌前没有拍");
                    temp.Add(handCardsComponent.GetAll());
                }
            }

            Logic_NJMJ.getInstance().FaMahjong(temp, deskComponent.RestLibrary);

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
            DeskComponent deskComponent = room.GetComponent<DeskComponent>();
            GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();

            deskComponent.RestLibrary.Clear();
            if (huaCount == 0)
            {
                //没牌
                room.Broadcast(new Actor_GameFlow());
            }

            room.IsGameOver = true;
            room.State = RoomState.Ready;
            room.tokenSource.Cancel();
            self.Multiples = 100;

            //游戏房间进入准备房间
            roomComponent.gameRooms.Remove(room.Id);
            roomComponent.readyRooms.Add(room.Id, room);

            Log.Debug("改变财富:" + huaCount * controllerComponent.RoomConfig.Multiples);
            await ChangeWeath(self, huaCount, room);

            //更新任务
            UpdateTask(room);
            UpdateChengjiu(room);
            UpdatePlayerInfo(room, huaCount);

            //            //更新任务
            //            UpdateTask(room);

            //设置在线时长
            foreach (var gamer in room.GetAll())
            {
                //在线
                if (!gamer.isOffline)
                {
                    gamer.EndTime = DateTime.Now;
                    TimeSpan span = gamer.EndTime - gamer.StartTime;
                    int totalSeconds = (int)span.TotalSeconds;
                    DBCommonUtil.RecordGamerTime(gamer.EndTime, false, gamer.UserID);
                    DBCommonUtil.RecordGamerInfo(gamer.UserID, totalSeconds);
                }
            }

            foreach (var gamer in room.GetAll())
            {
                if (gamer == null)
                    continue;
                gamer.RemoveComponent<HandCardsComponent>();
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
                //离线踢出
                if (gamer.isOffline)
                {
                    room.Remove(gamer.UserID);
                    gamer.isOffline = !gamer.isOffline;
                }
            }

            //人不足4人,准备进入空闲
            if (room.seats.Count < 4)
            {
                roomComponent.readyRooms.Remove(room.Id);
                roomComponent.idleRooms.Add(room);
            }

            //房间没人就释放
            if (room.seats.Count == 0)
            {
                Log.Debug($"房间释放:{room.Id}");
                roomComponent.RemoveRoom(room);
                room?.Dispose();
            }
        }

        private static void UpdatePlayerInfo(Room room ,int huaCount)
        {
            foreach (var gamer in room.GetAll())
            {
                //胜利
                if (gamer.UserID == room.ziMoUid)
                {
                    DBCommonUtil.UpdatePlayerInfo(gamer.UserID, huaCount, true);
                }
                else
                {
                    DBCommonUtil.UpdatePlayerInfo(gamer.UserID, 0);
                }
            }
        }

        private static void UpdateTask(Room room)
        {
            GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
            foreach (var gamer in room.GetAll())
            {
                //胜利
                if (gamer.UserID == room.ziMoUid)
                {
                    if (controllerComponent.RoomName == RoomName.ChuJi)
                    {
                        Log.Debug("新手场SHENGLI");
                        //	102	新手场	在新手场赢得10场胜利	1000	10
                        DBCommonUtil.UpdateTask(gamer.UserID, 102, 1);
                    }
                    else if (controllerComponent.RoomName == RoomName.JingYing)
                    {
                        Log.Debug("精英场SHENGLI");
                        //	103	精英场	在精英场赢得30场胜利	100000	30
                        DBCommonUtil.UpdateTask(gamer.UserID, 103, 1);
                    }

                    Log.Debug("	连赢5场");
                    //	104	游戏高手	连赢5场	10000	5
                    DBCommonUtil.UpdateTask(gamer.UserID, 104, 1);
                }
                //输了
                else
                {
                    Log.Debug("SHULE");
                    //	104	游戏高手	连赢5场	10000	5
                    DBCommonUtil.UpdateTask(gamer.UserID, 104, -1);
                }

                //101  新的征程	完成一局游戏	100	1
                DBCommonUtil.UpdateTask(gamer.UserID, 101, 1);
            }

        }

        private static void UpdateChengjiu(Room room)
        {
            foreach (var gamer in room.GetAll())
            {
                //胜利
                if (gamer.UserID == room.ziMoUid)
                {
                    Log.Debug("成就胜利");
                    //赢得10局游戏
                    DBCommonUtil.UpdateChengjiu(gamer.UserID, 104, 1);
                    //赢得100局游戏
                    DBCommonUtil.UpdateChengjiu(gamer.UserID, 105, 1);
                    //赢得1000局游戏
                    DBCommonUtil.UpdateChengjiu(gamer.UserID, 106, 1);
                }

                //新手上路 完后10局游戏
                DBCommonUtil.UpdateChengjiu(gamer.UserID, 101, 1);
                //已有小成 完成100局游戏
                DBCommonUtil.UpdateChengjiu(gamer.UserID, 102, 1);
                //完成1000局游戏
                DBCommonUtil.UpdateChengjiu(gamer.UserID, 103, 1);
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
                       
                        await DBCommonUtil.ChangeWealth(gamer.UserID, 1, huaCount * self.RoomConfig.Multiples, self.RoomConfig.Name + "结算");
                        //	105	赚钱高手	当日累计赢取10000金币	10000	10000
                        DBCommonUtil.UpdateTask(gamer.UserID, 105, huaCount * self.RoomConfig.Multiples);
                        // 110 小试身手 单局赢取10000金币满 100局
                        if (huaCount * self.RoomConfig.Multiples >= 10000)
                            DBCommonUtil.UpdateChengjiu(gamer.UserID, 110, 1);
                        // 111 来者不拒 单局赢取100万金币满 100局
                        if (huaCount * self.RoomConfig.Multiples >= 1000000)
                            DBCommonUtil.UpdateChengjiu(gamer.UserID, 111, 1);
                        // 112 富豪克星 单局赢取一亿金币满 100局
                        if (huaCount * self.RoomConfig.Multiples >= 100000000)
                            DBCommonUtil.UpdateChengjiu(gamer.UserID, 112, 1);
                            DBCommonUtil.UpdateChengjiu(gamer.UserID, 112, 1);
                    }
                    else
                    {
                        if (room.fangPaoUid != 0)
                        {
                            if (gamer.UserID == room.fangPaoUid)
                            {
                                Log.Debug($"玩家：{gamer.UserID} 输了{huaCount * self.RoomConfig.Multiples}");
                                await DBCommonUtil.ChangeWealth(gamer.UserID, 1, -huaCount * self.Multiples, self.RoomConfig.Name + "结算");
                            }
                        }
                        else
                        {
                            Log.Debug($"玩家：{gamer.UserID} 输了{huaCount * self.RoomConfig.Multiples}");
                            await DBCommonUtil.ChangeWealth(gamer.UserID, 1, -huaCount * self.Multiples, self.RoomConfig.Name + "结算");
                        }
                    }
                }
            }
        }
    }
}