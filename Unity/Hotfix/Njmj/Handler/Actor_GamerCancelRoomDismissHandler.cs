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
    public class Actor_GamerCancelRoomDismissHandler : AMHandler<Actor_GamerCancelRoomDismiss>
    {
        protected override async void Run(ETModel.Session session, Actor_GamerCancelRoomDismiss message)
        {
            try
            {
                Log.Info($"收到取消:");
                UI uiRoomDismiss = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoomDismiss);
                if (uiRoomDismiss == null) return;

                UIRoomDismissComponent uiRoomDismissComponent = uiRoomDismiss.GetComponent<UIRoomDismissComponent>();

                uiRoomDismissComponent.SetUserCancel(message.UserId);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
