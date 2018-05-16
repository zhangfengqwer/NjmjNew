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
            try
            {
                Log.Info($"收到出牌:{JsonHelper.ToJson(message)}");
                MahjongInfo mahjongInfo = new MahjongInfo() { weight = (byte) message.weight, m_weight = (Consts.MahjongWeight) message.weight };

                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();

                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    handCardsComponent.PlayCard(mahjongInfo, message.index);
                }
                else
                {
                    handCardsComponent.PlayOtherCard(mahjongInfo);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
