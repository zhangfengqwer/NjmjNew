using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public class GameHelp
    {
        /// <summary>
        /// 收服务费
        /// </summary>
        /// <param name="room"></param>
        public static void CostServiceCharge(Room room)
        {
            GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
            long cost = controllerComponent.RoomConfig.ServiceCharge;
            
            foreach (var gamer in room.GetAll())
            {
                ChangeGamerGold(room, gamer, (int) - cost, controllerComponent.RoomConfig.Name + "报名费");
            }
        }

        /// <summary>
        /// 游戏内改变金币
        /// </summary>
        /// <param name="room"></param>
        /// <param name="gamer"></param>
        /// <param name="amount"></param>
        /// <param name="msg"></param>
        public static async void ChangeGamerGold(Room room, Gamer gamer, int amount,string msg = "游戏内改变金币")
        {
            await DBCommonUtil.ChangeWealth(gamer.UserID, 1, amount, msg, room);

            room.Broadcast(new Actor_GamerChangeGold()
            {
                Uid = gamer.UserID,
                GoldAmount = amount
            });
        }

        public static void RoomDispose(Room room)
        {
            RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
            roomComponent.RemoveRoom(room);
            room?.Dispose();
        }
    }
}