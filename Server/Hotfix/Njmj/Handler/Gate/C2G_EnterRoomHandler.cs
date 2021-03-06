﻿using System;
using System.Collections.Generic;
using System.Net;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_EnterRoomHandler : AMRpcHandler<C2G_EnterRoom, G2C_EnterRoom>
	{
		protected override async void Run(Session session, C2G_EnterRoom message, Action<G2C_EnterRoom> reply)
		{
		    G2C_EnterRoom response = new G2C_EnterRoom();
			try
			{
			    User user = session.GetComponent<SessionUserComponent>().User;

			    if (user == null)
			    {
			        return;
			    }

			    //向map服务器发送请求
			    StartConfigComponent config = Game.Scene.GetComponent<StartConfigComponent>();
			    IPEndPoint mapIPEndPoint = config.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
			    Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapIPEndPoint);

                //玩家进入房间
			    M2G_PlayerEnterRoom m2GPlayerEnterRoom = (M2G_PlayerEnterRoom)await mapSession.Call(new G2M_PlayerEnterRoom()
			    {
                    RoomType = message.RoomType,
			        UserId = user.UserID,
			        SessionId = session.InstanceId,
			        PlayerId = user.Id,
                    RoomId = message.RoomId
			    });

			    Log.Info(JsonHelper.ToJson(m2GPlayerEnterRoom));

                response.Error = m2GPlayerEnterRoom.Error;
                response.Message = m2GPlayerEnterRoom.Message;

                session.GetComponent<SessionUserComponent>().User.ActorID = m2GPlayerEnterRoom.GameId;
                
                reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}