using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerReadyTimeOutHandler : AMHandler<Actor_GamerReadyTimeOut>
    {
        protected override async void Run(ETModel.Session session, Actor_GamerReadyTimeOut message)
        {
            try
            {
                Log.Info($"收到准备超时:{message.Message}");
                CommonUtil.ShowUI(UIType.UIMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRoom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIReady);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChatShow);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChat);

                ToastScript.createToast(message.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
