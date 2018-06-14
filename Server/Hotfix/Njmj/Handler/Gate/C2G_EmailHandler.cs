using ETModel;
using MongoDB.Driver;
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
                DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
                FilterDefinition<ComponentWithId> filterDefinition = new JsonFilterDefinition<ComponentWithId>($"{{UId:{message.Uid}}}");
                List<ComponentWithId> components = await dbComponent.GetCollection(typeof(EmailInfo).Name).FindAsync(filterDefinition).Result.ToListAsync();

             
                List <EmailInfo> listEmail = new List<EmailInfo>();

                // 未读
                for (int i = components.Count - 1; i>= 0; i--)
                {
                    if (((EmailInfo)components[i]).State == 0)
                    {
                        listEmail.Add((EmailInfo)components[i]);

                        if (listEmail.Count >= 50)
                        {
                            break;
                        }
                    }
                }

                // 已读
                if (listEmail.Count < 50)
                {
                    for (int i = components.Count - 1; i >= 0; i--)
                    {
                        if (((EmailInfo)components[i]).State == 1)
                        {
                            listEmail.Add((EmailInfo)components[i]);

                            if (listEmail.Count >= 50)
                            { 
                                break;
                            }
                        }
                    }
                }

                List<Email> emailList = new List<Email>();
                for (int i = 0; i < listEmail.Count; ++i)
                {
                    EmailInfo info = listEmail[i];
                    Email email = new Email();
                    email.EmailTitle = info.EmailTitle;
                    email.Content = info.Content;
                    email.State = info.State;
                    email.RewardItem = info.RewardItem;
                    email.Date = info.CreateTime;
                    email.EId = info.EmailId;
                    emailList.Add(email);
                }
                response.EmailInfoList = emailList;
                reply(response);
            }
            catch(Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
