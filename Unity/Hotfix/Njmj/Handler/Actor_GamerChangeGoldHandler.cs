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
    public class Actor_GamerChangeGoldHandler : AMHandler<Actor_GamerChangeGold>
    {
        protected override async void Run(Session session, Actor_GamerChangeGold message)
        {
            try
            {
                Log.Info($"收到改变金币:"+message.Uid+ "GoldAmount," + message.GoldAmount);
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                if (uiRoom == null)
                {
                    Log.Warning("uiroom  null");
                    return;
                }

                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
                handCardsComponent.ChangeGold(message.GoldAmount);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
