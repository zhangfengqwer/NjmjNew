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

        public static void CheckIsExistTheUser(long uid)
        {
            User[] users = Game.Scene.GetComponent<UserComponent>().GetAll();
            foreach (var _user in users)
            {
                if (_user.UserID == uid)
                {
                    try
                    {
                        // 发送强制离线
                        {
                            Actor_ForceOffline actor_ForceOffline = new Actor_ForceOffline();
                            _user.session.Send(actor_ForceOffline);
                        }

                        _user.session.Dispose();
                        Game.Scene.GetComponent<UserComponent>().Remove(_user.UserID);
                    }
                    catch (Exception ex)
                    {

                    }

                    break;
                }
            }
        }
    }
}