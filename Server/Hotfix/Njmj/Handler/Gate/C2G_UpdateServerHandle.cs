using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_UpdateServerHandler : AMRpcHandler<C2G_UpdateServer, G2C_UpdateServer>
    {
        protected override async void Run(Session session, C2G_UpdateServer message, Action<G2C_UpdateServer> reply)
        {
            Log.Debug("收到C2G_UpdateServer：" + JsonHelper.ToJson(message));
            G2C_UpdateServer response = new G2C_UpdateServer();
            try
            {
                Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());
                reply(response);
            }
            catch (Exception e)
            {
                response.Error = ErrorCode.ERR_ReloadFail;
                StartConfig myStartConfig = Game.Scene.GetComponent<StartConfigComponent>().StartConfig;
                InnerConfig innerConfig = myStartConfig.GetComponent<InnerConfig>();
                response.Message = $"{innerConfig.IPEndPoint} reload fail, {e}";
                reply(response);
            }


            //            G2C_UpdateServer response = new G2C_UpdateServer();
            //            try
            //            {
            //                StartConfigComponent startConfigComponent = Game.Scene.GetComponent<StartConfigComponent>();
            //                Log.Debug("startConfigComponent：" + JsonHelper.ToJson(startConfigComponent));
            //                NetInnerComponent netInnerComponent = Game.Scene.GetComponent<NetInnerComponent>();
            //                foreach (StartConfig startConfig in startConfigComponent.GetAll())
            //                {
            ////                    if (!message.AppType.Is(startConfig.AppType))
            ////                    {
            ////                        continue;
            ////                    }
            //                    InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
            //                    Session serverSession = netInnerComponent.Get(innerConfig.IPEndPoint);
            //                    Log.Debug("发送M2A_Reload");
            //                    await serverSession.Call(new M2A_Reload());
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                ReplyError(response, e, reply);
            //            }
        }
    }
}
