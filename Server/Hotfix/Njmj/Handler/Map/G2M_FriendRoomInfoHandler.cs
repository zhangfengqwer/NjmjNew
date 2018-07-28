using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Map)]
    public class G2M_FriendRoomInfoHandler : AMRpcHandler<G2M_FriendRoomInfo, M2G_FriendRoomInfo>
    {
        protected override async void Run(Session session, G2M_FriendRoomInfo message, Action<M2G_FriendRoomInfo> reply)
        {
            M2G_FriendRoomInfo response = new M2G_FriendRoomInfo();
            //Log.Info("G2M_FriendRoomInfo");
            try
            {
                //获取房间信息
                {
                    //获取所有空闲房间接口
                    RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                    foreach (var room in roomComponent.rooms.Values)
                    {
                        if (!room.IsFriendRoom) continue;

                        GameControllerComponent gameControllerComponent = room.GetComponent<GameControllerComponent>();
                        FriendRoomInfo friendRoomInfo = new FriendRoomInfo();
                        friendRoomInfo.Hua = gameControllerComponent.RoomConfig.Multiples;
                        friendRoomInfo.Ju = gameControllerComponent.RoomConfig.JuCount;
                        friendRoomInfo.RoomId = gameControllerComponent.RoomConfig.FriendRoomId;
                        friendRoomInfo.IsPublic = gameControllerComponent.RoomConfig.IsPublic ? 1 : 2;
                        //设置头像
                        foreach (var gamer in room.GetAll())
                        {
                            if (gamer == null) continue;
                            friendRoomInfo.Icons.Add(gamer.playerBaseInfo.Icon);
                        }
                        response.Info.Add(friendRoomInfo);
                    }
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
