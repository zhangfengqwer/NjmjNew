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
    public class Actor_GamerHuPaiHandler : AMHandler<Actor_GamerHuPai>
    {
        protected override async void Run(Session session, Actor_GamerHuPai message)
        {
            try
            {
                Log.Info($"收到胡:" + JsonHelper.ToJson(message));
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);

                UIGameResultComponent gameResultComponent =
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.UIGameResult).GetComponent<UIGameResultComponent>();

                gameResultComponent.setData(message, gamerComponent,100);
                uiRoomComponent.ISGaming = false;

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    SoundsHelp.Instance.playSound_Win();
                }
                else
                {
                    SoundsHelp.Instance.playSound_Fail();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
