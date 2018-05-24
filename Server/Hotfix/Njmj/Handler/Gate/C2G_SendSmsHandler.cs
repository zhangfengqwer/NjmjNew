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
                string uid = message.Uid.ToString();
                uid = uid.Substring(1);
                HttpUtil.SendSms("0", message.Phone);
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
	}
}