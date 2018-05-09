using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate>
	{
		protected override async void Run(Session session, C2G_LoginGate message, Action<G2C_LoginGate> reply)
		{
			G2C_LoginGate response = new G2C_LoginGate();
			try
			{
			    long userId = Game.Scene.GetComponent<NjmjGateSessionKeyComponent>().Get(message.Key);
			    if (userId == 0)
			    {
			        response.Error = ErrorCode.ERR_ConnectGateKeyError;
			        response.Message = "Gate key验证失败!";
			        reply(response);
			        return;
			    }

			    //创建User对象
			    User user = UserFactory.Create(userId, session.Id);
			    await user.AddComponent<ActorComponent>().AddLocation();

			    //添加User对象关联到Session上
			    session.AddComponent<SessionUserComponent>().User = user;
			    //添加消息转发组件
			    await session.AddComponent<ActorComponent, string>(ActorType.GateSession).AddLocation();
                ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                List<ShopInfo> shopInfoList = new List<ShopInfo>();
                for (int i = 1; i< configCom.GetAll(typeof(ShopConfig)).Length + 1; ++i)
                {
                    ShopConfig config = (ShopConfig)configCom.Get(typeof(ShopConfig), i);
                    ShopInfo info = new ShopInfo();
                    info.Id = config.Id;
                    info.Name = config.Name;
                    info.Price = config.Price;
                    info.ShopType = config.shopType;
                    info.Desc = config.Desc;
                    shopInfoList.Add(info);
                }
                response.PlayerId = user.Id;
                response.Uid = userId;
                response.ShopInfoList = shopInfoList;
				reply(response);

				session.Send(new G2C_TestHotfixMessage() { Info = "recv hotfix message success" });
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}

        private void AddShopInfo()
        {
            

        }
	}
}