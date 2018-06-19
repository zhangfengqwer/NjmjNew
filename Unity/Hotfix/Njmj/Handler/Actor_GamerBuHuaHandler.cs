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
                GamerUIComponent gamerUiComponent = gamer.GetComponent<GamerUIComponent>();
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    handCardsComponent.BuHua(mahjongInfo,true);
                    SoundsHelp.Instance.PlayBuHua(PlayerInfoComponent.Instance.GetPlayerInfo().PlayerSound);
                }
                else
                {
                    handCardsComponent.BuHua(mahjongInfo, false);
                    SoundsHelp.Instance.PlayBuHua(gamer.PlayerInfo.PlayerSound);
                }

                //补花显示
                gamerUiComponent.SetBuHua(message.weight);

                //剩下的牌
                uiRoomComponent.SetRestCount();

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
