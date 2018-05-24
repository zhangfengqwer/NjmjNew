using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2G_BindPhoneHandler : AMRpcHandler<C2G_BindPhone, G2C_BindPhone>
	{
        protected override async void Run(Session session, C2G_BindPhone message, Action<G2C_BindPhone> reply)
        {
            Log.Info(JsonHelper.ToJson(message));
            G2C_BindPhone response = new G2C_BindPhone();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{_id:{message.Uid}}}");
                
                if (!string.IsNullOrEmpty(accountInfos[0].Phone))
                {
                    response.Message = "您已绑定手机号，请勿重复绑定";
                    response.Error = ErrorCode.ERR_PhoneCodeError;
                    reply(response);
                    return;
                }

                // 校验验证码
                {
                    string uid = message.Uid.ToString();
                    uid = uid.Substring(1);
                    string str = HttpUtil.CheckSms("0", message.Phone, message.Code);
                    if (!CommonUtil.checkSmsCode(str))
                    {
                        response.Message = "验证码错误";
                        response.Error = ErrorCode.ERR_PhoneCodeError;
                        reply(response);
                        return;
                    }
                }

                accountInfos[0].Phone = message.Phone;
                await proxyComponent.Save(accountInfos[0]);
                reply(response);
            }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
	        }
	    }
	}
}