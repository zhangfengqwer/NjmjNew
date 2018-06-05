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
        public static async void CostServiceCharge(Room room)
        {
            GameControllerComponent controllerComponent = room.GetComponent<GameControllerComponent>();
            long cost = controllerComponent.ServiceCharge;

            foreach (var gamer in room.GetAll())
            {
                await DBCommonUtil.ChangeGold(gamer.UserID, -cost);
            }

//            room.Broadcast(new Actor_GamerChangeGold())
        }
    }
}