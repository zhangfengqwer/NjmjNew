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

        public static void BroadCastToSingle(this UserComponent self, IActorMessage message,long uid)
        {
            User[] users = Game.Scene.GetComponent<UserComponent>().GetAll();
            foreach (var _user in users)
            {
                if (uid == _user.UserID)
                {
                    ActorProxy actorProxy = _user.GetComponent<UnitGateComponent>().GetActorProxy();
                    actorProxy.Send(message);
                }
            }
        }

        public static void ForceOffline(long uid,string reason)
        {
            User[] users = Game.Scene.GetComponent<UserComponent>().GetAll();
            if(uid == -1)
            {
                //全部强制离线
                for(int i = 0;i< users.Length; ++i)
                {
                    try
                    {
                        // 发送强制离线
                        {
                            Actor_ForceOffline actor_ForceOffline = new Actor_ForceOffline();
                            actor_ForceOffline.Reason = reason;
                            users[i].session.Send(actor_ForceOffline);
                        }

                        users[i].session.Dispose();
                        Game.Scene.GetComponent<UserComponent>().Remove(users[i].UserID);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

            }
            else
            {
                //单个强制离线
                foreach (var _user in users)
                {
                    if (_user.UserID == uid)
                    {
                        try
                        {
                            // 发送强制离线
                            {
                                Actor_ForceOffline actor_ForceOffline = new Actor_ForceOffline();
                                actor_ForceOffline.Reason = reason;
                                _user.session.Send(actor_ForceOffline);
                            }

                            _user.session.Dispose();
                            Game.Scene.GetComponent<UserComponent>().Remove(_user.UserID);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }

                        break;
                    }
                }
            }
        }

        public static void EmergencyNotice(long uid, string content)
        {
            User[] users = Game.Scene.GetComponent<UserComponent>().GetAll();
            if (uid == -1)
            {
                // 群发
                for (int i = 0; i < users.Length; ++i)
                {
                    try
                    {
                        Actor_EmergencyNotice actor = new Actor_EmergencyNotice();
                        actor.Content = content;
                        users[i].session.Send(actor);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

            }
            else
            {
                // 单发
                foreach (var _user in users)
                {
                    if (_user.UserID == uid)
                    {
                        try
                        {
                            Actor_EmergencyNotice actor = new Actor_EmergencyNotice();
                            actor.Content = content;
                            _user.session.Send(actor);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }

                        break;
                    }
                }
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
                            actor_ForceOffline.Reason = "您的账号已在别处登录，请重新登录。";
                            _user.session.Send(actor_ForceOffline);
                        }

                        _user.session.Dispose();
                        Game.Scene.GetComponent<UserComponent>().Remove(_user.UserID);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }

                    break;
                }
            }
        }
    }
}