using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_PlayerInfoHandler : AMRpcHandler<C2G_PlayerInfo, G2C_PlayerInfo>
    {
        protected override async void Run(Session session, C2G_PlayerInfo message, Action<G2C_PlayerInfo> reply)
        {
            Log.Info(JsonHelper.ToJson(message));
            G2C_PlayerInfo response = new G2C_PlayerInfo();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                PlayerBaseInfo playerInfo = await proxyComponent.Query<PlayerBaseInfo>(message.uid);
                response.PlayerInfo = new PlayerInfo();
                if (playerInfo != null)
                {
                    response.Message = "数据库里已存在玩家的基本信息，返回玩家信息";
                    response.PlayerInfo.Name = playerInfo.Name;
                    response.PlayerInfo.GoldNum = playerInfo.GoldNum;
                    response.PlayerInfo.WingNum = playerInfo.WingNum;
                    response.PlayerInfo.Icon = playerInfo.Icon;
                    reply(response);
                    return;
                }
                AccountInfo accountInfo = await proxyComponent.Query<AccountInfo>(message.uid);
                if (accountInfo == null)
                {
                    Log.Error("Account数据库里不存在该用户");
                    response.Message = "Account数据库里不存在该用户";
                    response.PlayerInfo = null;
                }
                else
                {
                    PlayerBaseInfo playerBaseInfo = ComponentFactory.CreateWithId<PlayerBaseInfo>(IdGenerater.GenerateId());
                    playerBaseInfo.Id = message.uid;
                    playerBaseInfo.Name = "默认";
                    playerBaseInfo.GoldNum = 10;
                    playerBaseInfo.WingNum = 0;
                    playerBaseInfo.Icon = "f_icon1";
                    await proxyComponent.Save(playerBaseInfo);

                    response.PlayerInfo.Name = playerBaseInfo.Name;
                    response.PlayerInfo.GoldNum = playerBaseInfo.GoldNum;
                    response.PlayerInfo.WingNum = playerBaseInfo.WingNum;
                    response.PlayerInfo.Icon = playerBaseInfo.Icon;

                    Log.Info(JsonHelper.ToJson(response));
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
