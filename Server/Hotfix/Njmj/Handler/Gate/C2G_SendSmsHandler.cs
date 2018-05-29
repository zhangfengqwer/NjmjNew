using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using MongoDB.Bson;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2G_SendSmsHandler : AMRpcHandler<C2G_SendSms, G2C_SendSms>
	{
		protected override async void Run(Session session, C2G_SendSms message, Action<G2C_SendSms> reply)
		{
            G2C_SendSms response = new G2C_SendSms();

            try
            {
                Log.Debug("请求验证码");
                string str = HttpUtil.SendSms("0", message.Phone);

                SortedDictionary<string, string> dic = CommonUtil.XmlToDictionary(str);
                string ResultCode;
                dic.TryGetValue("ResultCode", out ResultCode);

                string ResultMessageDetails;
                dic.TryGetValue("ResultMessageDetails", out ResultMessageDetails);

                //Log.Debug(str);
                //Log.Debug(JsonHelper.ToJson(dic));
                //Log.Debug("ResultCode = " + ResultCode);
                //Log.Debug("ResultMessageDetails = " +ResultMessageDetails);

                if (ResultCode.CompareTo("0") == 0)
                {
                    //response.Message = ResultMessageDetails;

                    response.Message = "您申请验证码数量已达上限";
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