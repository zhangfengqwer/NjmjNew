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
    public class Actor_ForceOfflineHandler : AMHandler<Actor_ForceOffline>
    {
        protected override async void Run(ETModel.Session session, Actor_ForceOffline message)
        {
            try
            {
                HeartBeat.getInstance().stopHeartBeat();

                {
                    UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("提示", message.Reason);
                    script.setOnClickOkEvent(() =>
                    {
                        //Game.Scene.GetComponent<UIComponent>().RemoveAll();
                        //CommonUtil.ShowUI(UIType.UILogin);

                        Application.Quit();
                    });

                    script.setOnClickCloseEvent(() =>
                    {
                        //Game.Scene.GetComponent<UIComponent>().RemoveAll();
                        //CommonUtil.ShowUI(UIType.UILogin);

                        Application.Quit();
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
