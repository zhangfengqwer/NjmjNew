using System;
using System.Collections.Generic;
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
	        if (room == null) return;

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
	                gamer.IsTrusteeship = false;
                    //Log.Info($"玩家{gamer.UserID}出牌:" + message.weight);
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
	                room.reconnectList.Add(actorGamerPlayCard);

                    gamer.IsCanHu = false;
	                gamer.IsCanPeng = false;
	                gamer.IsCanGang = false;

                    //等待客户端有没有人碰
	                bool isNeedWait = false;

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
	                        Log.Info($"{_gamer.UserID}可碰:"+JsonHelper.ToJson(canOperation));
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
	                        Log.Info($"{_gamer.UserID}可杠" + JsonHelper.ToJson(canOperation));
                            room.GamerBroadcast(_gamer, canOperation);
                        }

                        //判断小胡,4个花以上才能胡
                        if (currentCards.PengGangCards.Count > 0 || currentCards.PengCards.Count > 0)
	                    {
                            if (currentCards.FaceCards.Count >= 4)
	                        {
	                            if (room.CanHu(mahjongInfo, cards))
	                            {
	                                Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
	                                canOperation.Uid = _gamer.UserID;

                                    _gamer.IsCanHu = true;
	                                isNeedWait = true;

	                                canOperation.OperationType = 2;
	                                room.GamerBroadcast(_gamer, canOperation);
                                }
	                            else
	                            {
//	                                Log.Debug("buneng");
                                }
	                        }
	                    }
	                    else
	                    {
	                        if (room.CanHu(mahjongInfo, cards))
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

	                if (isNeedWait)
	                {
	                    await room.StartOperateTime();
                    }
                    //没人可以操作就直接发牌
	                else
	                {
	                    //发牌
	                    room.GamerGrabCard();
                    }
                }
	            else
	            {
	                Log.Warning("玩家出牌不存在:" + message.weight);
	            }

	            room.IsPlayingCard = false;
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