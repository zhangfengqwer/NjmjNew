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
	        PlayCard(gamer, message);
	    }

	    public static async void PlayCard(Gamer gamer, Actor_GamerPlayCard message)
	    {
	        try
	        {
	            MahjongInfo mahjongInfo = new MahjongInfo()
	            {
	                weight = (byte) message.weight,
	                m_weight = (Consts.MahjongWeight) message.weight
	            };

	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);

	            OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
	            DeskComponent deskComponent = room.GetComponent<DeskComponent>();

                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
	            List<MahjongInfo> mahjongInfos = handCardsComponent.GetAll();

	            if (orderController.CurrentAuthority != gamer.UserID)
	            {
	                Log.Warning("没有轮到当前玩家出牌:" + gamer.UserID);
	                Log.Warning("当前出牌玩家:" + orderController.CurrentAuthority);
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
	                Log.Debug("玩家出牌:" + message.weight);
                    //当前出的牌
	                deskComponent.CurrentCard = mahjongInfo;
	                deskComponent.CurrentAuthority = gamer.UserID;
	                handCardsComponent.PlayCards.Add(mahjongInfo);
                    mahjongInfos.RemoveAt(index);

	                room.Broadcast(new Actor_GamerPlayCard()
	                {
	                    weight = message.weight,
	                    Uid = gamer.UserID,
	                    index = message.index
	                });

                    //等待客户端有没有人碰
	                Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
	                bool isNeedWait = false;

	                foreach (var _gamer in room.GetAll())
	                {
	                    if (_gamer.UserID == gamer.UserID)
	                        continue;

	                    List<MahjongInfo> cards = _gamer.GetComponent<HandCardsComponent>().GetAll();
	                    canOperation.Uid = _gamer.UserID;

                        if (Logic_NJMJ.getInstance().isCanPeng(mahjongInfo, cards))
	                    {
                            _gamer.IsCanPeng = true;
                            isNeedWait = true;

                            canOperation.OperationType = 0;

	                        room.GamerBroadcast(_gamer, canOperation);
	                    }

	                    if (Logic_NJMJ.getInstance().isCanGang(mahjongInfo, cards))
	                    {
                            _gamer.IsCanGang = true;
	                        isNeedWait = true;
                            canOperation.OperationType = 1;
	                        room.GamerBroadcast(_gamer, canOperation);
                        }
                        if (room.CanHu(mahjongInfo, cards))
                        {
                            _gamer.IsCanHu = true;
                            isNeedWait = true;

                            canOperation.OperationType = 2;
                            room.GamerBroadcast(_gamer, canOperation);
                        }
	                }

	                if (isNeedWait)
	                {
	                    room.StartOperateTime();
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
	        }
	        catch (Exception e)
	        {
	            Log.Error(e);
	        }
	    }
	}
}