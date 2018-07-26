using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_CreateFriendRoomHandler : AMRpcHandler<G2M_CreateFriendRoom, M2G_CreateFriendRoom>
	{
		protected override async void Run(Session session, G2M_CreateFriendRoom message, Action<M2G_CreateFriendRoom> reply)
		{
		    M2G_CreateFriendRoom response = new M2G_CreateFriendRoom();
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                Gamer gamer = null;
                Room room = null;

                foreach (var _room in roomComponent.rooms.Values)
                {
                    room = _room;
                    gamer = room.Get(message.UserId);
                    if (gamer != null)
                    {
                        Log.Info("找到房间:" + _room.Id);
                        break;
                    }
                }

                if (gamer == null)
                {
                    Room friendRoom = RoomFactory.CreateFriendRoom(message);
                    roomComponent.Add(friendRoom);

                    GameControllerComponent gameControllerComponent = friendRoom.GetComponent<GameControllerComponent>();
                    response.RoomId = gameControllerComponent.RoomConfig.FriendRoomId;
                }
                else
                {
                    response.Error = ErrorCode.ERR_Common;
                    response.Message = "正在游戏内,无法创建有游戏";
                }

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