using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_GetItemHandler : AMRpcHandler<C2G_GetItem, G2C_GetItem>
    {
        protected override async void Run(Session session, C2G_GetItem message, Action<G2C_GetItem> reply)
        {
            G2C_GetItem response = new G2C_GetItem();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                PlayerBaseInfo baseInfo = await proxyComponent.Query<PlayerBaseInfo>(message.UId);
                PlayerInfo playerInfo = new PlayerInfo();

                for (int i = 0;i< message.InfoList.Count; ++i)
                {
                    GetItemInfo getItem = message.InfoList[i];
                    await DBCommonUtil.ChangeWealth(message.UId, getItem.ItemID, getItem.Count, "邮件领取奖励");
                }

                List<EmailInfo> emailInfoList = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.UId},_id:{message.MailId}}}");
                if(emailInfoList.Count > 0)
                {
                    emailInfoList[0].State = 1;
                    await proxyComponent.Save(emailInfoList[0]);
                }
                response.Result = true;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
