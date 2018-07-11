using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_OldUserHandler : AMHandler<Actor_OldUser>
    {
        protected override async void Run(ETModel.Session session, Actor_OldUser message)
        {
            try
            {
                Log.Debug($"我是老用户:" + message.OldAccount);

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIAccountBind);

                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIAccountBind) != null)
                {
                    if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIAccountBind).GetComponent<UIAccountBindComponent>() != null)
                    {
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIAccountBind).GetComponent<UIAccountBindComponent>().initData(message.OldAccount);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}