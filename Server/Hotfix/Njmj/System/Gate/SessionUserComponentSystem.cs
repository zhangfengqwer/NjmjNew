using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class SessionUserComponentDestroySystem : DestroySystem<SessionUserComponent>
    {
        public override async void Destroy(SessionUserComponent self)
        {
            //释放User对象时将User对象从管理组件中移除
            Game.Scene.GetComponent<UserComponent>()?.Remove(self.User.UserID);

            StartConfigComponent config = Game.Scene.GetComponent<StartConfigComponent>();

            //正在匹配中发送玩家退出匹配请求
            if (self.User.IsMatching)
            {
                //      IPEndPoint matchIPEndPoint = config.MatchConfig.GetComponent<InnerConfig>().IPEndPoint;
                //      Session matchSession = Game.Scene.GetComponent<NetInnerComponent>().Get(matchIPEndPoint);
                //      await matchSession.Call(new G2M_PlayerExitMatch_Req() { UserID = this.User.UserID });
            }

            //正在游戏中发送玩家退出房间请求
            if (self.User.ActorID != 0)
            {
                Log.Info($"session释放,玩家MapId：{self.User.ActorID}");

                ActorMessageSenderComponent actorMessageSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
                ActorMessageSender actorMessageSender = actorMessageSenderComponent.Get(self.User.ActorID);

                actorMessageSender.Send(new Actor_GamerExitRoom()
                {
                    IsFromClient = false,
                });
            }

            //向登录服务器发送玩家下线消息
//            IPEndPoint realmIPEndPoint = config.RealmConfig.GetComponent<InnerConfig>().IPEndPoint;
//            Session realmSession = Game.Scene.GetComponent<NetInnerComponent>().Get(realmIPEndPoint);
//            realmSession.Send(new Actor_ForceOffline());

            self.User.Dispose();
            self.User = null;
        }
    }
}
