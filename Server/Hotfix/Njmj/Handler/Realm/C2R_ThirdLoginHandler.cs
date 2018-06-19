using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ETModel;
using MongoDB.Driver;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_ThirdLoginHandler : AMRpcHandler<C2R_ThirdLogin, R2C_ThirdLogin>
	{
	    protected override async void Run(Session session, C2R_ThirdLogin message, Action<R2C_ThirdLogin> reply)
	    {
	        Log.Debug("收到第三方登录");

            R2C_ThirdLogin response = new R2C_ThirdLogin();
	        try
	        {
	            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
//	            Log.Debug("proxyComponent:" + proxyComponent.dbAddress);

	            List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{Third_Id:'{message.Third_Id}'}}");
                
                // 用户已存在，走登录流程
                if (accountInfos.Count > 0)
                {
                    AccountInfo accountInfo = accountInfos[0];

                    // 黑名单检测
                    if (await DBCommonUtil.CheckIsInBlackList(accountInfo.Id, session))
                    {
                        response.Message = "您的账号存在异常，请联系客服处理。";
                        response.Error = ErrorCode.ERR_PhoneCodeError;
                        reply(response);
                        return;
                    }

                    // 随机分配一个Gate
                    StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                    IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
                    Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

                    // 向gate请求一个key,客户端可以拿着这个key连接gate
                    G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { UserId = accountInfo.Id });

                    string outerAddress = config.GetComponent<OuterConfig>().IPEndPoint2.ToString();

//                    Log.Warning("Gate的ip:" + outerAddress);

                    response.Address = outerAddress;
                    response.Key = g2RGetLoginKey.Key;
                    reply(response);

                    // 登录日志
                    await DBCommonUtil.Log_Login(accountInfo.Id, session);
                }
                // 用户不存在，走注册流程
                else
                {
                    AccountInfo accountInfo = ComponentFactory.CreateWithId<AccountInfo>(UidUtil.createUID());
                    accountInfo.Third_Id = message.Third_Id;
                    accountInfo.MachineId = message.MachineId;
                    accountInfo.ChannelName = message.ChannelName;
                    accountInfo.ClientVersion = message.ClientVersion;

                    await proxyComponent.Save(accountInfo);

                    // 添加用户信息
                    PlayerBaseInfo playerBaseInfo = await DBCommonUtil.addPlayerBaseInfo(accountInfo.Id, "", message.Name, message.Head);

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

                    // 登录日志
                    await DBCommonUtil.Log_Login(accountInfo.Id, session);
                }
	        }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
	        }
	    }
	}
}