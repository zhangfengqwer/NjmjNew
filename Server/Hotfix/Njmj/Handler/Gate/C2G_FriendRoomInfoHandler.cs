using ETModel;
using System;
using System.Collections.Generic;
using System.Net;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_FriendRoomInfoHandler : AMRpcHandler<C2G_FriendRoomInfo, G2C_FriendRoomInfo>
    {
        protected override async void Run(Session session, C2G_FriendRoomInfo message, Action<G2C_FriendRoomInfo> reply)
        {
            G2C_FriendRoomInfo response = new G2C_FriendRoomInfo();
            try
            {
                //获取房间信息
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                User user = session.GetComponent<SessionUserComponent>().User;
                List<PlayerBaseInfo> playerInfoList = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");

                if (playerInfoList.Count > 0)
                {
                    if (!playerInfoList[0].IsGiveFriendKey)
                    {
                        string curTime = CommonUtil.getCurTimeNormalFormat();
                        //每天赠送好友房钥匙
                        await DBCommonUtil.AddFriendKey(message.UId, 3, curTime, "每天赠送3把好友房钥匙");

                        playerInfoList[0].IsGiveFriendKey = true;
                        response.IsGiveFriendKey = true;
                        Log.Debug(response.IsGiveFriendKey + "bool");
                        await proxyComponent.Save(playerInfoList[0]);
                    }
                    else
                    {
                        //今天已经赠送好友房钥匙
                    }
                }

                {
                    //向map服务器发送请求
                    ConfigComponent configCom = Game.Scene.GetComponent<ConfigComponent>();
                    StartConfigComponent _config = Game.Scene.GetComponent<StartConfigComponent>();
                    IPEndPoint mapIPEndPoint = _config.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                    Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapIPEndPoint);

                    M2G_FriendRoomInfo m2GFriendRoomInfo = (M2G_FriendRoomInfo)await mapSession.Call(new G2M_FriendRoomInfo() { });
                    response.Info = m2GFriendRoomInfo.Info;
                }
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
