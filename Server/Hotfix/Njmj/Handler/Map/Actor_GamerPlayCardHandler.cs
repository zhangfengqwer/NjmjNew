using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerPlayCardHandler : AMActorHandler<Gamer, Actor_GamerPlayCard>
	{
	    protected override async Task Run(Gamer gamer, Actor_GamerPlayCard message)
	    {
	        await PlayCard(gamer, message);
	    }

	    public static async Task PlayCard(Gamer gamer, Actor_GamerPlayCard message)
	    {
	        RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	        Room room = roomComponent.Get(gamer.RoomID);
	        if (room == null || room.IsGameOver) return;

            try
            {
	            MahjongInfo mahjongInfo = new MahjongInfo()
	            {
	                weight = (byte) message.weight,
	                m_weight = (Consts.MahjongWeight) message.weight
	            };

                //加锁
	            if (room.IsPlayingCard)
	            {
	                Log.Warning("当前正在出牌，不能再出牌了");
	                return;
	            }
                room.IsPlayingCard = true;

                GameControllerComponent gameController = room.GetComponent<GameControllerComponent>();
	            OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
	            DeskComponent deskComponent = room.GetComponent<DeskComponent>();

                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
	            if (handCardsComponent == null) return;
	            List<MahjongInfo> mahjongInfos = handCardsComponent.GetAll();

	            if (orderController.CurrentAuthority != gamer.UserID)
	            {
	                Log.Warning("没有轮到当前玩家出牌:" + gamer.UserID);
	                Log.Warning("当前出牌玩家:" + orderController.CurrentAuthority);
	                room.IsPlayingCard = false;
                    return;
	            }

	            int index = -1;
	            for (int i = 0; i < mahjongInfos.Count; i++)
	            {
	                if (mahjongInfos[i].m_weight == mahjongInfo.m_weight)
	                {
	                    index = i;
	                    break;
	                }
	            }

	            if (index >= 0)
                {
	                //停止倒计时
                    room?.tokenSource?.Cancel();
                    Log.Info($"玩家{gamer.UserID}出牌:" + mahjongInfo.m_weight);
                    //当前出的牌
	                deskComponent.CurrentCard = mahjongInfo;
	                deskComponent.CurrentAuthority = gamer.UserID;
	                handCardsComponent.PlayCards.Add(mahjongInfo);
                    mahjongInfos.RemoveAt(index);
	                room.my_lastMahjong = mahjongInfo;
                    Actor_GamerPlayCard actorGamerPlayCard = new Actor_GamerPlayCard()
	                {
	                    weight = message.weight,
	                    Uid = gamer.UserID,
	                    index = message.index
	                };

                    room.Broadcast(actorGamerPlayCard);

                    gamer.IsCanHu = false;
	                gamer.IsCanPeng = false;
	                gamer.IsCanGang = false;

                    #region 4个人连续出同样的牌，第一个出牌的人立即支付其他三人

                    foreach (var _gamer in room.GetAll())
                    {
                        
                    }

                    #endregion

                    #region 一个人出4张一样的牌
                    //4个人出一样的牌
                    int temp = 0;
                    foreach (var playCard in handCardsComponent.PlayCards)
                    {
                        if (playCard.m_weight == mahjongInfo.m_weight)
                        {
                            temp++;
                        }
                    }
                    if (temp == 4)
                    {
                        //罚分
                        foreach (var _gamer in room.GetAll())
                        {
                            if (_gamer.UserID == gamer.UserID)
                            {
                                GameHelp.ChangeGamerGold(room, _gamer, - 10 * gameController.RoomConfig.Multiples * 3);
                            }
                            else
                            {
                                GameHelp.ChangeGamerGold(room, _gamer, 10 * gameController.RoomConfig.Multiples);
                            }
                        }

                        room.LastBiXiaHu = true;
                    }
                    #endregion

                    #region 一人前四次出牌打出东南西北（不必按顺序），则其他每名玩家立即支付给该玩家

                    if (handCardsComponent.PlayCards.Count == 4)
                    {
                        int Feng_Dong = handCardsComponent.PlayCards.Count(a => a.m_weight == Consts.MahjongWeight.Feng_Dong);
                        int Feng_Nan = handCardsComponent.PlayCards.Count(a => a.m_weight == Consts.MahjongWeight.Feng_Nan);
                        int Feng_Xi = handCardsComponent.PlayCards.Count(a => a.m_weight == Consts.MahjongWeight.Feng_Xi);
                        int Feng_Bei = handCardsComponent.PlayCards.Count(a => a.m_weight == Consts.MahjongWeight.Feng_Bei);

                        if (Feng_Dong == 1 && Feng_Nan == 1 && Feng_Xi == 1 && Feng_Bei == 1)
                        {
                            //四连风
                            foreach (var _gamer in room.GetAll())
                            {
                                if (_gamer.UserID == gamer.UserID)
                                {
                                    GameHelp.ChangeGamerGold(room, _gamer, 5 * gameController.RoomConfig.Multiples * 3);
                                }
                                else
                                {
                                    GameHelp.ChangeGamerGold(room, _gamer, -5 * gameController.RoomConfig.Multiples);
                                }
                            }
                        }

                    }

                    #endregion

                    #region 等待客户端有没有人碰杠胡   
                    //等待客户端有没有人碰
                    bool isNeedWait = false;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
	                foreach (var _gamer in room.GetAll())
	                {
	                    if (_gamer == null)
	                        continue;
	                    if (_gamer.UserID == gamer.UserID)
	                        continue;

	                    HandCardsComponent currentCards = _gamer.GetComponent<HandCardsComponent>();

	                    List<MahjongInfo> cards = _gamer.GetComponent<HandCardsComponent>().GetAll();

                        if (Logic_NJMJ.getInstance().isCanPeng(mahjongInfo, cards)) 
	                    {
	                        Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
	                        canOperation.Uid = _gamer.UserID;

                            _gamer.IsCanPeng = true;
                            isNeedWait = true;
                            canOperation.OperationType = 0;
	                        //Log.Info($"{_gamer.UserID}可碰:"+JsonHelper.ToJson(canOperation));
                            room.GamerBroadcast(_gamer, canOperation);
                        }

                        //明杠
                        if (Logic_NJMJ.getInstance().isCanGang(mahjongInfo, cards))
	                    {
	                        Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
	                        canOperation.Uid = _gamer.UserID;

                            _gamer.IsCanGang = true;
	                        isNeedWait = true;
                            canOperation.OperationType = 1;
	                        //Log.Info($"{_gamer.UserID}可杠" + JsonHelper.ToJson(canOperation));
                            room.GamerBroadcast(_gamer, canOperation);
                        }

	                    if (room.CanHu(mahjongInfo, cards))
	                    {
	                         _gamer.huPaiNeedData.my_lastMahjong = room.my_lastMahjong;
	                         _gamer.huPaiNeedData.restMahjongCount = deskComponent.RestLibrary.Count;
	                         _gamer.huPaiNeedData.isSelfZhuaPai = orderController.CurrentAuthority == _gamer.UserID;
	                         _gamer.huPaiNeedData.isZhuangJia = currentCards.IsBanker;
	                         _gamer.huPaiNeedData.isGetYingHuaBuPai = _gamer.isGetYingHuaBuPai;
	                         _gamer.huPaiNeedData.isGangEndBuPai = _gamer.isGangEndBuPai;
	                         _gamer.huPaiNeedData.isGangFaWanPai = _gamer.isGangFaWanPai;
	                         _gamer.huPaiNeedData.isFaWanPaiTingPai = _gamer.isFaWanPaiTingPai;
	                         _gamer.huPaiNeedData.my_yingHuaList = currentCards.FaceCards;
	                         _gamer.huPaiNeedData.my_gangList = currentCards.GangCards;
	                         _gamer.huPaiNeedData.my_pengList = currentCards.PengCards;
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

	                        List<MahjongInfo> infos = new List<MahjongInfo>(cards);
	                        infos.Add(mahjongInfo);

	                        List<Consts.HuPaiType> huPaiTypes = Logic_NJMJ.getInstance().getHuPaiType(infos, _gamer.huPaiNeedData);

	                        Log.Info(JsonHelper.ToJson(_gamer.huPaiNeedData));
	                        Log.Info(JsonHelper.ToJson(huPaiTypes));

	                        if (huPaiTypes.Count > 0)
	                        {
	                            //判断小胡,4个花以上才能胡
                                if (huPaiTypes[0] == Consts.HuPaiType.Normal)
	                            {
	                                if (currentCards.PengGangCards.Count > 0 || currentCards.PengCards.Count > 0)
	                                {
	                                    if (currentCards.FaceCards.Count >= 4)
	                                    {
	                                        _gamer.IsCanHu = true;
	                                        isNeedWait = true;
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
                                    isNeedWait = true;
                                    Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
                                    canOperation.Uid = _gamer.UserID;
                                    canOperation.OperationType = 2;
                                    room.GamerBroadcast(_gamer, canOperation);
                                }
	                        }
	                    }
	                }

                    sw.Stop();
                    Log.Info("判断碰刚胡时间:" + sw.ElapsedMilliseconds);

                    #endregion

                    if (isNeedWait)
	                {
	                    room.IsNeedWaitOperate = true;
                        room.StartOperateTime();
                    }
                    //没人可以操作就直接发牌
	                else
	                {
	                    room.IsNeedWaitOperate = false;
                        //发牌
                        room.GamerGrabCard();
	                    room.IsPlayingCard = false;
                    }
                }
	            else
	            {
	                Log.Warning("玩家出牌不存在:" + message.weight);
	                room.IsPlayingCard = false;

                }
            }
	        catch (Exception e)
	        {
	            Log.Error(e);
	            room.IsPlayingCard = false;
            }

	        await Task.CompletedTask;
        }
	}
}