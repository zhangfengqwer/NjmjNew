using System;
using System.Collections.Generic;
using System.Diagnostics;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_RegisterHandler : AMRpcHandler<C2R_Register, R2C_Register>
	{
	    protected override async void Run(Session session, C2R_Register message, Action<R2C_Register> reply)
	    {
	        Log.Info(JsonHelper.ToJson(message));
            R2C_Register response = new R2C_Register();
	        try
	        {
	            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
	            List<AccountInfo> accountInfos = await proxyComponent.QueryJson<AccountInfo>($"{{Account:'{message.Account}'}}");
	            if (accountInfos.Count > 0)
	            {
                    response.Error = ErrorCode.AccountExist;
	                response.Message = "用户名存在";
	                reply(response);
                    return;
	            }
	            AccountInfo accountInfo = ComponentFactory.CreateWithId<AccountInfo>(IdGenerater.GenerateId());
	            accountInfo.Account = message.Account;
	            accountInfo.Password = message.Password;
	            await proxyComponent.Save(accountInfo);

	            Stopwatch sw = new Stopwatch();
	            sw.Start();
	            List<AccountInfo> infos = await proxyComponent.QueryJsonCurrentDay<AccountInfo>();
	            sw.Stop();
	            Log.Info($"查询时间:{sw.ElapsedMilliseconds}");
	            Log.Info($"当天的注册有:{infos.Count}");

	            reply(response);
	        }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
	        }
	    }
	}
}