using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerReconnectHandler : AMHandler<Actor_GamerReconnet>
    {
        protected override async void Run(Session session, Actor_GamerReconnet message)
        {
            try
            {
                Log.Info($"断线重连:");
                SoundsHelp.Instance.SoundMute(true);

                GameObject mask = GameObject.Instantiate(CommonUtil.getGameObjByBundle("Image_Desk_Card", "RoomMask"),GameObject.Find("Global/UI/CommonCanvas").transform);

                //进入
                List<GamerInfo> Gamers = new List<GamerInfo>();
                foreach (var item in message.Gamers)
                {
                    GamerInfo gamerInfo = new GamerInfo();
                    gamerInfo.UserID = item.UserID;
                    gamerInfo.SeatIndex = item.SeatIndex;
                    gamerInfo.IsReady = true;
                    gamerInfo.playerInfo = item.playerInfo;
                    Gamers.Add(gamerInfo);

                    //将出的牌加入到手牌中
                    item.handCards.AddRange(item.playCards);

                    foreach (var card in item.pengCards)
                    {
                        item.handCards.Add(card);
                        item.handCards.Add(card);
                    }

                    foreach (var card in item.gangCards)
                    {
                        item.handCards.Add(card);
                        item.handCards.Add(card);
                        item.handCards.Add(card);
                    }

                    Logic_NJMJ.getInstance().SortMahjong(item.handCards);

                    Log.Debug($"{item.UserID} 手牌：{item.handCards.Count}");
                }

                await Actor_GamerEnterRoomHandler.GamerEnterRoom(new Actor_GamerEnterRoom() { Gamers = Gamers });

                //开始游戏
                var actorStartGame = new Actor_StartGame();
                actorStartGame.GamerDatas = message.Gamers;
                actorStartGame.restCount = message.RestCount;
                actorStartGame.RoomType = message.RoomType;
                Actor_StartGameHandler.StartGame(actorStartGame);

                //碰刚
                foreach (var item in message.Gamers)
                {
                    foreach (var card in item.pengCards)
                    {
                        Actor_GamerOperation gamerOperation = new Actor_GamerOperation();
                        gamerOperation.Uid = item.UserID;
                        gamerOperation.weight = card.weight;
                        gamerOperation.OperationType = 0;
                        Actor_GamerOperateHandler.GamerOperation(gamerOperation);

                    }

                    foreach (var card in item.gangCards)
                    {
                        Actor_GamerOperation gamerOperation = new Actor_GamerOperation();
                        gamerOperation.Uid = item.UserID;
                        gamerOperation.weight = card.weight;
                        gamerOperation.OperationType = 1;
                        Actor_GamerOperateHandler.GamerOperation(gamerOperation);
                    }
                }

                //打牌
                foreach (var item in message.Gamers)
                {
                    Log.Debug($"{item.UserID} 重连出牌");
                    for (int i = 0; i < item.playCards.Count; i++)
                    {
                        MahjongInfo card = item.playCards[i];
                        int index = Logic_NJMJ.getInstance().GetIndex(item.handCards, card);
                        Actor_GamerPlayCard playCard = new Actor_GamerPlayCard();
                        playCard.Uid = item.UserID;
                        playCard.weight = card.weight;
                        playCard.index = index;
//                        await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(100);
                        Actor_GamerPlayCardHandler.PlayCard(playCard);
//                        item.handCards.RemoveAt(index);
                    }
                }
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(3000);
                bool b = PlayerPrefs.GetInt("isOpenSound", 0) == 0;
                SoundsHelp.Instance.SoundMute(b);
                GameObject.Destroy(mask);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
