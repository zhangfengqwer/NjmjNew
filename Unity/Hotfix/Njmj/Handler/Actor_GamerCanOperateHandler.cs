using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerCanOperateHandler : AMHandler<Actor_GamerCanOperation>
    {
        protected override async void Run(Session session, Actor_GamerCanOperation message)
        {
            try
            {
                Log.Info($"收到有人碰刚胡");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();

                if (message.Uid == gamerComponent.LocalGamer.UserID)
                {
                    uiRoomComponent.ShowOperation(message.OperationType);
                }

                //显示黄色bg
//                uiRoomComponent.ShowTurn(message.Uid);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
