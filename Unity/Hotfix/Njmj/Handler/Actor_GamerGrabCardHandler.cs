using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerGrabCardHandler : AMHandler<Actor_GamerGrabCard>
    {
        protected override async void Run(Session session, Actor_GamerGrabCard message)
        {
            try
            {
                Log.Info($"收到抓拍:{JsonHelper.ToJson(message)}");
                MahjongInfo mahjongInfo = new MahjongInfo() { weight = (byte)message.weight, m_weight = (Consts.MahjongWeight)message.weight };
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    handCardsComponent.GrabCard(mahjongInfo);
                }
                else
                {
                    handCardsComponent.GrabOtherCard();
                }

                //剩下的牌
                uiRoomComponent.SetRestCount();

                //显示黄色bg
                uiRoomComponent.ShowTurn(message.Uid);
                uiRoomComponent.ClosePropmtBtn();

                SoundsHelp.Instance.playSound_MoPai();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
