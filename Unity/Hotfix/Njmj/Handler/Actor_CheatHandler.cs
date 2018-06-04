using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_CheatHandler : AMHandler<Actor_GamerCheat>
    {
        protected override void Run(Session session, Actor_GamerCheat message)
        {
            try
            {
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);

                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();

                Gamer gamer = gamerComponent.Get(message.Uid);

                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
                if (handCardsComponent == null) return;
                handCardsComponent.DeleteAllItem(handCardsComponent.CardBottom);

                foreach (var card in message.handCards)
                {
                    card.m_weight = (Consts.MahjongWeight) card.weight;
                }

                handCardsComponent.AddCards(message.handCards);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
