using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerPlayCardHandler : AMHandler<Actor_GamerPlayCard>
    {
        protected override async void Run(Session session, Actor_GamerPlayCard message)
        {
            PlayCard(message);
        }

        public static void PlayCard(Actor_GamerPlayCard message)
        {
            try
            {
                Log.Info($"收到出牌");
                MahjongInfo mahjongInfo = new MahjongInfo() { weight = (byte) message.weight, m_weight = (Consts.MahjongWeight) message.weight };

                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
                HandCardsComponent cardsComponent = gamerComponent.LocalGamer.GetComponent<HandCardsComponent>();

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    handCardsComponent.PlayCard(mahjongInfo, message.index, uiRoomComponent.currentItem);
                    SoundsHelp.Instance.PlayCardSound(PlayerInfoComponent.Instance.GetPlayerInfo().PlayerSound,message.weight);
                }
                else
                {
                    handCardsComponent.PlayOtherCard(mahjongInfo, uiRoomComponent.currentItem);
                    SoundsHelp.Instance.PlayCardSound(gamer.PlayerInfo.PlayerSound, message.weight);
                }

                gamerComponent.LastPlayUid = message.Uid;
                SoundsHelp.Instance.playSound_ChuPai();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
