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
//            Log.Info(JsonHelper.ToJson(message));
            G2C_PlayerInfo response = new G2C_PlayerInfo();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<PlayerBaseInfo> playerInfo = await proxyComponent.QueryJson<PlayerBaseInfo>($"{{_id:{message.uid}}}");
                List<OtherData> otherDatas = await proxyComponent.QueryJson<OtherData>($"{{UId:{message.uid}}}");
                response.PlayerInfo = new PlayerInfo();
                if (playerInfo != null)
                {
                    Log.Debug("获取玩家数据" + JsonHelper.ToJson(playerInfo));
                    response.Message = "数据库里已存在玩家的基本信息，返回玩家信息";
                    response.PlayerInfo.Name = playerInfo[0].Name;
                    response.PlayerInfo.GoldNum = playerInfo[0].GoldNum;
                    response.PlayerInfo.WingNum = playerInfo[0].WingNum;
                    response.PlayerInfo.HuaFeiNum = playerInfo[0].HuaFeiNum;
                    response.PlayerInfo.Icon = playerInfo[0].Icon;
                    response.PlayerInfo.IsRealName = playerInfo[0].IsRealName;
                    AccountInfo accountInfo = await DBCommonUtil.getAccountInfo(message.uid);
                    response.PlayerInfo.Phone = accountInfo.Phone;
                    response.PlayerInfo.PlayerSound = playerInfo[0].PlayerSound;
                    response.PlayerInfo.RestChangeNameCount = playerInfo[0].RestChangeNameCount;
                    response.PlayerInfo.VipTime = playerInfo[0].VipTime;
                    response.PlayerInfo.EmogiTime = playerInfo[0].EmogiTime;
                    response.PlayerInfo.MaxHua = playerInfo[0].MaxHua;
                    response.PlayerInfo.TotalGameCount = playerInfo[0].TotalGameCount;
                    response.PlayerInfo.WinGameCount = playerInfo[0].WinGameCount;
                    if(otherDatas.Count > 0)
                    {
                        response.OwnIcon = otherDatas[0].OwnIcon;
                    }

                    // 今天是否签到过
                    {
                        List<DailySign> dailySigns = await proxyComponent.QueryJson<DailySign>($"{{CreateTime:/^{DateTime.Now.GetCurrentDay()}/,Uid:{message.uid}}}");
                        if (dailySigns.Count == 0)
                        {
                            response.PlayerInfo.IsSign = false;
                        }
                        else
                        {
                            response.PlayerInfo.IsSign = true;
                        }
                    }

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
