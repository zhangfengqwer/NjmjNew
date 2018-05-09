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

                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(100);
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent roomComponent = uiRoom.GetComponent<UIRoomComponent>();

                Gamer[] gamers = gamerComponent.GetAll();

                for (int i = 0; i < gamers.Length; i++)
                {
                    if (gamers[i] == null)
                        continue;
                    if (gamers[i].UserID != 0)
                    {
                        roomComponent.RemoveGamer(gamers[i].UserID);
                    }
                }

                GamerInfo localGamer = null;
                for (int i = 0; i < message.Gamers.Count; i++)
                {
                    if (message.Gamers[i].UserID == PlayerInfoComponent.Instance.uid)
                    {
                        localGamer = message.Gamers[i];
                        gamerComponent.LocalGamer = GamerFactory.Create(localGamer.UserID, localGamer.IsReady);
                        break;
                    }
                }

                if (localGamer == null)
                {
                    return;
                }

                for (int i = 0; i < message.Gamers.Count; i++)
                {
                    GamerInfo gamerInfo = message.Gamers[i];
                    Gamer gamer;
                    if (gamerInfo.UserID == localGamer.UserID)
                    {
                        gamer = gamerComponent.LocalGamer;
                    }
                    else
                    {
                        gamer = GamerFactory.Create(gamerInfo.UserID, gamerInfo.IsReady);
                    }

                    //排序
                    int index = gamerInfo.SeatIndex - localGamer.SeatIndex;
                    if (index < 0) index += 4;
                    gamerInfo.SeatIndex = index;
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
