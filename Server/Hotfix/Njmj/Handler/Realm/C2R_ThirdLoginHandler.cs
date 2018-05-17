using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_ThirdLoginHandler : AMRpcHandler<C2R_ThirdLogin, R2C_ThirdLogin>
	{
	    protected override async void Run(Session session, C2R_ThirdLogin message, Action<R2C_ThirdLogin> reply)
	    {
	        Log.Info(JsonHelper.ToJson(message));
            R2C_ThirdLogin response = new R2C_ThirdLogin();
	        try
	        {
	            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
	            List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{Third_Id:'{message.Third_Id}'}}");
                
                // 用户已存在，走登录流程
                if (accountInfos.Count > 0)
                {
                    AccountInfo accountInfo = accountInfos[0];
                    
                    // 随机分配一个Gate
                    StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                    IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
                    Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

                    // 向gate请求一个key,客户端可以拿着这个key连接gate
                    G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { UserId = accountInfo.Id });

                    string outerAddress = config.GetComponent<OuterConfig>().IPEndPoint2.ToString();

                    response.Address = outerAddress;
                    response.Key = g2RGetLoginKey.Key;
                    reply(response);
                }
                // 用户不存在，走注册流程
                else
                {
                    AccountInfo accountInfo = ComponentFactory.CreateWithId<AccountInfo>(UidUtil.createUID());
                    accountInfo.Third_Id = message.Third_Id;
                    accountInfo.MachineId = message.MachineId;
                    accountInfo.ChannelName = message.MachineId;
                    accountInfo.ClientVersion = message.ClientVersion;

                    await proxyComponent.Save(accountInfo);

                    // 随机分配一个Gate
                    StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                    IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
                    Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

                    // 向gate请求一个key,客户端可以拿着这个key连接gate
                    G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { UserId = accountInfo.Id });

                    string outerAddress = config.GetComponent<OuterConfig>().IPEndPoint2.ToString();

                    response.Address = outerAddress;
                    response.Key = g2RGetLoginKey.Key;
                    reply(response);
                }
	        }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
	        }
	    }
	}
}