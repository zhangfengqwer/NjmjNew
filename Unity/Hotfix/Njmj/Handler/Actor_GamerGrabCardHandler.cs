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
        protected override async void Run(ETModel.Session session, Actor_GamerGrabCard message)
        {
            try
            {
                Log.Info($"收到抓拍");
                MahjongInfo mahjongInfo = new MahjongInfo() { weight = (byte)message.weight, m_weight = (Consts.MahjongWeight)message.weight };
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                if (uiRoom == null) return;
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

                //当前出牌玩家
                gamerComponent.CurrentPlayUid = message.Uid;
                gamerComponent.IsPlayed = false;

                //剩下的牌
                uiRoomComponent.SetRestCount();

                //显示黄色bg
                uiRoomComponent.ShowTurn(message.Uid);
                uiRoomComponent.ClosePropmtBtn();

                SoundsHelp.Instance.playSound_MoPai();

                uiRoomComponent.CurrentMahjong = mahjongInfo;

                Gamer currentGamer = gamerComponent.Get(PlayerInfoComponent.Instance.uid);
                HandCardsComponent currentGamerCard = currentGamer.GetComponent<HandCardsComponent>();
                currentGamerCard.CloseHandCardCanPeng();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
