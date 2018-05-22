﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerOperateHandler : AMActorHandler<Gamer,Actor_GamerOperation>
	{
	    protected override async Task Run(Gamer gamer, Actor_GamerOperation message)
	    {
	        await Task.CompletedTask;
            try
            {
                Log.Debug("有人碰或刚");
                MahjongInfo mahjongInfo = new MahjongInfo() {weight = (byte) message.weight, m_weight = (Consts.MahjongWeight) message.weight};

                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Room room = roomComponent.Get(gamer.RoomID);
                room.IsOperate = true;

                DeskComponent deskComponent = room.GetComponent<DeskComponent>();
                OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
                HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                List<MahjongInfo> mahjongInfos = handCards.GetAll();

//                if (orderController.CurrentAuthority != gamer.UserID)
//                {
//                    Log.Warning("没有轮到当前玩家出牌:" + gamer.UserID);
//                    Log.Warning("当前出牌玩家:" + orderController.CurrentAuthority);
//                    return;
//                }

                //胡牌
                if (message.OperationType == 2)
                {
                    //自摸
                    if (orderController.CurrentAuthority == gamer.UserID)
                    {
                        if (Logic_NJMJ.getInstance().isHuPai(mahjongInfos))
                        {
                            HuPai(gamer, room, mahjongInfos);
                        }
                    }
                    //放炮
                    else
                    {
                        if (room.CanHu(deskComponent.CurrentCard, mahjongInfos))
                        {
                            List<MahjongInfo> temp = new List<MahjongInfo>(mahjongInfos);
                            temp.Add(deskComponent.CurrentCard);
                            HuPai(gamer, room, temp);
                        }
                    }
                 
                }
                //放弃
                else if (message.OperationType == 3)
                {
                    room.tokenSource.Cancel();
                    room.GamerGrabCard();
                }
                else
                {
                    Actor_GamerOperation gamerOperation = new Actor_GamerOperation();
                    gamerOperation.Uid = gamer.UserID;
                    gamerOperation.weight = deskComponent.CurrentCard.weight;

                    //有没有人胡牌
                    while (true)
                    {
//                        await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);

                        if (!GetCanHu(room))
                        {
                            break;
                        }
                    }

                    //游戏结束
                    if (room.IsGameOver) return;

                    //碰
                    if (message.OperationType == 0)
                    {
                        if (Logic_NJMJ.getInstance().isCanPeng(deskComponent.CurrentCard, mahjongInfos))
                        {
                            gamerOperation.OperationType = 0;
                            room.Broadcast(gamerOperation);

                            //更新手牌
                            for (int i = 0; i < 2; i++)
                            {
                                int index = Logic_NJMJ.getInstance().GetIndex(mahjongInfos, deskComponent.CurrentCard);
                                mahjongInfos.RemoveAt(index);
                            }

                            handCards.PengCards.Add(deskComponent.CurrentCard);
                        }
                    }
                    //杠
                    else if (message.OperationType == 1)
                    {
                        if (Logic_NJMJ.getInstance().isCanGang(deskComponent.CurrentCard, mahjongInfos))
                        {
                            gamerOperation.OperationType = 1;
                            room.Broadcast(gamerOperation);

                            //更新手牌
                            for (int i = 0; i < 3; i++)
                            {
                                int index = Logic_NJMJ.getInstance().GetIndex(mahjongInfos, deskComponent.CurrentCard);
                                mahjongInfos.RemoveAt(index);
                            }

                            handCards.GangCards.Add(deskComponent.CurrentCard);

                            //杠完之后不能
                            gamer.isFaWanPaiTingPai = false;
                            //杠完之后抓牌
                            room.isGangEndBuPai = true;
                            room.isGetYingHuaBuPai = false;
                            room.GrabMahjong();
                        }
                    }

                    //碰完当前玩家出牌
                    orderController.CurrentAuthority = gamer.UserID;
                    room.StartTime();
                }
            }
	        catch (Exception e)
	        {
	            Log.Error(e);
            }
	    }

        /// <summary>
        ///  胡牌
        /// </summary>
        /// <param name="gamer"></param>
        /// <param name="room"></param>
        /// <param name="mahjongInfos"></param>
	    private static void HuPai(Gamer gamer, Room room, List<MahjongInfo> mahjongInfos)
	    {
	        DeskComponent deskComponent = room.GetComponent<DeskComponent>();
	        OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
	        HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();

            room.tokenSource.Cancel();

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
	            if (_gamer.UserID == gamer.UserID)
	                continue;
	            HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
	            temp.Add(handCardsComponent.PengCards);

	            //设置其他人的牌
                GamerData gamerData = new GamerData();
	            gamerData.handCards = handCardsComponent.GetAll();
	            actorGamerHuPai.GamerDatas.Add(gamerData);
	        }

	        huPaiNeedData.other1_pengList = temp[0];
	        huPaiNeedData.other2_pengList = temp[1];
	        huPaiNeedData.other3_pengList = temp[2];

            List<Consts.HuPaiType> huPaiTypes = Logic_NJMJ.getInstance().getHuPaiType(mahjongInfos, huPaiNeedData);

	        room.Broadcast(actorGamerHuPai);

	        room.IsGameOver = true;
	        gamer.IsCanHu = false;
	        Log.Info("huPaiNeedData:" + JsonHelper.ToJson(huPaiNeedData));
	        foreach (var item in huPaiTypes)
	        {
	            Log.Info("有人胡牌:" + item.ToString());
            }
	    }

	    private static bool GetCanHu(Room room)
	    {
	        foreach (var _gamer in room.GetAll())
	        {
	            if (_gamer.IsCanHu)
	            {
	                return true;
	            }
	        }
	        return false;
	    }
	}
}