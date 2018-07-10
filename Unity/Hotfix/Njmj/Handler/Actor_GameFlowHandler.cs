using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GameFlowHandler : AMHandler<Actor_GameFlow>
    {
        protected override async void Run(ETModel.Session session, Actor_GameFlow message)
        {
            try
            {
                Log.Info($"收到流局");

                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                UIGameResultComponent gameResultComponent =
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.UIGameResult).GetComponent<UIGameResultComponent>();

                gameResultComponent.SetFlowGame(message, gamerComponent);
                uiRoomComponent.ISGaming = false;

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
