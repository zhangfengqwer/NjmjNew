using ETModel;
using System;
using System.Collections.Generic;

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
                List<PlayerBaseInfo> playerInfoList = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.UId}}}");

                if (playerInfoList.Count > 0)
                {
                    if (!playerInfoList[0].IsGiveFriendKey)
                    {
                        //每天赠送好友房钥匙
                        List<UserBag> bagInfoList = await proxyComponent.QueryJson<UserBag>($"{{UId:{message.UId},BagId:{112}}}");
                        if(bagInfoList.Count >= 0)
                        {
                            bagInfoList[0].Count += 3;
                            await proxyComponent.Save(bagInfoList[0]);
                        }
                        else
                        {
                            UserBag info = ComponentFactory.CreateWithId<UserBag>(IdGenerater.GenerateId());
                            info.BagId = 112;
                            info.Count = 3;
                            await proxyComponent.Save(info);
                        }

                        playerInfoList[0].IsGiveFriendKey = true;
                        await proxyComponent.Save(playerInfoList[0]);
                    }
                    else
                    {
                        //今天已经赠送好友房钥匙
                    }
                }

                {
                    //获取所有房间接口

                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
