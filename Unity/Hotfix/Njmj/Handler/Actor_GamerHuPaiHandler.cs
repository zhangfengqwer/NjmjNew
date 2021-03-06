﻿using System;
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
        protected override async void Run(ETModel.Session session, Actor_GamerHuPai message)
        {
            try
            {
                Log.Info($"收到胡:");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                if (uiRoom == null) return;

                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();

                SoundsHelp.Instance.PlayHuSound(gamer.PlayerInfo.PlayerSound);

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    SoundsHelp.Instance.playSound_Win();
                }
                else
                {
                    SoundsHelp.Instance.playSound_Fail();
                }

                if (message.IsZiMo)
                {
                    handCardsComponent.ShowOperateAnimAsync((int) GamerOpearteType.zimo);
                }
                else
                {
                    handCardsComponent.ShowOperateAnimAsync((int)GamerOpearteType.Hu);
                }

                uiRoomComponent.exitBtn.gameObject.SetActive(false);

                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1500);
                uiRoomComponent.exitBtn.gameObject.SetActive(true);
                if (gamerComponent.GetGamerCount() < 4) return;

                UIGameResultComponent gameResultComponent =
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.UIGameResult).GetComponent<UIGameResultComponent>();

                RoomConfig roomConfig;
                if (uiRoomComponent.RoomType == 3)
                {
                    roomConfig = uiRoomComponent.RoomConfig;
                    gameResultComponent.SetFriendRoom();
                    gameResultComponent.startTimer(5);
                }
                else
                {
                    roomConfig = ConfigHelp.Get<RoomConfig>(uiRoomComponent.RoomType);
                    gameResultComponent.startTimer(20);
                }
                gameResultComponent.setData(message, gamerComponent, roomConfig.Multiples);
                UIRoomComponent.ISGaming = false;
                uiRoomComponent.ClosePropmtBtn();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
