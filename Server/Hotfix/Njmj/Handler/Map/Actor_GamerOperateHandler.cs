using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Map)]
    public class Actor_GamerOperateHandler : AMActorHandler<Gamer, Actor_GamerOperation>
    {
        protected override async Task Run(Gamer gamer, Actor_GamerOperation message)
        {
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);

                room.IsOperate = true;
                room.IsPlayingCard = false;
                room.IsNeedWaitOperate = false;
                if (room.IsGameOver) return;

                DeskComponent deskComponent = room.GetComponent<DeskComponent>();
                OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
                GameControllerComponent gameController = room.GetComponent<GameControllerComponent>();

                Gamer currentGamer = room.Get(orderController.CurrentAuthority);

                HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                if (handCards == null) return;
                List<MahjongInfo> mahjongInfos = handCards.GetAll();
                Log.Info($"玩家{gamer.UserID}碰或刚{message.OperationType},当前出牌:{deskComponent.CurrentCard.m_weight}");

                //胡牌
                int huaCount = 0;
                if (message.OperationType == 2)
                {
                    //自摸
                    bool isFinish = false;
                    if (orderController.CurrentAuthority == gamer.UserID)
                    {
                        if (Logic_NJMJ.getInstance().isHuPai(mahjongInfos))
                        {
                            huaCount = HuPai(gamer, room, mahjongInfos, true);

                            room.huPaiUid = gamer.UserID;
                            isFinish = true;
                        }
                    }
                    //放炮
                    else
                    {
                        if (room.CanHu(deskComponent.CurrentCard, mahjongInfos))
                        {
                            List<MahjongInfo> temp = new List<MahjongInfo>(mahjongInfos);
                            temp.Add(deskComponent.CurrentCard);
                            huaCount = HuPai(gamer, room, temp, false);
                            isFinish = true;
                            room.huPaiUid = gamer.UserID;
                            room.fangPaoUid = orderController.CurrentAuthority;
                        }
                    }

                    if (isFinish)
                    {
                        //游戏结束结算
                        await gameController.GameOver(huaCount);
                    }

                    gamer.IsCanHu = false;
                    gamer.IsCanPeng = false;
                    gamer.IsCanGang = false;
                }
                //放弃
                else if (message.OperationType == 3)
                {
                    Log.Debug("放弃:" + gamer.UserID);
                    gamer.IsCanHu = false;
                    gamer.IsCanPeng = false;
                    gamer.IsCanGang = false;
                    if (orderController.CurrentAuthority == gamer.UserID)
                    {
                        //room.tokenSource.Cancel();
                    }
                    else
                    {
                        room.tokenSource.Cancel();
                        Log.Info($"{gamer.UserID}取消，发牌");
                        room.GamerGrabCard();
                    }
                }
                else
                {
                    Actor_GamerOperation gamerOperation = new Actor_GamerOperation();
                    gamerOperation.Uid = gamer.UserID;
                    gamerOperation.weight = deskComponent.CurrentCard.weight;

                    //有没有人胡牌
                    while (true)
                    {
                        if (!GetCanHu(room, gamer))
                        {
                            break;
                        }
                        await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                    }

                    //游戏结束
                    if (room.IsGameOver)
                    {
                        Log.Warning("游戏结束不能碰刚");
                        return;
                    }

                    // 碰
                    if (message.OperationType == 0)
                    {
                        if (Logic_NJMJ.getInstance().isCanPeng(deskComponent.CurrentCard, mahjongInfos))
                        {
                            room.tokenSource.Cancel();
                            //Log.Info("11111");

                            gamerOperation.OperationType = 0;
                            gamerOperation.OperatedUid = deskComponent.CurrentAuthority;
                            room.Broadcast(gamerOperation);

                            //更新手牌
                            for (int i = 0; i < 2; i++)
                            {
                                int index = Logic_NJMJ.getInstance().GetIndex(mahjongInfos, deskComponent.CurrentCard);
                                mahjongInfos.RemoveAt(index);
                            }
                            //Log.Info("222222");
                            handCards.PengCards.Add(deskComponent.CurrentCard);
                            currentGamer.GetComponent<HandCardsComponent>().PlayCards.Remove(deskComponent.CurrentCard);

                            //添加碰的人
                            PengOrBar pengOrBar = ComponentFactory.Create<PengOrBar>();
                            pengOrBar.OperateType = OperateType.Peng;
                            pengOrBar.Weight = deskComponent.CurrentCard.weight;
                            pengOrBar.UserId = deskComponent.CurrentAuthority;
                            pengOrBar.BarType = BarType.None;

                            handCards.PengOrBars.Add(pengOrBar);

                            Log.Debug("PengOrBars:" + handCards.PengOrBars.Count);

                            //碰完当前玩家出牌
                            orderController.CurrentAuthority = gamer.UserID;
                            room.StartTime();
                        }
                    }
                    // 杠
                    else
                    {
                        HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
                        gamerOperation.OperationType = message.OperationType;

                        bool isSuccess = false;
                        //明杠
                        if (Logic_NJMJ.getInstance().isCanGang(deskComponent.CurrentCard, mahjongInfos))
                        {
                            isSuccess = true;

                            //更新手牌
                            for (int i = 0; i < 3; i++)
                            {
                                int index = Logic_NJMJ.getInstance().GetIndex(mahjongInfos, deskComponent.CurrentCard);
                                mahjongInfos.RemoveAt(index);
                            }

                            handCards.GangCards.Add(deskComponent.CurrentCard);

                            currentGamer.GetComponent<HandCardsComponent>().PlayCards.Remove(deskComponent.CurrentCard);

                            //杠扣钱
                            GameHelp.ChangeGamerGold(room, gamer, 20 * gameController.RoomConfig.Multiples);
                            GameHelp.ChangeGamerGold(room, currentGamer, -20 * gameController.RoomConfig.Multiples);

                            //添加明杠
                            gamerOperation.OperatedUid = deskComponent.CurrentAuthority;
                            PengOrBar pengOrBar = ComponentFactory.Create<PengOrBar>();
                            pengOrBar.OperateType = OperateType.Bar;
                            pengOrBar.Weight = deskComponent.CurrentCard.weight;
                            pengOrBar.UserId = deskComponent.CurrentAuthority;
                            pengOrBar.BarType = BarType.LightBar;
                            handCards.PengOrBars.Add(pengOrBar);

                        }
                        //暗杠
                        else if (Logic_NJMJ.getInstance().IsAnGang(handCardsComponent.GetAll(), out var weight))
                        {
                            isSuccess = true;
                            gamerOperation.weight = (int) weight;
                            gamerOperation.OperationType = 4;

                            MahjongInfo info = new MahjongInfo()
                            {
                                weight = (byte) weight,
                                m_weight = (Consts.MahjongWeight) weight
                            };
                            //更新手牌
                            for (int i = 0; i < 4; i++)
                            {
                                int index = Logic_NJMJ.getInstance().GetIndex(mahjongInfos, info);
                                mahjongInfos.RemoveAt(index);
                            }

                            handCards.GangCards.Add(info);
                            //杠扣钱
                            foreach (var _gamer in room.GetAll())
                            {
                                if (_gamer.UserID == gamer.UserID)
                                {
                                    GameHelp.ChangeGamerGold(room, _gamer, 10 * gameController.RoomConfig.Multiples * 3);
                                }
                                else
                                {
                                    GameHelp.ChangeGamerGold(room, _gamer, -10 * gameController.RoomConfig.Multiples);
                                }
                            }

                            //添加暗杠
                            gamerOperation.OperatedUid = 0;
                            PengOrBar pengOrBar = ComponentFactory.Create<PengOrBar>();
                            pengOrBar.OperateType = OperateType.Bar;
                            pengOrBar.Weight = (int) weight;
                            pengOrBar.UserId = 0;
                            pengOrBar.BarType = BarType.DarkBar;
                            handCards.PengOrBars.Add(pengOrBar);

                        }
                        //碰杠
                        else if (Logic_NJMJ.getInstance().IsPengGang(handCardsComponent.PengCards, handCardsComponent.GetAll(), out var weight1))
                        {
                            isSuccess = true;
                            gamerOperation.weight = weight1;
                            gamerOperation.OperationType = 5;

                            MahjongInfo info = new MahjongInfo()
                            {
                                weight = (byte) weight1,
                                m_weight = (Consts.MahjongWeight) weight1
                            };
                            //更新手牌
                            for (int i = 0; i < 1; i++)
                            {
                                int index = Logic_NJMJ.getInstance().GetIndex(mahjongInfos, info);
                                mahjongInfos.RemoveAt(index);
                            }

                            handCardsComponent.PengCards.Remove(info);

                            handCards.GangCards.Add(info);
                            handCards.PengGangCards.Add(info);

                            //添加碰杠

                            PengOrBar pengOrBar = null;
                            foreach (var item in handCardsComponent.PengOrBars)
                            {
                                if (item.Weight == weight1 && item.OperateType == OperateType.Peng)
                                {
                                    pengOrBar = item;
                                }
                            }

                            if (pengOrBar == null)
                            {
                                Log.Error("碰刚的牌没有碰过");
                                return;
                            }
                            pengOrBar.OperateType = OperateType.Bar;
                            pengOrBar.BarType = BarType.PengBar;
                            gamerOperation.OperatedUid = pengOrBar.UserId;
                            //杠扣钱
                            foreach (var _gamer in room.GetAll())
                            {
                                if (_gamer.UserID == gamer.UserID)
                                {
                                    GameHelp.ChangeGamerGold(room, _gamer, 20 * gameController.RoomConfig.Multiples);
                                }
                                else if(_gamer.UserID == pengOrBar.UserId)
                                {
                                    GameHelp.ChangeGamerGold(room, _gamer, -20 * gameController.RoomConfig.Multiples);
                                }
                            }
                        }

                        if (isSuccess)
                        {
                            room.Broadcast(gamerOperation);

                            //杠完之后不能
                            gamer.isFaWanPaiTingPai = false;
                            gamer.isGangEndBuPai = true;
                            gamer.isGetYingHuaBuPai = false;
                            //杠完后是本人出牌
                            orderController.CurrentAuthority = gamer.UserID;
                            //杠完之后抓牌
                            await room.GrabMahjongNoHua(gamer);

                            #region 比下胡 累计2个暗杠或3个明杠（同一个玩家）

                            int lightCount = handCards.PengOrBars.Count(c => c.BarType == BarType.LightBar);
                            int darkCount = handCards.PengOrBars.Count(c => c.BarType == BarType.DarkBar);
                            if (lightCount >= 3 || darkCount >= 2)
                            {
                                room.LastBiXiaHu = true;
                            }

                            #endregion
                        }
                        else
                        {
                            Log.Debug("不能杠");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        ///  胡牌
        /// </summary>
        /// <param name="gamer"></param>
        /// <param name="room"></param>
        /// <param name="mahjongInfos"></param>
        /// <param name="b"></param>
        private static int HuPai(Gamer gamer, Room room, List<MahjongInfo> mahjongInfos, bool isZimo)
        {
            room.IsZimo = isZimo;
            room.huPaiUid = gamer.UserID;
            if (!isZimo)
            {
                gamer.isGangEndBuPai = false;
                gamer.isGetYingHuaBuPai = false;
            }

            int huaCount = 0;

            DeskComponent deskComponent = room.GetComponent<DeskComponent>();
            OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
            HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();

            Actor_GamerHuPai actorGamerHuPai = new Actor_GamerHuPai();
            actorGamerHuPai.Uid = gamer.UserID;

            HuPaiNeedData huPaiNeedData = new HuPaiNeedData();
            huPaiNeedData.my_lastMahjong = room.my_lastMahjong;
            huPaiNeedData.restMahjongCount = deskComponent.RestLibrary.Count;
            huPaiNeedData.isSelfZhuaPai = orderController.CurrentAuthority == gamer.UserID;
            huPaiNeedData.isZhuangJia = handCards.IsBanker;
            huPaiNeedData.isGetYingHuaBuPai = gamer.isGetYingHuaBuPai;
            huPaiNeedData.isGangEndBuPai = gamer.isGangEndBuPai;
            huPaiNeedData.isGangFaWanPai = gamer.isGangFaWanPai;
            huPaiNeedData.isFaWanPaiTingPai = gamer.isFaWanPaiTingPai;
            huPaiNeedData.my_yingHuaList = handCards.FaceCards;
            huPaiNeedData.my_gangList = handCards.GangCards;
            huPaiNeedData.my_pengList = handCards.PengCards;

            List<List<MahjongInfo>> temp = new List<List<MahjongInfo>>();

            foreach (var _gamer in room.GetAll())
            {
                _gamer.RemoveComponent<TrusteeshipComponent>();

                if (_gamer.UserID == gamer.UserID)
                    continue;
                HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
                temp.Add(handCardsComponent.PengCards);

                //设置其他人的牌
                GamerData gamerData = new GamerData();
                gamerData.handCards = handCardsComponent.GetAll();
                gamerData.UserID = _gamer.UserID;
                actorGamerHuPai.GamerDatas.Add(gamerData);
            }

            huPaiNeedData.other1_pengList = temp[0];
            huPaiNeedData.other2_pengList = temp[1];
            huPaiNeedData.other3_pengList = temp[2];

            //比下胡
//            if (room.IsLianZhuang)
//            {
//                if (room.BankerGamer.UserID == room.huPaiUid)
//                {
//                    room.LiangZhuangCount++;
//                    actorGamerHuPai.BixiaHuCount = room.LiangZhuangCount * 10;
//                }
//            }
            if (room.IsBiXiaHu)
            {
                actorGamerHuPai.BixiaHuCount = 20;
            }
           
            List<Consts.HuPaiType> huPaiTypes = Logic_NJMJ.getInstance().getHuPaiType(mahjongInfos, huPaiNeedData);

            foreach (var huPaiType in huPaiTypes)
            {
                if (huPaiType != Consts.HuPaiType.Normal)
                {
                    room.LastBiXiaHu = true;
                }
            }

            //自摸
            actorGamerHuPai.IsZiMo = isZimo;
            if (!isZimo)
            {
                actorGamerHuPai.FangPaoUid = orderController.CurrentAuthority;
                room.Get(orderController.CurrentAuthority).isFangPao = true;
            }
            else
            {
                gamer.isZimo = true;
            }

            //硬花
            actorGamerHuPai.YingHuaCount = handCards.FaceCards.Count;
            //硬花
            huaCount += handCards.FaceCards.Count;
            //软花
            foreach (var pengorbar in handCards.PengOrBars)
            {
                //东南西北风 碰牌
                if (pengorbar.OperateType == OperateType.Peng)
                {
                    if (pengorbar.Weight >= 31 && pengorbar.Weight <= 37)
                    {
                        actorGamerHuPai.RuanHuaCount += 1;
                    }
                }
                else
                {
                    if (pengorbar.BarType == BarType.DarkBar)
                    {
                        //风牌暗杠
                        if (pengorbar.Weight >= 31 && pengorbar.Weight <= 37)
                        {
                            actorGamerHuPai.RuanHuaCount += 3;
                        }
                        //万条筒暗杠
                        else
                        {
                            actorGamerHuPai.RuanHuaCount += 2;
                        }
                    }
                    else
                    {
                        //风牌明杠
                        if (pengorbar.Weight >= 31 && pengorbar.Weight <= 37)
                        {
                            actorGamerHuPai.RuanHuaCount += 2;
                        }
                        //万条筒明杠
                        else
                        {
                            actorGamerHuPai.RuanHuaCount += 1;
                        }
                    }
                }
            }

            int fengKeHuaShu = Logic_NJMJ.getInstance().getFengKeHuaShu(mahjongInfos);

            //将碰刚将入手牌
            foreach (var pengorbar in handCards.PengOrBars)
            {
                if (pengorbar.OperateType == OperateType.Peng)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        mahjongInfos.Add(new MahjongInfo()
                        {
                            m_weight = (Consts.MahjongWeight)pengorbar.Weight,
                            weight = (byte)pengorbar.Weight
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        mahjongInfos.Add(new MahjongInfo()
                        {
                            m_weight = (Consts.MahjongWeight)pengorbar.Weight,
                            weight = (byte)pengorbar.Weight
                        });
                    }
                }
            }

            int queYiMenHuaShu = Logic_NJMJ.getInstance().getQueYiMenHuaShu(mahjongInfos);

            actorGamerHuPai.RuanHuaCount += queYiMenHuaShu;
            actorGamerHuPai.RuanHuaCount += fengKeHuaShu;

           huaCount += actorGamerHuPai.RuanHuaCount;
            //胡牌类型
            foreach (var type in huPaiTypes)
            {
                actorGamerHuPai.HuPaiTypes.Add((int) type);
            }   

            room.Broadcast(actorGamerHuPai);

            room.IsGameOver = true;
            gamer.IsCanHu = false;
            gamer.IsWinner = true;

            foreach (var item in huPaiTypes)
            {
                Log.Info("有人胡牌:" + item.ToString());
            }

            Log.Info("huPaiNeedData:" + JsonHelper.ToJson(huPaiNeedData));

            //设置胡牌的花数
            for (int j = 0; j < huPaiTypes.Count; j++)
            {
                Consts.HuPaiType huPaiType = (Consts.HuPaiType) huPaiTypes[j];
                int count;
                Logic_NJMJ.getInstance().HuPaiHuaCount.TryGetValue(huPaiType, out count);
                //胡牌花
                huaCount += count;
            }

            //基数
            huaCount += 20;
            huaCount += actorGamerHuPai.BixiaHuCount;
            huaCount *= 2;

            return huaCount;
        }

        private static bool GetCanHu(Room room,Gamer selfGamer)
        {
            foreach (var _gamer in room.GetAll())
            {
                if (selfGamer.UserID == _gamer.UserID)
                {
                    continue;
                }
                if (_gamer.IsCanHu)
                {
                    return true;
                }
            }

            return false;
        }
    }
}