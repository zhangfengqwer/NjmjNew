using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_StartGameHandler: AMHandler<Actor_StartGame>
    {
        protected override async void Run(Session session, Actor_StartGame message)
        {
            StartGame(message,false);
        }

        public static async void StartGame(Actor_StartGame message, bool isReconnect)
        {
            Log.Debug($"收到开始");

            try
            {
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);

                if (uiRoom == null) uiRoom = Game.Scene.GetComponent<UIComponent>().Create(UIType.UIRoom);

                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIReady);

                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                //先掷骰子
                if (!isReconnect)
                {
                    uiRoomComponent.exitBtn.interactable = false;
                    uiRoomComponent.players.SetActive(true);
                    GameObject DiceAnim = uiRoomComponent.dice.Get<GameObject>("DiceAnim");
                    GameObject DiceBottom = uiRoomComponent.dice.Get<GameObject>("DiceBottom");
                    Image Dice1 = DiceBottom.transform.Find("Dice1").GetComponent<Image>();
                    Image Dice2 = DiceBottom.transform.Find("Dice2").GetComponent<Image>();
                    DiceAnim.SetActive(true);
                    SoundsHelp.Instance.playSound_ShaiZi();
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(2000);
                    DiceAnim.SetActive(false);
                    DiceBottom.SetActive(true);
                    int number1 = RandomHelper.RandomNumber(1, 7);
                    Dice1.sprite = CommonUtil.getSpriteByBundle("Image_Dice", "num_" + number1);
                    int number2 = RandomHelper.RandomNumber(1, 7);
                    Dice2.sprite = CommonUtil.getSpriteByBundle("Image_Dice", "num_" + number2);
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                   

                    //发牌动画

                    List<MahjongInfo> myCard = null;
                    foreach (var gameData in message.GamerDatas)
                    {
                        foreach (var mahjong in gameData.handCards)
                        {
                            mahjong.m_weight = (Consts.MahjongWeight) mahjong.weight;
                        }

                        foreach (var mahjong in gameData.faceCards)
                        {
                            mahjong.m_weight = (Consts.MahjongWeight) mahjong.weight;
                        }

                        foreach (var gamer in gamerComponent.GetAll())
                        {
                            if (gamer.UserID != gameData.UserID) continue;
//                            if (gamer.UserID == PlayerInfoComponent.Instance.uid)
//                            {
//                                myCard = new List<MahjongInfo>(gameData.handCards);
//                            }
                            gamer.gameData = gameData;
                            GamerUIComponent gamerUi = gamer.GetComponent<GamerUIComponent>();
                            gamerUi.GameStart();

                            gamer.IsBanker = gameData.IsBanker;
                            HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                            if (handCards != null)
                            {
                                handCards.Reset();
                            }
                            else
                            {
                                handCards = gamer.AddComponent<HandCardsComponent, GameObject, int, int>(gamerUi.Panel, gamerUi.Index, gameData.SeatIndex);
                            }

                            handCards.myCard = new List<MahjongInfo>(gameData.handCards);
                        }

                      
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        foreach (var gamer in gamerComponent.GetAll())
                        {
                            HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                            
                            if (PlayerInfoComponent.Instance.uid == gamer.UserID)
                            {
                                handCards.StartDealCardAnim(true);
                            }
                            else
                            {
                                handCards.StartDealCardAnim(false);
                            }
                            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(400);
                        }
                    }

                    //自己的牌翻一下
                    foreach (var gamer in gamerComponent.GetAll())
                    {
                        if (gamer.UserID == PlayerInfoComponent.Instance.uid)
                        {
                            HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                            handCards.FanPai();
                        }
                    }
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(500);
                    DiceBottom.SetActive(false);
                }

                uiRoomComponent.StartGame(message.restCount);
                uiRoomComponent.exitBtn.interactable = true;
                uiRoomComponent.SetRoomType(message.RoomType);

                foreach (var gameData in message.GamerDatas)
                {
                    foreach (var mahjong in gameData.handCards)
                    {
                        mahjong.m_weight = (Consts.MahjongWeight)mahjong.weight;
                    }

                    foreach (var mahjong in gameData.faceCards)
                    {
                        mahjong.m_weight = (Consts.MahjongWeight)mahjong.weight;
                    }

                    foreach (var gamer in gamerComponent.GetAll())
                    {
                        if (gamer.UserID != gameData.UserID) continue;
                      
                        GamerUIComponent gamerUi = gamer.GetComponent<GamerUIComponent>();
                        gamerUi.GameStart();
                        HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                        if (handCards != null)
                        {
                            handCards.Reset();
                        }
                        else
                        {
                            handCards = gamer.AddComponent<HandCardsComponent, GameObject, int, int>(gamerUi.Panel, gamerUi.Index, gameData.SeatIndex);
                        }

                        //设置庄家
                        gamer.IsBanker = gameData.IsBanker;
                        gamerUi.SetZhuang();

                        //当前出牌玩家
                        if (gamer.IsBanker)
                        {
                            gamerComponent.CurrentPlayUid = gamer.UserID;
                        }

                        if (gamer.UserID == gamerComponent.LocalGamer.UserID)
                        {
                            //本地玩家添加手牌
                            uiRoomComponent.SetTreasureTime(gameData.OnlineSeconds);
                            handCards.AddCards(gameData.handCards);
                            handCards.ShowBg();
                        }
                        else
                        {
                            handCards.AddOtherCards(gamer.IsBanker);
                        }

                        handCards.SetFaceCards(gameData.faceCards);
                        foreach (var card in gameData.faceCards)
                        {
                            gamerUi.SetBuHua(card.weight);
                        }

                    }
                }

                //时间倒计时
                foreach (var gamer in gamerComponent.GetAll())
                {
                    if (gamer.IsBanker)
                    {
                        uiRoomComponent.ShowTurn(gamer.UserID);
                    }
                }

                uiRoom.GameObject.SetActive(true);
                uiRoomComponent.ISGaming = true;

           
                uiRoomComponent.tip.SetActive(true);
                uiRoomComponent.tip.GetComponentInChildren<Image>().sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "shangji_tip");
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(3000);
               
                uiRoomComponent?.tip?.SetActive(false);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}