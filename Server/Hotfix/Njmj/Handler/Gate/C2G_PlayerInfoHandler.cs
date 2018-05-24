using System;
using System.Collections.Generic;
using System.Text;
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

                response.Message = "Account数据库里不存在该用户";
                response.PlayerInfo = null;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
