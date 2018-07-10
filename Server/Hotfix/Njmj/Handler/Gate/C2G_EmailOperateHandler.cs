using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EmailOperateHandler : AMRpcHandler<C2G_EmailOperate, G2C_EmailOperate>
    {
        protected override async void Run(Session session, C2G_EmailOperate message, Action<G2C_EmailOperate> reply)
        {
            G2C_EmailOperate response = new G2C_EmailOperate();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<EmailInfo> emailInfoList = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.Uid},EmailId:{message.EmailId}}}");
                if (emailInfoList.Count > 0)
                {
                    if (message.state == 1)
                    {
                        //领取奖励
                        PlayerBaseInfo baseInfo = await proxyComponent.Query<PlayerBaseInfo>(message.Uid);
                        PlayerInfo playerInfo = new PlayerInfo();
                        await DBCommonUtil.changeWealthWithStr(message.Uid, emailInfoList[0].RewardItem, "领取邮件奖励");
                        emailInfoList[0].State = 1;
                        await proxyComponent.Save(emailInfoList[0]);
                    }
                    else if (message.state == 2)
                    {
                        //删除邮件
                        emailInfoList[0].State = 2;//1,已领取 2,删除
                        await proxyComponent.Save(emailInfoList[0]);
                    }
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
