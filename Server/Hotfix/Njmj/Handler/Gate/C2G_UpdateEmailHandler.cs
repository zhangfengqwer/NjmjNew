using System;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UpdateEmailHandler : AMRpcHandler<C2G_UpdateEmail, G2C_UpdateEmail>
    {
        protected override async void Run(Session session, C2G_UpdateEmail message, Action<G2C_UpdateEmail> reply)
        {
            G2C_UpdateEmail response = new G2C_UpdateEmail();
            try
            {
                DBProxyComponent proxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
                EmailInfo emailInfo = await proxyComponent.Query<EmailInfo>(message.EId);
                if (emailInfo == null)
                {
                    Log.Error($"{message.EId}的信箱不存在;");
                    response.Message = "邮箱信息有错";
                    reply(response);
                }
                else
                {
                    emailInfo.Id = message.EId;
                    //emailInfo.State = message.state;
                    await proxyComponent.Save(emailInfo);
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
