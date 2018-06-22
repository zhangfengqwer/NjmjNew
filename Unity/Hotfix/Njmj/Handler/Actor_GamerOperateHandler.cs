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
    public class Actor_GamerOperateHandler : AMHandler<Actor_GamerOperation>
    {
        protected override async void Run(Session session, Actor_GamerOperation message)
        {
            GamerOperation(message,false);
        }

        public static void GamerOperation(Actor_GamerOperation message, bool isReconnect)
        {
            try
            {
                Log.Info($"收到有人碰杠胡");
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                Gamer gamer = gamerComponent.Get(message.Uid);
                HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
                uiRoomComponent.ClosePropmtBtn();
                uiRoomComponent.ShowTurn(message.Uid);
                MahjongInfo mahjongInfo = new MahjongInfo() { weight = (byte) message.weight, m_weight = (Consts.MahjongWeight) message.weight };


                if (message.OperationType == 0)
                {
                    SoundsHelp.Instance.PlayPeng(PlayerInfoComponent.Instance.GetPlayerInfo().PlayerSound);
                }
                else
                {
                    SoundsHelp.Instance.PlayGang(PlayerInfoComponent.Instance.GetPlayerInfo().PlayerSound);
                }

                if (PlayerInfoComponent.Instance.uid == message.Uid)
                {
                    if (message.OperationType == 0)
                    {
                        gamerComponent.CurrentPlayUid = message.Uid;
                        gamerComponent.IsPlayed = false;
                    }

                    //碰刚
                    if (message.OperationType == 5)
                    {
                        handCardsComponent.SetPengGang(message.OperationType, mahjongInfo);
                    }
                    else
                    {
                        handCardsComponent.SetPeng(message.OperationType, mahjongInfo);
                    }
                }
                else
                {
                    //碰刚
                    if (message.OperationType == 5)
                    {
                        handCardsComponent.SetOtherPengGang(message.OperationType, mahjongInfo);
                    }
                    else
                    {
                        handCardsComponent.SetOtherPeng(message.OperationType, mahjongInfo);
                    }
                }
                //显示碰刚动画
                handCardsComponent.ShowOperateAnimAsync(message.OperationType);


                if (isReconnect)
                {
                    return;
                }
                //碰和碰刚删除出的牌
                if (message.OperationType == 0 || message.OperationType == 1)
                {
                    Gamer currentGamer = gamerComponent.Get(gamerComponent.LastPlayUid);
                    HandCardsComponent currentCards = currentGamer.GetComponent<HandCardsComponent>();

                    GameObject.Destroy(currentCards.currentPlayCardObj);
                    currentCards.cardDisplayObjs.Remove(currentCards.currentPlayCardObj);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}