using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerExitRoomHandler : AMHandler<Actor_GamerExitRoom>
    {
        protected override async void Run(Session session, Actor_GamerExitRoom message)
        {
            try
            {
                Log.Info($"收到退出:{JsonHelper.ToJson(message)}");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);

                if (uiRoom == null) return;

                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();

                if (gamerComponent.LocalGamer.UserID == message.Uid)
                {
                    CommonUtil.ShowUI(UIType.UIMain);
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRoom);
                }
                else
                {
                    uiRoomComponent.RemoveGamer(message.Uid);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
