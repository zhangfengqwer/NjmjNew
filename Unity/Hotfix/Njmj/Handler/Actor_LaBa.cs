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
    public class Actor_LaBaHandler: AMHandler<Actor_LaBa>
    {
        protected override async void Run(Session session, Actor_LaBa message)
        {
            try
            {
                Log.Debug($"收到喇叭:" + JsonHelper.ToJson(message));
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain) != null)
                {
                    if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>() != null)
                    {
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().addLaBaContent(message.LaBaContent);
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