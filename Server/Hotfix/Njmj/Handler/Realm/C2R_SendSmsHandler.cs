using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using MongoDB.Bson;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_SendSmsHandler : AMRpcHandler<C2R_SendSms, R2C_SendSms>
	{
		protected override async void Run(Session session, C2R_SendSms message, Action<R2C_SendSms> reply)
		{
            R2C_SendSms response = new R2C_SendSms();

            try
            {
                Log.Debug("请求验证码");
                string str = HttpUtil.SendSms("1", message.Phone);
                Log.Debug(str);
                if (!CommonUtil.checkSmsCode(str))
                {
                    response.Message = CommonUtil.getResultMessageDetails(str);
                    response.Error = ErrorCode.ERR_PhoneCodeError;
                    reply(response);
                    return;
                }

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
	}
}