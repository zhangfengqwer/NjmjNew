using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public static class RoomSystem
    {
        public static void Broadcast(this Room self, IActorMessage message)
        {
            foreach (Gamer gamer in self.gamers)
            {
                if (gamer == null || gamer.isOffline)
                {
                    continue;
                }
                ActorProxy actorProxy = gamer.GetComponent<UnitGateComponent>().GetActorProxy();
                actorProxy.Send(message);
            }
        }

        public static void BroadGamerEnter(this Room self)
        {
            List<GamerInfo> Gamers = new List<GamerInfo>();
            foreach (var item in self.seats)
            {
                GamerInfo gamerInfo = new GamerInfo();
                gamerInfo.UserID = item.Key;
                gamerInfo.SeatIndex = item.Value;
                Gamer temp = self.Get(item.Key);
                gamerInfo.IsReady = temp.IsReady;
                Gamers.Add(gamerInfo);
            }

            self.Broadcast(new Actor_GamerEnterRoom()
            {
                Gamers = Gamers
            });
        }
    }
}
