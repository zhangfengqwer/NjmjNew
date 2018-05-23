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
                    response.PlayerInfo.HuaFeiNum = playerInfo.HuaFeiNum;
                    response.PlayerInfo.Icon = playerInfo.Icon;
                    response.PlayerInfo.IsRealName = playerInfo.IsRealName;
                    response.PlayerInfo.Phone = playerInfo.Phone;
                    response.PlayerInfo.PlayerSound = playerInfo.PlayerSound;
                    response.PlayerInfo.RestChangeNameCount = playerInfo.RestChangeNameCount;
                    response.PlayerInfo.VipTime = playerInfo.VipTime;
                    response.PlayerInfo.EmogiTime = playerInfo.EmogiTime;
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
                    playerBaseInfo.Phone = "";
                    playerBaseInfo.IsRealName = false;
                    playerBaseInfo.TotalGameCount = 0;
                    playerBaseInfo.WingNum = 0;
                    playerBaseInfo.VipTime = "2018-05-18 00:00:00";
                    playerBaseInfo.PlayerSound = Common_Random.getRandom(1,4);
                    playerBaseInfo.RestChangeNameCount = 1;
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
