using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_PhoneLoginHandler : AMRpcHandler<C2R_PhoneLogin, R2C_PhoneLogin>
	{
	    protected override async void Run(Session session, C2R_PhoneLogin message, Action<R2C_PhoneLogin> reply)
	    {
	        Log.Info(JsonHelper.ToJson(message));
            R2C_PhoneLogin response = new R2C_PhoneLogin();
	        try
	        {
	            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
	            List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{Phone:'{message.Phone}'}}");

                // 用验证码登录
                if (message.Code.CompareTo("") != 0)
                {
                    // 先校验验证码
                    {
                        string str = HttpUtil.CheckSms("0",message.Phone, message.Code);
                        if (!CommonUtil.checkSmsCode(str))
                        {
                            response.Message = "验证码错误";
                            response.Error = ErrorCode.ERR_PhoneCodeError;
                            reply(response);
                            return;
                        }
                    }
                    // 用户已存在，走登录流程
                    if (accountInfos.Count > 0)
                    {
                        AccountInfo accountInfo = accountInfos[0];

                        // 黑名单检测
                        if (await DBCommonUtil.CheckIsInBlackList(accountInfo.Id,session))
                        {
                            response.Message = "您的账号存在异常，请联系客服处理。";
                            response.Error = ErrorCode.ERR_PhoneCodeError;
                            reply(response);
                            return;
                        }

                        // 更新Token
                        accountInfo.Token = CommonUtil.getToken(message.Phone);
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
                        response.Token = accountInfo.Token;
                        reply(response);
                        // 登录日志
                        await DBCommonUtil.Log_Login(accountInfo.Id, session, message.ClientVersion);
                    }
                    // 用户不存在，走注册流程
                    else
                    {
                        AccountInfo accountInfo = ComponentFactory.CreateWithId<AccountInfo>(UidUtil.createUID());
                        accountInfo.Phone = message.Phone;
                        accountInfo.Token = CommonUtil.getToken(message.Phone);
                        accountInfo.MachineId = message.MachineId;
                        accountInfo.ChannelName = message.ChannelName;
                        accountInfo.ClientVersion = message.ClientVersion;

                        await proxyComponent.Save(accountInfo);

                        // 添加用户信息
                        PlayerBaseInfo playerBaseInfo = await DBCommonUtil.addPlayerBaseInfo(accountInfo.Id, accountInfo.Phone,"","");

                        // 随机分配一个Gate
                        StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                        IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
                        Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

                        // 向gate请求一个key,客户端可以拿着这个key连接gate
                        G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { UserId = accountInfo.Id });

                        string outerAddress = config.GetComponent<OuterConfig>().IPEndPoint2.ToString();

                        response.Address = outerAddress;
                        response.Key = g2RGetLoginKey.Key;
                        response.Token = accountInfo.Token;
                        reply(response);
                        // 登录日志
                        await DBCommonUtil.Log_Login(accountInfo.Id, session,message.ClientVersion);
                    }
                }
                // 用Token登录
                else if (message.Token.CompareTo("") != 0)
                {
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

                        if (accountInfo?.Token?.CompareTo(message.Token) == 0)
                        {
                            // 随机分配一个Gate
                            StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
                            IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
                            Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

                            // 向gate请求一个key,客户端可以拿着这个key连接gate
                            G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() { UserId = accountInfo.Id });

                            string outerAddress = config.GetComponent<OuterConfig>().IPEndPoint2.ToString();

                            response.Address = outerAddress;
                            response.Key = g2RGetLoginKey.Key;
                            response.Token = accountInfo.Token;
                            reply(response);

                            // 登录日志
                            await DBCommonUtil.Log_Login(accountInfo.Id, session, message.ClientVersion);
                        }
                        else
                        {
                            response.Message = "Token失效，请重新验证登录";
                            response.Error = ErrorCode.ERR_TokenError;
                            reply(response);
                            return;
                        }
                    }
                    else
                    {
                        response.Message = "用户不存在";
                        response.Error = ErrorCode.ERR_AccountNoExist;
                        reply(response);
                        return;
                    }
                }
                // 传的数据错误
                else
                {
                    response.Message = "请求参数缺失";
                    response.Error = ErrorCode.ERR_ParamError;
                    reply(response);
                    return;
                }
	        }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
	        }
	        session.Dispose();
        }
	}
}