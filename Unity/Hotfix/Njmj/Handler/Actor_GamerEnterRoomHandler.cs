using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerEnterRoomHandler : AMHandler<Actor_GamerEnterRoom>
    {
        protected override async void Run(Session session, Actor_GamerEnterRoom message)
        {
            try
            {
                Log.Info($"收到actor:{JsonHelper.ToJson(message)}");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent roomComponent = uiRoom.GetComponent<UIRoomComponent>();

                for (int i = 0; i < message.Gamers.Count; i++)
                {
                    GamerInfo gamerInfo = message.Gamers[i];
                    Gamer gamer = GamerFactory.Create(gamerInfo.UserID, gamerInfo.IsReady);

                    if (gamer.UserID == PlayerInfoComponent.Instance.uid)
                    {
                        gamerComponent.LocalGamer = gamer;
                    }
                    //根据座位的indax添加玩家
                    roomComponent.AddGamer(gamer, gamerInfo.SeatIndex);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
