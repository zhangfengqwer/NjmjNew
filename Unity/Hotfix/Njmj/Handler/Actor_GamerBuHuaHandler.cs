using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerBuHuaHandler : AMHandler<Actor_GamerBuHua>
    {
        protected override async void Run(Session session, Actor_GamerBuHua message)
        {
            try
            {
                Log.Info($"收到补花:{message.weight}");
                MahjongInfo mahjongInfo = new MahjongInfo() { weight = (byte)message.weight, m_weight = (Consts.MahjongWeight)message.weight };
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    handCardsComponent.BuHua(mahjongInfo,true);
                }
                else
                {
                    handCardsComponent.BuHua(mahjongInfo, false);
                }

                //剩下的牌
                uiRoomComponent.SetRestCount();

                SoundComponent.Instance.PlayClip("effect_nv1_buhua");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
