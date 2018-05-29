using ETModel;
using System;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_ChatHandler : AMHandler<Actor_Chat>
    {
        protected override void Run(Session session, Actor_Chat message)
        {
            try
            {
                Log.Info("收到表情：" + JsonHelper.ToJson(message));

                UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChatShow);
                if (message.ChatType == 1)
                {
                    ui.GetComponent<UIChatShowComponent>().ShowExpressAni(message.Value);
                }
                else if(message.ChatType == 2)
                {
                    ui.GetComponent<UIChatShowComponent>().ShowChatContent(message.Value,message.UId);
                    
                }
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChat).GetComponent<UIChatComponent>().CloseOrOpenChatUI(false);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
