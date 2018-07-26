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
	        await GamerReady(gamer, message);
	    }

	    public static async Task GamerReady(Gamer gamer, Actor_GamerReady message)
	    {
	        try
	        {
	            Log.Info($"收到玩家{gamer.UserID}准备");
	            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
	            Room room = roomComponent.Get(gamer.RoomID);

	            if (room == null || gamer == null || gamer.IsReady || room.State == RoomState.Game)
	            {
	                return;
	            }

	            gamer.IsReady = true;
	            //消息广播给其他人
	            room?.Broadcast(new Actor_GamerReady() {Uid = gamer.UserID});

	            Gamer[] gamers = room.GetAll();
	            //房间内有4名玩家且全部准备则开始游戏
	            if (room.Count == 4 && gamers.Where(g => g.IsReady).Count() == 4)
	            {
	                room.State = RoomState.Game;
	                room.CurrentJuCount++;
                    room.IsGameOver = false;
	                room.RoomDispose();

	                #region 好友房扣钥匙

	                if (room.IsFriendRoom && room.CurrentJuCount == 1)
	                {
	                    RoomConfig roomConfig = room.GetComponent<GameControllerComponent>().RoomConfig;
	                    await DBCommonUtil.DeleteFriendKey(roomConfig.MasterUserId, roomConfig.KeyCount, "好友房房主扣除钥匙");
	                }
	                #endregion
                    //设置比下胡
                    if (room.LastBiXiaHu)
	                {
                        room.IsBiXiaHu = true;
                    }
	                else
	                {
	                    room.IsBiXiaHu = false;
                    }
	                room.LastBiXiaHu = false;


                    if (roomComponent.gameRooms.TryGetValue(room.Id, out var itemRoom))
	                {
	                    roomComponent.gameRooms.Remove(room.Id);
	                }

	                roomComponent.gameRooms.Add(room.Id, room);
	                roomComponent.idleRooms.Remove(room.Id);
	                //添加用户
	                room.UserIds.Clear();
	                //初始玩家开始状态
	                foreach (var _gamer in gamers)
	                {
	                    room.UserIds.Add(_gamer.UserID);

	                    if (_gamer.GetComponent<HandCardsComponent>() == null)
	                    {
	                        _gamer.AddComponent<HandCardsComponent>();
	                    }

	                    _gamer.IsReady = false;
	                }

	                Log.Info($"{room.Id}房间开始,玩家:{JsonHelper.ToJson(room.UserIds)}");


                    GameControllerComponent gameController = room.GetComponent<GameControllerComponent>();
	                OrderControllerComponent orderController = room.GetComponent<OrderControllerComponent>();
	                DeskComponent deskComponent = room.GetComponent<DeskComponent>();

	                Gamer bankerGamer = null;
	                HandCardsComponent bankerHandCards = null;
	                if (room.IsLianZhuang)
	                {
	                    if (room.huPaiUid != 0 && room.huPaiUid == room.BankerGamer?.UserID)
	                    {
	                        bankerGamer = room.Get(room.huPaiUid);
	                        bankerHandCards = bankerGamer.GetComponent<HandCardsComponent>();
	                        bankerHandCards.IsBanker = true;
	                        room.BankerGamer = bankerGamer;
                            //连庄
                            room.LastBiXiaHu = true;
                        }
	                    else
	                    {
	                        int gamerSeat = room.GetGamerSeat(room.BankerGamer.UserID);
	                        int currentSeat;
	                        if (gamerSeat == 3)
	                        {
	                            currentSeat = 0;
	                        }
	                        else
	                        {
	                            currentSeat = ++gamerSeat;
	                        }

	                        bankerGamer = room.gamers[currentSeat];
	                        bankerHandCards = bankerGamer.GetComponent<HandCardsComponent>();
	                        bankerHandCards.IsBanker = true;
	                        room.BankerGamer = bankerGamer;
	                    }
	                }
	                else
	                {
	                    if (room.IsFriendRoom)
	                    {
	                        GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
	                        long masterUserId = controllerComponent.RoomConfig.MasterUserId;
	                        bankerGamer = room.Get(masterUserId);
                        }
	                    else
	                    {
	                        //随机庄家
	                        int number = RandomHelper.RandomNumber(0, 12);
	                        int i = number % 4;
	                        bankerGamer = room.gamers[i];
                        }
	                    bankerHandCards = bankerGamer.GetComponent<HandCardsComponent>();
	                    bankerHandCards.IsBanker = true;
	                    room.BankerGamer = bankerGamer;
                    }

	                //发牌
	                gameController.DealCards();
	                orderController.Start(bankerGamer.UserID);

	                //去除花牌
	                foreach (var _gamer in gamers)
	                {
	                    HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
	                    List<MahjongInfo> handCards = handCardsComponent.GetAll();

	                    for (int j = handCards.Count - 1; j >= 0; j--)
	                    {
	                        MahjongInfo mahjongInfo = handCards[j];

	                        if (mahjongInfo.m_weight >= Consts.MahjongWeight.Hua_HongZhong)
	                        {
	                            handCards.RemoveAt(j);
	                            mahjongInfo.weight = (byte) mahjongInfo.m_weight;
	                            handCardsComponent.FaceCards.Add(mahjongInfo);
	                        }
	                    }

	                    //加牌
	                    int handCardsCount = handCards.Count;
	                    for (int j = 0; j < 13 - handCardsCount; j++)
	                    {
	                        GetCardNotFace(deskComponent, handCardsComponent);
	                    }
	                }

	                //庄家多发一张牌
	                GetCardNotFace(deskComponent, bankerHandCards);

//	                List<MahjongInfo> infos = bankerHandCards.GetAll();
//	                bankerHandCards.library = new List<MahjongInfo>(list);

	                //给客户端传送数据
	                Actor_StartGame actorStartGame = new Actor_StartGame();
	                foreach (var itemGame in gamers)
	                {
	                    GamerData gamerData = new GamerData();
	                    HandCardsComponent handCardsComponent = itemGame.GetComponent<HandCardsComponent>();

	                    List<MahjongInfo> mahjongInfos = handCardsComponent.library;
	                    foreach (var mahjongInfo in mahjongInfos)
	                    {
	                        mahjongInfo.weight = (byte) mahjongInfo.m_weight;
	                    }

	                    gamerData.UserID = itemGame.UserID;
	                    gamerData.handCards = mahjongInfos;
	                    gamerData.faceCards = handCardsComponent.FaceCards;
	                    if (handCardsComponent.IsBanker)
	                    {
	                        gamerData.IsBanker = true;
	                    }

	                    gamerData.SeatIndex = room.seats[itemGame.UserID];
	                    gamerData.OnlineSeconds = await DBCommonUtil.GetRestOnlineSeconds(itemGame.UserID);
	                    actorStartGame.GamerDatas.Add(gamerData);
	                }

	                actorStartGame.restCount = deskComponent.RestLibrary.Count;
	                GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
	               
	                if (room.IsFriendRoom)
	                {
	                    actorStartGame.RoomType = 3;
	                    actorStartGame.CurrentJuCount = room.CurrentJuCount;
	                }
	                else
	                {
	                    actorStartGame.RoomType = (int)gameControllerComponent.RoomConfig.Id;
                    }
                    //发送消息
                    room.Broadcast(actorStartGame);
//	                Log.Debug("发送开始：" + JsonHelper.ToJson(actorStartGame));

	                //排序
	                var startTime = DateTime.Now;
	                foreach (var _gamer in gamers)
	                {
	                    HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();
	                    //排序
	                    handCardsComponent.Sort();
	                    handCardsComponent.GrabCard = handCardsComponent.GetAll()[handCardsComponent.GetAll().Count - 1];
	                    //设置玩家在线开始时间
	                    _gamer.StartTime = startTime;
	                    //await DBCommonUtil.RecordGamerTime(startTime, true, _gamer.UserID);
	                }

	                foreach (var _gamer in room.GetAll())
	                {
	                    HandCardsComponent handCardsComponent = _gamer.GetComponent<HandCardsComponent>();

	                    List<MahjongInfo> cards = handCardsComponent.GetAll();

	                    if (handCardsComponent.IsBanker)
	                    {
	                        if (Logic_NJMJ.getInstance().isHuPai(cards))
	                        {
	                            //ToDo 胡牌
	                            Actor_GamerCanOperation canOperation = new Actor_GamerCanOperation();
	                            canOperation.Uid = _gamer.UserID;
	                            _gamer.IsCanHu = true;
	                            canOperation.OperationType = 2;
	                            room.GamerBroadcast(_gamer, canOperation);
	                            _gamer.isGangFaWanPai = true;
	                        }
	                    }
	                    else
	                    {
	                        //检查听牌
	                        List<MahjongInfo> checkTingPaiList = Logic_NJMJ.getInstance().checkTingPaiList(cards);
	                        if (checkTingPaiList.Count > 0)
	                        {
	                            Log.Info($"{_gamer.UserID}听牌:");
	                            _gamer.isFaWanPaiTingPai = true;
	                        }
	                    }
	                }

	                //等客户端掷骰子
	                //是否超时
	                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(10 * 1000);
                    room.StartTime();
	                //扣服务费
	                if (!room.IsFriendRoom)
	                {
	                    GameHelp.CostServiceCharge(room);
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
        /// 发牌（不包括花牌）
        /// </summary>
        /// <param name="deskComponent"></param>
        /// <param name="handCardsComponent"></param>
	    private static void GetCardNotFace(DeskComponent deskComponent,HandCardsComponent handCardsComponent)
	    {
	        while (true)
	        {
	            int cardIndex = RandomHelper.RandomNumber(0, deskComponent.RestLibrary.Count);
	            MahjongInfo grabMahjong = deskComponent.RestLibrary[cardIndex];

	            deskComponent.RestLibrary.RemoveAt(cardIndex);

                //花牌
                if (grabMahjong.m_weight >= Consts.MahjongWeight.Hua_HongZhong)
	            {
	                handCardsComponent.FaceCards.Add(grabMahjong);
                }
	            else
	            {
	                handCardsComponent.GetAll().Add(grabMahjong);
                    break;
                }
            }
	    }
	}
}