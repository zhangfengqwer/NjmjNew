using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;
using MongoDB.Bson;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_LoginHandler : AMRpcHandler<C2R_Login, R2C_Login>
	{
		protected override async void Run(Session session, C2R_Login message, Action<R2C_Login> reply)
		{
			R2C_Login response = new R2C_Login();
		    Log.Info($"登录:{JsonHelper.ToJson(message)}");
			try
			{
			    DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
			    List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{Account:'{message.Account}',Password:'{message.Password}'}}");
			    if (accountInfos.Count == 0)
			    {
			        response.Message = "账号或密码错误";
			        response.Error = ErrorCode.ERR_AccountOrPasswordError;
			        reply(response);
			        return;
			    }
                
                AccountInfo accountInfo = accountInfos[0];

			    // 随机分配一个Gate
                StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
//				Log.Debug($"gate address: {MongoHelper.ToJson(config)}");
				IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
				Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

				// 向gate请求一个key,客户端可以拿着这个key连接gate
				G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() {UserId = accountInfo.Id});

				string outerAddress = config.GetComponent<OuterConfig>().IPEndPoint2.ToString();

				response.Address = outerAddress;
				response.Key = g2RGetLoginKey.Key;
                reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}