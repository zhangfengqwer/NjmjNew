using ETModel;
using System;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerCancelTrusteeshipHandler : AMHandler<Actor_GamerCancelTrusteeship>
    {
        protected override async void Run(Session session, Actor_GamerCancelTrusteeship message)
        {
            try
            {
                Log.Info($"玩家托管取消");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);
                if (message.Uid == PlayerInfoComponent.Instance.uid)
                {
//                    uiRoomComponent.ShowTrustship();
                }
                else
                {
                    GamerUIComponent gamerUIComponent = gamer.GetComponent<GamerUIComponent>();
                    gamerUIComponent.ShowPlayerIcon();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}