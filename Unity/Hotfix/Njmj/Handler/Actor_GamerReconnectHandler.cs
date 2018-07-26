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
        protected override async void Run(ETModel.Session session, Actor_GamerReconnet message)
        {
            try
            {
                Log.Info($"断线重连:");
                SoundsHelp.Instance.IsOpenSound(false);

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

                    Log.Info($"{item.UserID} 手牌：{item.handCards.Count}");
                }

                Actor_GamerEnterRoom actorGamerEnterRoom = new Actor_GamerEnterRoom()
                {
                        Gamers = Gamers,
                        RoomType = message.RoomType,
                };
                if (message.RoomType == 3)
                {
                    actorGamerEnterRoom.RoomId = message.RoomId;
                    actorGamerEnterRoom.MasterUserId = message.MasterUserId;
                    actorGamerEnterRoom.JuCount = message.JuCount;
                    actorGamerEnterRoom.Multiples = message.Multiples;
                }
                await Actor_GamerEnterRoomHandler.GamerEnterRoom(actorGamerEnterRoom);

                //开始游戏
                var actorStartGame = new Actor_StartGame();
                actorStartGame.GamerDatas = message.Gamers;
                actorStartGame.restCount = message.RestCount;
                actorStartGame.RoomType = message.RoomType;
                if (actorStartGame.RoomType == 3)
                {
                    actorStartGame.CurrentJuCount = message.CurrentJuCount;
                }

                Actor_StartGameHandler.StartGame(actorStartGame, true);

                //碰刚
                foreach (var item in message.Gamers)
                {
                    for (int i = 0; i < item.pengCards.Count; i++)
                    {
                        MahjongInfo card = item.pengCards[i];
                        Actor_GamerOperation gamerOperation = new Actor_GamerOperation();
                        gamerOperation.Uid = item.UserID;
                        gamerOperation.weight = card.weight;
                        gamerOperation.OperationType = 0;
                        gamerOperation.OperatedUid = item.OperatedPengUserIds[i];
                        Actor_GamerOperateHandler.GamerOperation(gamerOperation, true);
                    }

                    for (int i = 0; i < item.gangCards.Count; i++)
                    {
                        MahjongInfo card = item.gangCards[i];
                        long operatedGangUserIds = item.OperatedGangUserIds[i];
                        Actor_GamerOperation gamerOperation = new Actor_GamerOperation();
                        gamerOperation.Uid = item.UserID;
                        gamerOperation.weight = card.weight;
                        gamerOperation.OperationType = 1;
                        gamerOperation.OperatedUid = operatedGangUserIds;

                        if (operatedGangUserIds == 0)
                        {
                            gamerOperation.OperationType = 4;
                        }
                        Actor_GamerOperateHandler.GamerOperation(gamerOperation, true);
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

                //托管恢复
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                foreach (var item in message.Gamers)
                {
                    if (item.IsTrusteeship )
                    {
                        if (item.UserID == PlayerInfoComponent.Instance.uid)
                        {
                            uiRoomComponent.ShowTrustship();
                        }
                        else
                        {
                            GamerUIComponent gamerUIComponent = gamerComponent.Get(item.UserID).GetComponent<GamerUIComponent>();
                            gamerUIComponent.ShowTrust();
                        }
                    }
                }

                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(3000);
                int openSound = PlayerPrefs.GetInt("isOpenSound", 0);
                bool b = openSound == 1;
                SoundsHelp.Instance.IsOpenSound(b);
                GameObject.Destroy(mask);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
