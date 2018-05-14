using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_ForceOfflineHandler : AMHandler<Actor_ForceOffline>
    {
        protected override async void Run(Session session, Actor_ForceOffline message)
        {
            try
            {
                ToastScript.createToast("您的账号已在别处登录，请重新登录");

                Game.Scene.GetComponent<UIComponent>().RemoveAll();
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
