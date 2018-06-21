using ETModel;
using System;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerTrusteeshipHandler : AMHandler<Actor_GamerTrusteeship>
    {
        protected override async void Run(Session session, Actor_GamerTrusteeship message)
        {
            try
            {
                Log.Info($"玩家托管:"+message.Uid);

                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);
                if (message.Uid == PlayerInfoComponent.Instance.uid)
                {
                    uiRoomComponent.ShowTrustship();
                }
                else
                {
                    GamerUIComponent gamerUIComponent = gamer.GetComponent<GamerUIComponent>();
                    gamerUIComponent.ShowTrust();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
