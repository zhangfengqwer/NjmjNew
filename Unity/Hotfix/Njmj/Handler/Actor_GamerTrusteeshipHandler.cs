using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerTrusteeshipHandler : AMHandler<Actor_GamerTrusteeship>
    {
        protected override async void Run(Session session, Actor_GamerTrusteeship message)
        {
            try
            {
                Log.Info($"玩家托管:");

                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);

                uiRoomComponent.ShowTrustship();

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
