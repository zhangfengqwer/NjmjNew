using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class C2M_ActorGamerGetTreasureHandler : AMActorRpcHandler<Gamer, C2M_ActorGamerGetTreasure,M2C_ActorGamerGetTreasure>
	{
	    protected override async Task Run(Gamer gamer, C2M_ActorGamerGetTreasure message, Action<M2C_ActorGamerGetTreasure> reply)
	    {
	        M2C_ActorGamerGetTreasure response = new M2C_ActorGamerGetTreasure();
	        try
	        {
	            DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
	            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();

	            List<GamerInfoDB> gamerInfos = await proxyComponent.QueryJsonCurrentDayByUid<GamerInfoDB>(gamer.UserID, DateTime.Now);

	            if (gamerInfos.Count == 0)
	            {
	                response.Error = ErrorCode.ERR_Common;
	                response.Message = "未到时间";
                    reply(response);
                    return;
	            }
	            GamerInfoDB gamerInfo = gamerInfos[0];
	            gamerInfo.DailyTreasureCount++;
	            TreasureConfig config = (TreasureConfig) configComponent.Get(typeof(TreasureConfig), gamerInfo.DailyTreasureCount);
	            if (gamerInfo.DailyOnlineTime >= config.TotalTime)
	            {
	                await DBCommonUtil.ChangeWealth(gamer.UserID, 1, config.Reward);
	                await proxyComponent.Save(gamerInfo);
	                TreasureConfig treasureConfig = (TreasureConfig)configComponent.Get(typeof(TreasureConfig), ++gamerInfo.DailyTreasureCount);
	                response.RestSeconds = treasureConfig.TotalTime - gamerInfo.DailyOnlineTime;
	            }
	            else
	            {
	                response.Error = ErrorCode.ERR_Common;
	                response.Message = "未到时间";
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