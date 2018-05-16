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

                    mahjongInfos.RemoveAt(index);

                    room.Broadcast(new Actor_GamerPlayCard()
                    {
                        weight = message.weight,
                        Uid = gamer.UserID,
                        index = message.index
                    });

                    //下一个人出牌
                    orderController.Turn();
                    var currentGamer = room.Get(orderController.CurrentAuthority);
                    HandCardsComponent cardsComponent = currentGamer.GetComponent<HandCardsComponent>();
                    DeskComponent deskComponent = room.GetComponent<DeskComponent>();

                    int number = RandomHelper.RandomNumber(0, deskComponent.RestLibrary.Count);
                    MahjongInfo grabMahjong = deskComponent.RestLibrary[number];
                    //发牌
                    cardsComponent.AddCard(grabMahjong);
                    deskComponent.RestLibrary.RemoveAt(number);

                    Logic_NJMJ.getInstance().SortMahjong(cardsComponent.GetAll());

                    //发送抓牌消息
                    foreach (Gamer _gamer in room.gamers)
                    {
                        if (_gamer == null || _gamer.isOffline)
                        {
                            continue;
                        }
                        ActorProxy actorProxy = _gamer.GetComponent<UnitGateComponent>().GetActorProxy();
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
                        actorProxy.Send(actorGamerGrabCard);
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