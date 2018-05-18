
using System;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UpdatePlayerInfoHandler : AMRpcHandler<C2G_UpdatePlayerInfo, G2C_UpdatePlayerInfo>
    {
        protected override async void Run(Session session, C2G_UpdatePlayerInfo message, Action<G2C_UpdatePlayerInfo> reply)
        {
            G2C_UpdatePlayerInfo response = new G2C_UpdatePlayerInfo();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                PlayerBaseInfo playerInfo = await proxyComponent.Query<PlayerBaseInfo>(message.Uid);
                if (playerInfo == null)
                {
                    Log.Error($"{message.Uid}的玩家基本信息不存在:");
                    response.Message = "玩家的基本信息不存在";
                    response.playerInfo = null;
                    reply(response);
                }
                else
                {
                    playerInfo.Id = message.Uid;
                    playerInfo.Name = message.playerInfo.Name;
                    playerInfo.GoldNum = message.playerInfo.GoldNum;
                    playerInfo.WingNum = message.playerInfo.WingNum;
                    playerInfo.Icon = message.playerInfo.Icon;
                    await proxyComponent.Save(playerInfo);
                }
                response.playerInfo = message.playerInfo;
                reply(response);
                session.Send(new Actor_UpDateData { playerInfo = response.playerInfo });
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
