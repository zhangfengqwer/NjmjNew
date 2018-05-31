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
        protected override async void Run(Session session, Actor_OldUser message)
        {
            try
            {
                Log.Debug($"我是老用户");
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIAccountBind);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}