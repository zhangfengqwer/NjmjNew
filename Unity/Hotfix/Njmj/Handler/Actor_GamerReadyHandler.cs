using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerReadyHandler : AMHandler<Actor_GamerReady>
    {
        protected override async void Run(Session session, Actor_GamerReady message)
        {
            try
            {
                Log.Info($"收到准备:{JsonHelper.ToJson(message)}");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                UIReadyComponent uiReadyComponent = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIReady).GetComponent<UIReadyComponent>();

                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();

                Gamer gamer = gamerComponent.Get(message.Uid);
//                gamer.GetComponent<GamerUIComponent>().SetReady();
                uiReadyComponent.SetReady(message.Uid);

                SoundsHelp.Instance.playSound_ZhunBei();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
