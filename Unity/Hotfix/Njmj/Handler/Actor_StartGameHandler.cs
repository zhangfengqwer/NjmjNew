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
    public class Actor_StartGameHandler : AMHandler<Actor_StartGame>
    {
        protected override async void Run(Session session, Actor_StartGame message)
        {
            try
            {
                foreach (var VARIABLE in message.Mahjongs)
                {
                    
                }

                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                foreach (var gamer in gamerComponent.GetAll())
                {
                    GamerUIComponent gamerUi = gamer.GetComponent<GamerUIComponent>();
                    gamerUi.GameStart();
                    HandCardsComponent handCards = gamer.GetComponent<HandCardsComponent>();
                    if (handCards != null)
                    {
                        handCards.Reset();
                    }
                    else
                    {
                        handCards = gamer.AddComponent<HandCardsComponent, GameObject>(gamerUi.Panel);
                    }

                    if (gamer.UserID == gamerComponent.LocalGamer.UserID)
                    {
                        //本地玩家添加手牌
//                        handCards.AddCards(message.Mahjongs);
                    }


                }

                Log.Info($"收到开始:{JsonHelper.ToJson(message)}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
