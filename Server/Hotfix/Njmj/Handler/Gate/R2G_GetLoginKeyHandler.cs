﻿using System;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class R2G_GetLoginKeyHandler : AMRpcHandler<R2G_GetLoginKey, G2R_GetLoginKey>
	{
		protected override void Run(Session session, R2G_GetLoginKey message, Action<G2R_GetLoginKey> reply)
		{
			G2R_GetLoginKey response = new G2R_GetLoginKey();
			try
			{
				long key = RandomHelper.RandInt64();
				Game.Scene.GetComponent<NjmjGateSessionKeyComponent>().Add(key, message.UserId);
				response.Key = key;
				reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}