using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Actor_GamerReadyHandler : AMActorHandler<Gamer,Actor_GamerReady>
	{

	    protected override async Task Run(Gamer gamer, Actor_GamerReady message)
	    {
            try
	        {
	            Log.Info("收到玩家准备：" + JsonHelper.ToJson(message));
	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);
	           
                gamer.IsReady = true;
	            //消息广播给其他人
	            room.Broadcast(new Actor_GamerReady() { Uid = gamer.UserID });
	          
                Gamer[] gamers = room.GetAll();
                //房间内有4名玩家且全部准备则开始游戏
	            if (room.Count == 4 && gamers.Where(g => g.IsReady).Count() == 4)
	            {
	                //初始玩家开始状态
	                foreach (var _gamer in gamers)
	                {
	                    if (_gamer.GetComponent<HandCardsComponent>() == null)
	                    {
	                        _gamer.AddComponent<HandCardsComponent>();
	                    }
	                    _gamer.IsReady = false;
	                }

	                GameControllerComponent gameController = room.GetComponent<GameControllerComponent>();
	                OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();

                    //随机庄家
	                int number = RandomHelper.RandomNumber(0, 12);
	                int i = number % 4;
                    room.gamers[i].GetComponent<HandCardsComponent>().IsBanker = true;

	                //发牌
                    gameController.DealCards();
	                orderController.Start(gamers[0].UserID);
                    //给客户端传送数据
                    foreach (var itemGame in gamers)
	                {
	                    List<MahjongInfo> mahjongInfos = itemGame.GetComponent<HandCardsComponent>().library;
	                    foreach (var mahjongInfo in mahjongInfos)
	                    {
	                        mahjongInfo.weight = (byte) mahjongInfo.m_weight;
	                    }

	                    ActorProxy actorProxy = itemGame.GetComponent<UnitGateComponent>().GetActorProxy();
                        actorProxy.Send(new Actor_StartGame()
                        {
                            Mahjongs = itemGame.GetComponent<HandCardsComponent>().GetAll()
                        });
                    }

	                room.State = RoomState.Game;

	                if (roomComponent.gameRooms.TryGetValue(room.Id, out var itemRoom))
	                {
	                    roomComponent.gameRooms.Remove(room.Id);
	                }

                    roomComponent.gameRooms.Add(room.Id, room);
	                roomComponent.readyRooms.Remove(room.Id);


                    //排序
	                foreach (var _gamer in gamers)
	                {
	                    HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
	                    handCardsComponent.Sort();
                    }

                    //判断停牌，胡牌
	                foreach (var _gamer in gamers)
	                {
	                    HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
                        if (handCardsComponent.IsBanker)
	                    {
                            //庄家胡牌
	                        if (Logic_NJMJ.getInstance().isHuPai(handCardsComponent.GetAll()))
	                        {

	                        }
	                    }
	                    else
	                    {
                            //其他听牌
	                        List<MahjongInfo> checkTingPaiList = Logic_NJMJ.getInstance().checkTingPaiList(handCardsComponent.GetAll());
	                        if (checkTingPaiList.Count > 0)
	                        {

	                        }
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
	}
}