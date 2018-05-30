using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EmailHandler : AMRpcHandler<C2G_Email, G2C_Email>
    {
        protected override async void Run(Session session, C2G_Email message, Action<G2C_Email> reply)
        {
            G2C_Email response = new G2C_Email();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<EmailInfo> emailInfos = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.Uid}}}");
                List<Email> emailList = new List<Email>();
                if(emailInfos.Count > 0)
                {
                    for(int i = 0;i< emailInfos.Count; ++i)
                    {
                        if (emailInfos[i].State == 2)
                            continue;
                        EmailInfo info = emailInfos[i];
                        Email email = new Email();
                        email.EmailTitle = info.EmailTitle;
                        email.Content = info.Content;
                        email.State = info.State;
                        email.RewardItem = info.RewardItem;
                        email.Date = info.Date;
                        email.EId = info.EmailId;
                        emailList.Add(email);
                    }
                    response.EmailInfoList = emailList;
                    reply(response);
                }
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
