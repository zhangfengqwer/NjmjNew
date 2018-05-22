using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public static class UserComponentSystem
    {
        public static void BroadCast(this UserComponent self, IActorMessage message)
        {
            User[] users = Game.Scene.GetComponent<UserComponent>().GetAll();
            foreach (var _user in users)
            {
                ActorProxy actorProxy = _user.GetComponent<UnitGateComponent>().GetActorProxy();
                actorProxy.Send(message);
            }
        }
    }
}