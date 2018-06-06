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
                string reason = controllerComponent.RoomConfig.Name + "报名费";
                await DBCommonUtil.ChangeWealth(gamer.UserID,1, -(int)cost, reason);
            }

//            room.Broadcast(new Actor_GamerChangeGold())
        }
    }
}