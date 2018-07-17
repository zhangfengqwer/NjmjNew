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
            Log.Info("G2M_FriendRoomInfo");
            try
            {
                //获取房间信息
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                {
                    //获取所有房间接口
                    RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                    foreach (var room in roomComponent.rooms.Values)
                    {
                        FriendComponent friendComponent = room.GetComponent<FriendComponent>();
                        if (friendComponent == null) continue;
                        FriendRoomInfo friendRoomInfo = new FriendRoomInfo();
                        friendRoomInfo.Hua = friendComponent.Multiples;
                        friendRoomInfo.Ju = friendComponent.JuCount;
                        friendRoomInfo.RoomId = friendComponent.FriendRoomId;
                        friendRoomInfo.IsPublic = friendComponent.IsPublic == true ? 1 : 2;
                        //设置头像
                        foreach (var gamer in room.GetAll())
                        {
                            if (gamer == null) continue;
                            PlayerBaseInfo playerBaseInfo = await DBCommonUtil.getPlayerBaseInfo(gamer.UserID);
                            friendRoomInfo.Icons.Add(playerBaseInfo.Icon);
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
