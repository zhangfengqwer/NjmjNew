using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ETModel;
using MongoDB.Driver;

namespace ETHotfix
{
	[MessageHandler(AppType.Realm)]
	public class C2R_ChangeAccountHandler : AMRpcHandler<C2R_ChangeAccount, R2C_ChangeAccount>
	{
	    protected override async void Run(Session session, C2R_ChangeAccount message, Action<R2C_ChangeAccount> reply)
	    {
	        Log.Info(JsonHelper.ToJson(message));
            R2C_ChangeAccount response = new R2C_ChangeAccount();
	        try
	        {
                Game.Scene.GetComponent<UserComponent>().Remove(message.Uid);
	            reply(response);
	        }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
	        }
	    }
	}
}