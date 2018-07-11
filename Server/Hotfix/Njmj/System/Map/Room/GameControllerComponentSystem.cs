using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    public static class GameControllerComponentSystem
    {
        private static List<List<MahjongInfo>> temp = new List<List<MahjongInfo>>();

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
//                    Log.Debug("发牌前有牌了：" + handCardsComponent.GetAll().Count);
                    temp.Add(null);
                }
                else
                {
//                    Log.Debug("发牌前没有拍");
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
        public static async Task GameOver(this GameControllerComponent self, int huaCount)
        {
            RoomComponent roomComponent = null;
            Room room = null;
            try
            {
                room = self.GetParent<Room>();
                room.IsGameOver = true;
                room.tokenSource.Cancel();
                roomComponent = Game.Scene.GetComponent<RoomComponent>();
                DeskComponent deskComponent = room.GetComponent<DeskComponent>();
                GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();

                deskComponent.RestLibrary.Clear();

                if (huaCount == 0)
                {
                    //没牌
                    room.huPaiUid = 0;
                    room.fangPaoUid = 0;
                    room.LiangZhuangCount = 0;
                    room.Broadcast(new Actor_GameFlow());
                }

                await ChangeWeath(self, huaCount, room);

                //更新任务
                await UpdateTask(room);
                await UpdateChengjiu(room);
                await UpdatePlayerInfo(room, huaCount);
                //记录对局
                await DBCommonUtil.Log_Game(controllerComponent.RoomConfig.Name, room.GetAll()[0].UserID,
                    room.GetAll()[1].UserID,
                    room.GetAll()[2].UserID, room.GetAll()[3].UserID, room.huPaiUid);

                //设置在线时长
                foreach (var gamer in room.GetAll())
                {
                    //在线
                    if (!gamer.isOffline)
                    {
                        gamer.EndTime = DateTime.Now;
                        TimeSpan span = gamer.EndTime - gamer.StartTime;
                        int totalSeconds = (int)span.TotalSeconds;
                        //await DBCommonUtil.RecordGamerTime(gamer.EndTime, false, gamer.UserID);
                        await DBCommonUtil.RecordGamerInfo(gamer.UserID, totalSeconds);
                    }
                }
            
                room.State = RoomState.Idle;
                room.IsLianZhuang = true;
                
                //游戏房间进入准备房间
                roomComponent.gameRooms.Remove(room.Id);
                roomComponent.idleRooms.Add(room.Id, room);

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
                    gamer.IsTrusteeship = false;
                    //离线踢出
                    if (gamer.isOffline)
                    {
                        Log.Info("玩家结束游戏后离线踢出");
                        room.Remove(gamer.UserID);
                        gamer.isOffline = !gamer.isOffline;
                    }
                }

                //房间没人就释放
                if (room.seats.Count == 0)
                {
                    Log.Info($"房间释放:{room.Id}");
                    roomComponent.RemoveRoom(room);
                    room?.Dispose();
                }
            }
            catch (Exception e)
            {
                Log.Error("房间结算："+ e);
                //游戏房间进入准备房间
                roomComponent?.gameRooms.Remove(room.Id);
                roomComponent?.idleRooms.Add(room.Id, room);
            }
           
        }

        private static async Task UpdatePlayerInfo(Room room, int huaCount)
        {
            foreach (var gamer in room.GetAll())
            {
                if (gamer == null) continue;
                //胜利
                if (gamer.UserID == room.huPaiUid)
                {
//                    Log.Debug("玩家:" + gamer.UserID + "胜利");
                    await DBCommonUtil.UpdatePlayerInfo(gamer.UserID, huaCount, true);
                }
                else
                {
//                    Log.Debug("玩家:" + gamer.UserID + "失败");
                    await DBCommonUtil.UpdatePlayerInfo(gamer.UserID, 0);
                }
            }
        }

        private static async Task UpdateTask(Room room)
        {
            GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
            var dbProxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
            foreach (var gamer in room.GetAll())
            {
                if (gamer == null) continue;
                List<DuanwuDataBase> databases =
                    await dbProxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{gamer.UserID}}}");
                List<string> str_list = new List<string>();
                CommonUtil.splitStr(databases[0].ActivityType, str_list, ';');
