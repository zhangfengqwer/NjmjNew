using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoomSystemEx : AwakeSystem<Room>
    {
        public override void Awake(Room self)
        {
            self.Awake();
        }
    }



    public static class RoomSystem
    {
        public static async void Awake(this Room self)
        {
//            RoomDispose(self);
        }

        /// <summary>
        /// 一局10分钟超时解散
        /// </summary>
        /// <param name="self"></param>
        public static async void RoomDispose(this Room self)
        {
            if (self.roomTokenSource != null)
            {
                self.roomTokenSource.Cancel();
            }

            self.roomTokenSource = new CancellationTokenSource();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(10 * 60 * 1000, self.roomTokenSource.Token);
            Log.Warning("房间卡死，超时释放");
            Game.Scene.GetComponent<RoomComponent>().RemoveRoom(self, true);
            self.Broadcast(new Actor_GamerReadyTimeOut()
            {
                Message = "房间解散，被踢出"
            });

            self.Dispose();
        }

        public static void Broadcast(this Room self, IActorMessage message)
        {
            if (self == null) return;
            ActorMessageSenderComponent actorMessageSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
            foreach (Gamer gamer in self.gamers)
            {
                if (gamer == null || gamer.isOffline)
                {
                    continue;
                }

                UnitGateComponent unitGateComponent = gamer.GetComponent<UnitGateComponent>();
                actorMessageSenderComponent.GetWithActorId(unitGateComponent.GateSessionActorId).Send(message);
            }
        }

        public static void GamerBroadcast(this Room self, Gamer gamer, IActorMessage message)
        {
            if (gamer == null || gamer.isOffline)
            {
                return;
            }

            ActorMessageSenderComponent actorMessageSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
            UnitGateComponent unitGateComponent = gamer.GetComponent<UnitGateComponent>();
            actorMessageSenderComponent.GetWithActorId(unitGateComponent.GateSessionActorId).Send(message);
        }

        public static void GamerReconnect(this Room self, Gamer gamer, IActorMessage message)
        {
            if (gamer == null)
            {
                return;
            }

            ActorMessageSenderComponent actorMessageSenderComponent =
                Game.Scene.GetComponent<ActorMessageSenderComponent>();
            UnitGateComponent unitGateComponent = gamer?.GetComponent<UnitGateComponent>();
            actorMessageSenderComponent.GetWithActorId(unitGateComponent.GateSessionActorId).Send(message);
        }

        /// <summary>
        /// 加入房间Actor
        /// </summary>
        /// <param name="self"></param>
        /// <param name="userId"></param>
        public static async void BroadGamerEnter(this Room self, long userId)
        {
            List<GamerInfo> gamerInfos = new List<GamerInfo>();
            long roomType = self.GetComponent<GameControllerComponent>().RoomConfig.Id;
            GamerInfo currentInfo = null;
            for (int i = 0; i < self.GetAll().Length; i++)
            {
                Gamer _gamer = self.GetAll()[i];
                if (_gamer == null) continue;
                GamerInfo gamerInfo = new GamerInfo();
                gamerInfo.UserID = _gamer.UserID;
                gamerInfo.SeatIndex = self.GetGamerSeat(_gamer.UserID);
                gamerInfo.IsReady = _gamer.IsReady;

                if (_gamer.playerBaseInfo == null)
                {
                    _gamer.playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(gamerInfo.UserID);
                }

                PlayerInfo playerInfo = PlayerInfoFactory.Create(_gamer.playerBaseInfo);
                gamerInfo.playerInfo = playerInfo;

                if (gamerInfo.UserID == userId)
                {
                    currentInfo = gamerInfo;
                }

                gamerInfos.Add(gamerInfo);
            }

            foreach (var _gamer in self.GetAll())
            {
                if (_gamer == null || _gamer.isOffline)
                    continue;

                //第一次进入
                if (_gamer.UserID == userId)
                {
                    Actor_GamerEnterRoom actorGamerEnterRoom = new Actor_GamerEnterRoom()
                    {
                        RoomType = (int) roomType,
                        Gamers = gamerInfos,
                    };

                    if (roomType == 3)
                    {
                        GameControllerComponent gameControllerComponent = self.GetComponent<GameControllerComponent>();
                        actorGamerEnterRoom.RoomId = gameControllerComponent.RoomConfig.FriendRoomId;
                        actorGamerEnterRoom.MasterUserId = gameControllerComponent.RoomConfig.MasterUserId;
                        actorGamerEnterRoom.JuCount = gameControllerComponent.RoomConfig.JuCount;
                        actorGamerEnterRoom.Multiples = gameControllerComponent.RoomConfig.Multiples;
                        actorGamerEnterRoom.CurrentJuCount = self.CurrentJuCount;
                    }

                    self.GamerBroadcast(_gamer, actorGamerEnterRoom);
                }
                //有人加入
                else
                {
                    Actor_GamerJionRoom actorGamerJionRoom = new Actor_GamerJionRoom()
                    {
                        Gamer = currentInfo
                    };

                    self.GamerBroadcast(_gamer, actorGamerJionRoom);
                }
            }
        }

        /// <summary>
        /// 超时10s自动出牌
        /// </summary>
        /// <param name="self"></param>
        public static async void StartTime(this Room self, int time = 10)
        {
            if (self.tokenSource != null)
            {
                self.tokenSource.Cancel();
            }

            self.tokenSource = new CancellationTokenSource();
            OrderControllerComponent controllerComponent = self.GetComponent<OrderControllerComponent>();
            Gamer gamer = self.Get(controllerComponent.CurrentAuthority);
            if (gamer == null)
            {
                return;
            }

            if (gamer.isOffline)
            {
                self.TimeOut = 2;
            }
            else
            {
                self.TimeOut = time;
            }

            if (gamer.IsTrusteeship)
            {
                self.TimeOut = 2;
            }
            else
            {
                self.TimeOut = time;
            }

            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(self.TimeOut * 1000, self.tokenSource.Token);

            if (!self.tokenSource.IsCancellationRequested)
            {
                self.IsTimeOut = true;
                //超时自动出牌
                if (gamer == null)
                {
                    return;
                }

                MahjongInfo mahjongInfo = await gamer.GetComponent<HandCardsComponent>().PopCard();
                if (!gamer.IsTrusteeship)
                {
                    gamer.IsTrusteeship = true;
                    self.Broadcast(new Actor_GamerTrusteeship()
                    {
                        Uid = gamer.UserID
                    });
                }
            }
            else
            {
                self.IsTimeOut = false;
                Log.Debug("没有超时");
            }
        }

        /// <summary>
        /// 好友房结束后准备倒计时
        /// </summary>
        /// <param name="self"></param>
        public static async void StartReady(this Room self)
        {
            self.StartReadySource?.Cancel();
            self.StartReadySource = new CancellationTokenSource();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(5 * 1000, self.StartReadySource.Token);

            foreach (var gamer in self.GetAll())
            {
                if (gamer.isOffline)
                {
                    await Actor_GamerReadyHandler.GamerReady(gamer, new Actor_GamerReady() { });
                }
            }
        }

        /// <summary>
        /// 碰杠胡倒计时
        /// </summary>
        /// <param name="self"></param>
        public static async void StartOperateTime(this Room self)
        {
            self.tokenSource?.Cancel();

            self.tokenSource = new CancellationTokenSource();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(self.OperationTimeOut * 1000, self.tokenSource.Token);

            if (!self.tokenSource.IsCancellationRequested)
            {
                if (self.IsDisposed)
                {
                    return;
                }
                self.IsTimeOut = true;
                //没有人碰刚
                self.IsOperate = false;
                self.IsPlayingCard = false;
                self.IsNeedWaitOperate = false;
                foreach (var gamer in self.GetAll())
                {
                    gamer.IsCanGang = false;
                    gamer.IsCanGang = false;
                    gamer.IsCanHu = false;
                }
                //Log.Info($"碰刚超时，发牌");
                self.GamerGrabCard();
                //                Log.Debug("OperateTime超时");
            }
            else
            {
                self.IsTimeOut = false;
//                Log.Debug("OperateTime没有超时");
            }
        }


        public static bool CanHu(this Room self, MahjongInfo mahjongInfo, List<MahjongInfo> list)
        {
            List<MahjongInfo> temp = new List<MahjongInfo>(list);
            temp.Add(mahjongInfo);

            try
            {
                if (Logic_NJMJ.getInstance().isHuPai(temp))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error("胡牌错误:" + e);
                Log.Info(JsonHelper.ToJson(temp));
            }


            return false;
        }

        /// <summary>
        /// 发牌,包含抓牌和补花
        /// </summary>
        /// <param name="room"></param>
        public static async void GamerGrabCard(this Room room)
        {
            foreach (var gamer in room.GetAll())
            {
                if (gamer == null)
                {
                    Log.Error("发牌的时候gamer为null:"+JsonHelper.ToJson(room.UserIds)+"\n------："+JsonHelper.ToJson(room.GetAll()));
                    
                    continue;
                }
                gamer.isGangFaWanPai = false;
            }

            OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();

            orderController.Turn();
            var currentGamer = room.Get(orderController.CurrentAuthority);

            currentGamer.isGangEndBuPai = false;
            currentGamer.isGetYingHuaBuPai = false;
            await room.GrabMahjongNoHua(currentGamer);
        }

        /// <summary>
        /// 给当前玩家发一张不是花牌的牌
        /// </summary>
        /// <param name="room"></param>
        /// <param name="currentGamer"></param>
        /// <returns></returns>
        public static async Task<MahjongInfo> GrabMahjongNoHua(this Room room, Gamer currentGamer)
        {
            GameControllerComponent gameController = room.GetComponent<GameControllerComponent>();
            HandCardsComponent cardsComponent = currentGamer.GetComponent<HandCardsComponent>();
            var grabMahjong = GrabMahjong(room);
            if (grabMahjong == null)
            {
                Log.Info("没牌流局了");
                await gameController.GameOver(0);
                return null;
            }
            while (grabMahjong.m_weight >= Consts.MahjongWeight.Hua_HongZhong)
            {
                Actor_GamerBuHua actorGamerBuHua = new Actor_GamerBuHua()
                {
                    Uid = currentGamer.UserID,
                    weight = grabMahjong.weight
                };
                room.Broadcast(actorGamerBuHua);

                //从手牌中删除花牌
                Log.Info($"{currentGamer.UserID}补花,{grabMahjong.m_weight}");
                cardsComponent.FaceCards.Add(grabMahjong);

                #region 花杠
                int temp = 0;
                foreach (var faceCard in cardsComponent.FaceCards)
                {
                    if (faceCard.m_weight == grabMahjong.m_weight)
                    {
                        temp++;
                    }
                }

                Logic_NJMJ.getInstance().SortMahjong(cardsComponent.FaceCards);
                //春夏秋冬
                for (int i = 0; i < cardsComponent.FaceCards.Count - 4; i += 4)
                {
                    if (cardsComponent.FaceCards[i + 3].m_weight - cardsComponent.FaceCards[i + 2].m_weight == 2 &&
                        cardsComponent.FaceCards[i + 2].m_weight - cardsComponent.FaceCards[i + 1].m_weight == 2 &&
                        cardsComponent.FaceCards[i + 1].m_weight - cardsComponent.FaceCards[i].m_weight == 2)
                    {
                        temp = 4;
                    }
                }

                if (temp == 4)
                {
                    foreach (var _gamer in room.GetAll())
                    {
                        if (_gamer.UserID == currentGamer.UserID)
                        {
                            GameHelp.ChangeGamerGold(room, _gamer, 20 * gameController.RoomConfig.Multiples * 3);
                        }
                        else
                        {
                            GameHelp.ChangeGamerGold(room, _gamer, -20 * gameController.RoomConfig.Multiples);
                        }
                    }

                    room.LastBiXiaHu = true;
                }

                #endregion
                currentGamer.isGangEndBuPai = false;
                currentGamer.isGetYingHuaBuPai = true;

                grabMahjong = GrabMahjong(room);

                if (grabMahjong == null)
                {
                    Log.Info("没牌流局了");
                    await gameController.GameOver(0);
                    return null; ;
                }
            }

            room.StartTime();
            return grabMahjong;
        }


        /// <summary>
        /// 抓牌
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static MahjongInfo GrabMahjong(this Room room)
        {
            try
            {
                OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
                var currentGamer = room.Get(orderController.CurrentAuthority);
                HandCardsComponent cardsComponent = currentGamer.GetComponent<HandCardsComponent>();
                DeskComponent deskComponent = room.GetComponent<DeskComponent>();

                if (deskComponent.RestLibrary.Count == 0)
                {
                    Log.Info("没牌了");
                    return null;
                }

                MahjongInfo grabMahjong;
                if (room.NextGrabCard != null)
                {
                    grabMahjong = new MahjongInfo()
                    {
                        m_weight = room.NextGrabCard.m_weight,
                        weight = room.NextGrabCard.weight
                    };

                    room.NextGrabCard = null;
                    Log.Debug("发作弊牌：" + grabMahjong.m_weight);
                }
                else
                {
                    int number = RandomHelper.RandomNumber(0, deskComponent.RestLibrary.Count);
                    grabMahjong = deskComponent.RestLibrary[number];
                    deskComponent.RestLibrary.RemoveAt(number);

                    Log.Info($"{currentGamer.UserID}发牌："+ grabMahjong.m_weight);
                }

                //花牌返回
                if (grabMahjong.m_weight >= Consts.MahjongWeight.Hua_HongZhong)
                {
                    return grabMahjong;
                }

                //发牌
                cardsComponent.AddCard(grabMahjong);
                room.my_lastMahjong = grabMahjong;
                cardsComponent.GrabCard = grabMahjong;

                Logic_NJMJ.getInstance().SortMahjong(cardsComponent.GetAll());

                //发送抓牌消息
                ActorMessageSenderComponent actorMessageSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
             
                foreach (Gamer _gamer in room.gamers)
                {
                    if (_gamer == null || _gamer.isOffline)
                    {
                        continue;
                    }
                    UnitGateComponent unitGateComponent = _gamer?.GetComponent<UnitGateComponent>();
                    Actor_GamerGrabCard actorGamerGrabCard;
                    if (_gamer.UserID == orderController.CurrentAuthority)
                    {
                        actorGamerGrabCard = new Actor_GamerGrabCard()
                        {
                            Uid = currentGamer.UserID,
                            weight = (int) grabMahjong.m_weight
                        };

                    }
                    else
                    {
                        actorGamerGrabCard = new Actor_GamerGrabCard()
                        {
                            Uid = currentGamer.UserID,
                        };

                    }
                    actorMessageSenderComponent.GetWithActorId(unitGateComponent.GateSessionActorId).Send(actorGamerGrabCard);
                }

                //发完牌判断是否胡牌
                foreach (Gamer _gamer in room.gamers)
                {
                    if (_gamer == null || _gamer.isOffline)
                    {
                        continue;
                    }

                    if (_gamer.UserID == orderController.CurrentAuthority)
                    {
                        HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();

                        //判断胡牌
                        if (Logic_NJMJ.getInstance().isHuPai(handCardsComponent.GetAll()))
                        {
                            _gamer.huPaiNeedData.my_lastMahjong = room.my_lastMahjong;
                            _gamer.huPaiNeedData.restMahjongCount = deskComponent.RestLibrary.Count;
                            _gamer.huPaiNeedData.isSelfZhuaPai = orderController.CurrentAuthority == _gamer.UserID;
                            _gamer.huPaiNeedData.isZhuangJia = handCardsComponent.IsBanker;
                            _gamer.huPaiNeedData.isGetYingHuaBuPai = _gamer.isGetYingHuaBuPai;
                            _gamer.huPaiNeedData.isGangEndBuPai = _gamer.isGangEndBuPai;
                            _gamer.huPaiNeedData.isGangFaWanPai = _gamer.isGangFaWanPai;
                            _gamer.huPaiNeedData.isFaWanPaiTingPai = _gamer.isFaWanPaiTingPai;
                            _gamer.huPaiNeedData.my_yingHuaList = handCardsComponent.FaceCards;
                            _gamer.huPaiNeedData.my_gangList = handCardsComponent.GangCards;
                            _gamer.huPaiNeedData.my_pengList = handCardsComponent.PengCards;
                            List<List<MahjongInfo>> tempList = new List<List<MahjongInfo>>();
                            for (int i = 0; i < room.GetAll().Length; i++)
                            {
                                if (_gamer.UserID == room.GetAll()[i].UserID)
                                    continue;
                                tempList.Add(room.GetAll()[i].GetComponent<HandCardsComponent>().PengCards);
                            }

                            _gamer.huPaiNeedData.other1_pengList = tempList[0];
                            _gamer.huPaiNeedData.other2_pengList = tempList[1];
                            _gamer.huPaiNeedData.other3_pengList = tempList[2];

                            List<Consts.HuPaiType> huPaiTypes = Logic_NJMJ.getInstance().getHuPaiType(handCardsComponent.GetAll(), 
                                                                                            _gamer.huPaiNeedData);
                            Log.Info(JsonHelper.ToJson(_gamer.huPaiNeedData));
                            Log.Info(JsonHelper.ToJson(huPaiTypes));

                            if (huPaiTypes.Count > 0)
                            {
                                //判断小胡,4个花以上才能胡
                                if (huPaiTypes[0] == Consts.HuPaiType.Normal)
                                {
                                    if (handCardsComponent.PengGangCards.Count > 0 || handCardsComponent.PengCards.Count > 0)
                                    {
                                        if (handCardsComponent.FaceCards.Count >= 4)
                                        {
                                            _gamer.IsCanHu = true;
                                            Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
                                            canOperation.Uid = _gamer.UserID;
                                            canOperation.OperationType = 2;
                                            room.GamerBroadcast(_gamer, canOperation);
                                        }
                                    }
                                }
                                else
                                {
                                    _gamer.IsCanHu = true;
                                    Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
                                    canOperation.Uid = _gamer.UserID;
                                    canOperation.OperationType = 2;
                                    room.GamerBroadcast(_gamer, canOperation);
                                }
                            }
                        }

                        //暗杠
                        if (Logic_NJMJ.getInstance().IsAnGang(handCardsComponent.GetAll(), out var weight))
                        {
                            _gamer.IsCanGang = true;
                            Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
                            canOperation.Uid = _gamer.UserID;
                            canOperation.OperationType = 4;
                            room.GamerBroadcast(_gamer, canOperation);
                        }
                        //碰杠
                        else if (Logic_NJMJ.getInstance().IsPengGang(handCardsComponent.PengCards, handCardsComponent.GetAll(),out var weight2))
                        {
                            _gamer.IsCanGang = true;
                            Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
                            canOperation.Uid = _gamer.UserID;
                            canOperation.OperationType = 5;
                            room.GamerBroadcast(_gamer, canOperation);
                        }
                    }
                }

                return grabMahjong;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 等待房间解散操作
        /// </summary>
        /// <param name="self"></param>
        /// <param name="time"></param>
        public static async void WaitDismiss(this Room self, int time)
        {
            self.roomDismissTokenSource?.Cancel();
            self.roomDismissTokenSource = new CancellationTokenSource();
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(time * 1000, self.roomDismissTokenSource.Token);

            foreach (var gamer in self.GetAll())
            {
                if (gamer?.DismissState == DismissState.None)
                {
                    await Actor_GamerAgreeRoomDismissHandler.GamerAgreeRoomDismiss(gamer);
                }
            }
        }
    }
}