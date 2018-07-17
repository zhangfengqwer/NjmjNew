using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_CreateFriendRoomHandler : AMRpcHandler<C2G_CreateFriendRoom, G2C_CreateFriendRoom>
	{
		protected override async void Run(Session session, C2G_CreateFriendRoom message, Action<G2C_CreateFriendRoom> reply)
		{
		    G2C_CreateFriendRoom response = new G2C_CreateFriendRoom();
            try
            {
                //向map服务器发送请求
                StartConfigComponent config = Game.Scene.GetComponent<StartConfigComponent>();
                IPEndPoint mapIPEndPoint = config.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapIPEndPoint);

                M2G_CreateFriendRoom m2G_CreateFriendRoom = (M2G_CreateFriendRoom) await mapSession.Call(new G2M_CreateFriendRoom()
                {
                    UserId = message.UserId,
                    FriendRoomInfo = message.FriendRoomInfo
                });
                response.RoomId = m2G_CreateFriendRoom.RoomId;
                reply(response);
            }
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}

		    await Task.CompletedTask;
        }
	}
}