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

                MahjongInfo info = mahjongInfos[message.index];
                if (info.weight == message.weight)
                {
                    Log.Debug("玩家出牌:" + message.weight);

                    mahjongInfos.RemoveAt(message.index);

                    room.Broadcast(new Actor_GamerPlayCard()
                    {
                        weight = message.weight,
                        Uid = gamer.UserID,
                        index = message.index
                    });

                    //下一个人出牌
//                    orderController.Turn();
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