//                Log.Debug(str_list.Count + "");
                //胜利
                if (gamer.UserID == room.huPaiUid)
                {
                    if (controllerComponent.RoomName == RoomName.ChuJi)
                    {
                        Log.Debug("新手场SHENGLI");
                        //	102	新手场	在新手场赢得10场胜利	1000	10
                        await DBCommonUtil.UpdateTask(gamer.UserID, 102, 1);
                    }
                    else if (controllerComponent.RoomName == RoomName.JingYing)
                    {
                        Log.Debug("精英场SHENGLI");
                        //	103	精英场	在精英场赢得30场胜利	100000	30
                        await DBCommonUtil.UpdateTask(gamer.UserID, 103, 1);
                    }

//                    Log.Debug("	连赢5场");
                    //	104	游戏高手	连赢5场	10000	5
                    await DBCommonUtil.UpdateTask(gamer.UserID, 104, 1);

                    //端午任务
                    for (int i = 0; i < str_list.Count; ++i)
                    {
                        int id = 100 + int.Parse(str_list[i]) + 1;
//                        Log.Debug(id + "");
                        if (id == 105 || id == 106 || id == 107 || id == 108)
                        {
                            await DBCommonUtil.UpdateDuanwuActivity(gamer.UserID, id, 1);
                        }
                    }
                }
                //输了
                else
                {
//                    Log.Debug("SHULE");
                    //	104	游戏高手	连赢5场	10000	5
                    await DBCommonUtil.UpdateTask(gamer.UserID, 104, -1);
                }

                //101  新的征程	完成一局游戏	100	1
                await DBCommonUtil.UpdateTask(gamer.UserID, 101, 1);

                for (int i = 0; i < str_list.Count; ++i)
                {
                    int id = 100 + int.Parse(str_list[i]) + 1;
                    if (id == 101 || id == 102 || id == 103 || id == 104)
                    {
                        await DBCommonUtil.UpdateDuanwuActivity(gamer.UserID, id, 1);
                    }
                }
            }
        }

        private static async Task UpdateChengjiu(Room room)
        {
//            Log.Debug("更新成就:房间ID为:" + room.Id);
            foreach (var gamer in room.GetAll())
            {
                if (gamer == null) continue;
                //胜利
                if (gamer.UserID == room.huPaiUid)
                {
//                    Log.Debug("成就胜利");
                    //赢得10局游戏
                    await DBCommonUtil.UpdateChengjiu(gamer.UserID, 104, 1);
                    //赢得100局游戏
                    await DBCommonUtil.UpdateChengjiu(gamer.UserID, 105, 1);
                    //赢得1000局游戏
                    await DBCommonUtil.UpdateChengjiu(gamer.UserID, 106, 1);
                }

                //新手上路 完后10局游戏
                await DBCommonUtil.UpdateChengjiu(gamer.UserID, 101, 1);
//                Log.Debug("不论输赢都会加一" + gamer.UserID + "任务" + 101);
                //已有小成 完成100局游戏
                await DBCommonUtil.UpdateChengjiu(gamer.UserID, 102, 1);
//                Log.Debug("不论输赢都会加一" + gamer.UserID + "任务" + 102);
                //完成1000局游戏
                await DBCommonUtil.UpdateChengjiu(gamer.UserID, 103, 1);
//                Log.Debug("不论输赢都会加一" + gamer.UserID + "任务" + 103);
                 
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
            int amount = huaCount * self.RoomConfig.Multiples;
            if (huaCount > 0)
            {
                //改变财富
                foreach (var gamer in room.GetAll())
                {
                    //自摸
                    if (room.IsZimo)
                    {
                        if (gamer.UserID == room.huPaiUid)
                        {
                            GameHelp.ChangeGamerGold(room, gamer, amount * 3, self.RoomConfig.Name + "结算");
//                            await DBCommonUtil.ChangeWealth(gamer.UserID, 1, amount * 3,self.RoomConfig.Name + "结算");
                            UpdateTask(gamer, amount * 3);
                        }
                        else
                        {
//                            Log.Debug($"玩家：{gamer.UserID} 输了{amount}");
//                            await DBCommonUtil.ChangeWealth(gamer.UserID, 1, -amount, self.RoomConfig.Name + "结算");
                            GameHelp.ChangeGamerGold(room, gamer, -amount, self.RoomConfig.Name + "结算");
                        }
                    }
                    else
                    {
                        if (gamer.UserID == room.huPaiUid)
                        {
//                            await DBCommonUtil.ChangeWealth(gamer.UserID, 1, amount, self.RoomConfig.Name + "结算");
                            GameHelp.ChangeGamerGold(room, gamer, amount, self.RoomConfig.Name + "结算");
                            UpdateTask(gamer, amount);
                        }
                        else
                        {
                            if (gamer.UserID == room.fangPaoUid)
                            { 
//                                Log.Debug($"玩家：{gamer.UserID} 输了{amount}");
//                                await DBCommonUtil.ChangeWealth(gamer.UserID, 1, -amount, self.RoomConfig.Name + "结算");
                                GameHelp.ChangeGamerGold(room, gamer, -amount, self.RoomConfig.Name + "结算");
                            }
                        }
                    }
                }
            }
        }

        private static async void UpdateTask(Gamer gamer, int amount)
        {
            //	105	赚钱高手	当日累计赢取10000金币	10000	10000
            await DBCommonUtil.UpdateTask(gamer.UserID, 105, amount);
            // 110 小试身手 单局赢取10000金币满 100局
            if (amount >= 10000)
                await DBCommonUtil.UpdateChengjiu(gamer.UserID, 110, 1);
            // 111 来者不拒 单局赢取100万金币满 100局
            if (amount >= 1000000)
                await DBCommonUtil.UpdateChengjiu(gamer.UserID, 111, 1);
            // 112 富豪克星 单局赢取一亿金币满 100局
            if (amount >= 100000000)
                await DBCommonUtil.UpdateChengjiu(gamer.UserID, 112, 1);

            {
                var dbProxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DuanwuDataBase> databases =
                    await dbProxyComponent.QueryJson<DuanwuDataBase>($"{{UId:{gamer.UserID}}}");
//                Log.Debug(databases.Count + "========");
                List<string> str_list = new List<string>();
                CommonUtil.splitStr(databases[0].ActivityType, str_list, ';');

//                Log.Debug("str_list:" + JsonHelper.ToJson(str_list));
                for (int i = 0; i < str_list.Count; ++i)
                {
                    int id = 100 + int.Parse(str_list[i]) + 1;
//                    Log.Debug(id + "任务ID");
                    if (id == 109 || id == 110 || id == 111 || id == 112)
                    {
                        await DBCommonUtil.UpdateDuanwuActivity(gamer.UserID, id, amount);
                    }
                }
            }

        }
    }
}