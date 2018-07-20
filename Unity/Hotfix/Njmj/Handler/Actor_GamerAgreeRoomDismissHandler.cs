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
    public class Actor_GamerAgreeRoomDismissHandler : AMHandler<Actor_GamerAgreeRoomDismiss>
    {
        protected override async void Run(ETModel.Session session, Actor_GamerAgreeRoomDismiss message)
        {
            try
            {
                Log.Info($"收到同意:");
                UI uiRoomDismiss = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoomDismiss);
                if (uiRoomDismiss == null) return;
                UIRoomDismissComponent uiRoomDismissComponent = uiRoomDismiss.GetComponent<UIRoomDismissComponent>();

                uiRoomDismissComponent.SetUserAgree(message.UserId);



            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
