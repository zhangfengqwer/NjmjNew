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
    }
}
