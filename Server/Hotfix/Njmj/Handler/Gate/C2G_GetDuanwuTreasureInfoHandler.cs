using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetDuanwuTreasureInfoHandler : AMRpcHandler<C2G_GetDuanwuTreasureInfo, G2C_GetDuanwuTreasureInfo>
    {
        protected override async void Run(Session session, C2G_GetDuanwuTreasureInfo message, Action<G2C_GetDuanwuTreasureInfo> reply)
        {
            G2C_GetDuanwuTreasureInfo response = new G2C_GetDuanwuTreasureInfo();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<DuanwuTreasureLogInfo> treasures = new List<DuanwuTreasureLogInfo>();
                List<DuanwuTreasureInfo> infos = await proxyComponent.QueryJson<DuanwuTreasureInfo>($"{{UId:{message.UId}}}");
                for(int i = 0;i< infos.Count; ++i)
                {
                    DuanwuTreasureLogInfo info = new DuanwuTreasureLogInfo();
                    info.buyCount = infos[i].BuyCount;
                    info.TreasureId = infos[i].TreasureId;
                    treasures.Add(info);
                }
                response.Treasures = treasures;
                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
