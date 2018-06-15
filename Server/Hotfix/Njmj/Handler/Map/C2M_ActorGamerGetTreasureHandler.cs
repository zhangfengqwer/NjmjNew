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

                //记录数据
	            gamer.EndTime = DateTime.Now;
	            TimeSpan span = gamer.EndTime - gamer.StartTime;
	            int totalSeconds = (int)span.TotalSeconds;
	            await DBCommonUtil.RecordGamerTime(gamer.EndTime, false, gamer.UserID);
	            await DBCommonUtil.RecordGamerInfo(gamer.UserID, totalSeconds);

	            List<GamerInfoDB> gamerInfos = await proxyComponent.QueryJson<GamerInfoDB>($"{{UId:{gamer.UserID}}}");

                if (gamerInfos.Count == 0)
	            {
	                response.Error = ErrorCode.ERR_Common;
	                response.Message = "未到时间";
                    reply(response);
                    return;
	            }
	            GamerInfoDB gamerInfo = gamerInfos[0];
	            gamerInfo.DailyTreasureCount++;
	            TreasureConfig config = configComponent.Get(typeof(TreasureConfig), gamerInfo.DailyTreasureCount) as TreasureConfig;

	            Log.Debug("config:" + JsonHelper.ToJson(config));

	            if (config == null)
	            {
                    response.Error = ErrorCode.ERR_Common;
	                response.Message = "宝箱已领取完";
                }
	            else
	            {
	                if (gamerInfo.DailyOnlineTime >= config.TotalTime)
	                {
	                    await DBCommonUtil.ChangeWealth(gamer.UserID, 1, config.Reward, "游戏内宝箱奖励");
	                    await proxyComponent.Save(gamerInfo);
                        //下一个奖励
	                    TreasureConfig treasureConfig = (TreasureConfig)configComponent.Get(typeof(TreasureConfig), ++gamerInfo.DailyTreasureCount);
                        //宝箱领完了
	                    if (treasureConfig == null)
	                    {
	                        response.RestSeconds = -1;
	                        response.Reward = config.Reward;
                        }
	                    else
	                    {
	                        response.RestSeconds = treasureConfig.TotalTime - gamerInfo.DailyOnlineTime;
	                        response.Reward = config.Reward;
                        }
	                }
	                else
	                {
	                    response.Error = ErrorCode.ERR_Common;
	                    response.Message = "未到时间";
	                }
                }

	            reply(response);

	            gamer.StartTime = DateTime.Now;
	            await DBCommonUtil.RecordGamerTime(gamer.EndTime, false, gamer.UserID);
            }
	        catch (Exception e)
	        {
	            ReplyError(response, e, reply);
            }

	        await Task.CompletedTask;
        }
	}
}