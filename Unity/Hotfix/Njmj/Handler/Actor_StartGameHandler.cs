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
    public class Actor_StartGameHandler: AMHandler<Actor_StartGame>
    {
        protected override async void Run(Session session, Actor_StartGame message)
        {
            Log.Debug($"收到开始:{JsonHelper.ToJson(message)}");

            try
            {
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);

                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIReady);
               

                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                uiRoomComponent.StartGame(message.restCount);

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
                        GamerUIComponent gamerUi = gamer.GetComponent<GamerUIComponent>();
                        gamerUi.GameStart();
                        HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                        if (handCards != null)
                        {
//                            handCards.Reset();
                        }
                        else
                        {
                            handCards = gamer.AddComponent<HandCardsComponent, GameObject, int, int>(gamerUi.Panel, gamerUi.Index, gameData.SeatIndex);
                        }

                        //设置庄家
                        gamer.IsBanker = gameData.IsBanker;

                        if (gamer.UserID == gamerComponent.LocalGamer.UserID)
                        {
                            //本地玩家添加手牌
                            handCards.AddCards(gameData.handCards);
                            handCards.ShowBg();
                        }
                        else
                        {
                            handCards.AddOtherCards(gamer.IsBanker);
                        }

                        handCards.SetFaceCards(gameData.faceCards);
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
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}