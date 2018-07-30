using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_ShowAnimHandler : AMHandler<Actor_ShowAnimType>
    {
        protected override void Run(ETModel.Session session, Actor_ShowAnimType message)
        {
            UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            if (uiRoom == null) return;
            UIRoomComponent roomComponent = uiRoom.GetComponent<UIRoomComponent>();
            Log.Info("显示：" + message.Type);

            try
            {
                switch (message.Type)
                {
                    case 1:
                        ToastScript.createToast("扣除服务费:" + message.Count);
                        break;
                    case 2:
                    case 3:
                        roomComponent.ShowAnim(message.Type);
                        break;
                    case 4:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
