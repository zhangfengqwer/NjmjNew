//using ETModel;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//
//namespace ETHotfix
//{
//    [MessageHandler(AppType.Gate)]
//    public class C2G_PressTestHandler : AMRpcHandler<C2G_PressTest, G2C_PressTest>
//    {
//        protected override async void Run(Session session, C2G_PressTest message, Action<G2C_PressTest> reply)
//        {
//            G2C_PressTest response = new G2C_PressTest();
//            Log.Info("压力测试开始");
//            try
//            {
//                DBProxyComponent dBProxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
//                List<AccountInfo> accountInfos = await dBProxyComponent.QueryJsonDB<AccountInfo>("{}");
//                //添加消息转发组件
//                session.AddComponent<MailBoxComponent, string>(ActorType.GateSession);
//
//                foreach (var accountInfo in accountInfos)
//                {
//                    await SensTest(session, accountInfo);
//                }
//                reply(response);
//            }
//            catch (Exception e)
//            {
//                ReplyError(response, e, reply);
//            }
//        }
//
//        private static async Task SensTest(Session session, AccountInfo accountInfo)
//        {
//            Log.Info("session.InstanceId:" + session.InstanceId);
//
//            //创建User对象
//            User user = UserFactory.Create(accountInfo.Id, session);
//            await user.AddComponent<MailBoxComponent>().AddLocation();
//
//            //向map服务器发送请求
//            StartConfigComponent config = Game.Scene.GetComponent<StartConfigComponent>();
//            IPEndPoint mapIPEndPoint = config.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
//            Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapIPEndPoint);
//
//            //玩家进入房间
//            M2G_PlayerEnterRoom m2GPlayerEnterRoom = (M2G_PlayerEnterRoom) await mapSession.Call(new G2M_PlayerEnterRoom()
//            {
//                RoomType = 1,
//                UserId = accountInfo.Id,
//                SessionId = session.InstanceId,
//                PlayerId = user.Id,
//                RoomId = 0
//            });
//
//            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
//            ActorMessageSender actorMessageSender =
//                Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(m2GPlayerEnterRoom.GameId);
//            actorMessageSender.Send(new Actor_GamerReady());
//        }
//    }
//}
