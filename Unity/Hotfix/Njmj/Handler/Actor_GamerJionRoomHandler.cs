using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerJionRoomHandler : AMHandler<Actor_GamerJionRoom>
    {
        protected override async void Run(Session session, Actor_GamerJionRoom message)
        {
            try
            {
                Log.Debug("有人加入:");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                UI uiReady = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIReady);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent roomComponent = uiRoom.GetComponent<UIRoomComponent>();

                UIReadyComponent uiReadyComponent = uiReady.GetComponent<UIReadyComponent>();

                Gamer gamer = GamerFactory.Create(message.Gamer.UserID, message.Gamer.IsReady,message.Gamer.playerInfo);

                GamerUIComponent gamerUiComponent = gamer.GetComponent<GamerUIComponent>();

                //排序
                int index = message.Gamer.SeatIndex - roomComponent.localGamer.SeatIndex;
                if (index < 0) index += 4;

                //设置准备
                if (uiReadyComponent != null)
                {
//                    await gamerUiComponent.GetPlayerInfo();
                    if (gamer?.PlayerInfo != null)
                    {
                        gamerUiComponent?.SetHeadPanel(uiReadyComponent.HeadPanel[index]);
                        uiReady?.GetComponent<UIReadyComponent>()?.SetPanel(gamer, index);
                        //根据座位的indax添加玩家
                        roomComponent?.AddGamer(gamer, index);
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
