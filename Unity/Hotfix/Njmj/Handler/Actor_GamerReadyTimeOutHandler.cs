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
                GameUtil.Back2Main();
                
                {
                    UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("通知", message.Message);
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
