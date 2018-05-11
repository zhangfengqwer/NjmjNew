using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EmailHandler : AMRpcHandler<C2G_Eamil, G2C_Eamil>
    {
        protected override async void Run(Session session, C2G_Eamil message, Action<G2C_Eamil> reply)
        {
            G2C_Eamil response = new G2C_Eamil();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                List<EmailInfo> emailInfos = await proxyComponent.QueryJson<EmailInfo>($"{{UId:{message.Uid}}}");
                Email email = new Email();
                List<Email> emailList = new List<Email>();
                if(emailInfos.Count > 0)
                {
                    Log.Debug("有邮件");
                    for(int i = 0;i< emailInfos.Count; ++i)
                    {
                        EmailInfo info = emailInfos[i];
                        email.EmailTitle = info.EmailTitle;
                        email.Content = info.Content;
                        email.IsRead = info.IsRead;
                        email.RewardItem = info.RewardItem;
                        email.Date = info.Date;
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
