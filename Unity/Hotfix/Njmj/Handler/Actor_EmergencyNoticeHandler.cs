using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_EmergencyNoticeHandler : AMHandler<Actor_EmergencyNotice>
    {
        protected override async void Run(Session session, Actor_EmergencyNotice message)
        {
            try
            {
                // HeartBeat.getInstance().stopHeartBeat();

                {
                    UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("通知", message.Content);
                    script.setOnClickOkEvent(() =>
                    {
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                    });

                    script.setOnClickCloseEvent(() =>
                    {
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
