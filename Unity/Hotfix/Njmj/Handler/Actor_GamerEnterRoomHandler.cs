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
        protected override async void Run(ETModel.Session session, Actor_GamerEnterRoom message)
        {
            GamerEnterRoom(message);
        }

        public static async Task GamerEnterRoom(Actor_GamerEnterRoom message)
        {
            try
            {
                //玩家还在结算画面
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIGameResult) != null)
                {
                    return;
                }

                Log.Info($"收到玩家进入");
                //第一次进入创建UIRoom
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom) == null)
                {
                    CommonUtil.ShowUI(UIType.UIRoom);
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIMain);
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICreateFriendRoom);
                }

                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIReady) == null)
                {
                    CommonUtil.ShowUI(UIType.UIReady);
                }
                UINetLoadingComponent.closeNetLoading();
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                UI uiReady = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIReady);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent roomComponent = uiRoom.GetComponent<UIRoomComponent>();
                roomComponent.ContinueGamer();
                roomComponent.enterRoomMsg = message;
                roomComponent.SetRoomType(message.RoomType, message.Multiples);
                Gamer[] gamers = gamerComponent.GetAll();

                //清空座位
                for (int i = 0; i < gamers.Length; i++)
                {
                    if (gamers[i] == null)
                        continue;

                    if (gamers[i].UserID != 0)
                    {
                        Log.Debug("删除gamer");
                        roomComponent.RemoveGamer(gamers[i].UserID);
                    }
                }

                GamerInfo localGamer = null;
                for (int i = 0; i < message.Gamers.Count; i++)
                {
                    if (message.Gamers[i].UserID == PlayerInfoComponent.Instance.uid)
                    {
                        localGamer = message.Gamers[i];
                        gamerComponent.LocalGamer = GamerFactory.Create(localGamer.UserID, localGamer.IsReady,localGamer.playerInfo);
                        break;
                    }
                }

                if (localGamer == null)
                {
                    return;
                }

                roomComponent.localGamer = localGamer;

                //好友房处理
                if (message.RoomType == 3)
                {
                    roomComponent.IsFriendRoom = true;
                    roomComponent.JuCount = message.JuCount;
                    uiReady?.GetComponent<UIReadyComponent>()?.ClosePropt();
                    uiReady?.GetComponent<UIReadyComponent>()?.SetFriendRoom(true);
                    uiReady?.GetComponent<UIReadyComponent>()?.ShowWeChat(message.RoomId.ToString());
                    roomComponent.RoomConfig.Multiples = message.Multiples;
                    roomComponent.SetFriendSetting(message.MasterUserId);
                }
                else
                {
                    uiReady?.GetComponent<UIReadyComponent>()?.SetFriendRoom(false);
                    roomComponent.IsFriendRoom = false;
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
                        gamer = GamerFactory.Create(gamerInfo.UserID, gamerInfo.IsReady, gamerInfo.playerInfo);
                    }

                    UIReadyComponent uiReadyComponent = uiReady.GetComponent<UIReadyComponent>();
                    GamerUIComponent gamerUiComponent = gamer.GetComponent<GamerUIComponent>();

                    //排序
                    int index = gamerInfo.SeatIndex - localGamer.SeatIndex;
                    if (index < 0) index += 4;

                    //设置准备
                    if (uiReadyComponent != null)
                    {
                        if (gamer?.PlayerInfo != null)
                        {
                            gamerUiComponent?.SetHeadPanel(uiReadyComponent.HeadPanel[index],index);
                            gamerUiComponent?.SetFace(roomComponent.FacePanel[index]);
                            uiReady?.GetComponent<UIReadyComponent>()?.SetPanel(gamer, index);
                            //根据座位的indax添加玩家
                            roomComponent?.AddGamer(gamer, index);
                        }
                    }
                }

                SoundsHelp.Instance.playSound_JinRu();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
