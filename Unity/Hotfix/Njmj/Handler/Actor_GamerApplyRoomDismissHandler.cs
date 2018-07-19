using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Actor_GamerApplyRoomDismissHandler: AMHandler<Actor_GamerApplyRoomDismiss>
    {
        protected override async void Run(ETModel.Session session, Actor_GamerApplyRoomDismiss message)
        {
            try
            {
                Log.Info($"收到申请解散");
                UI ui = GameUtil.CreateUI(UIType.UIRoomDismiss);
                UIRoomDismissComponent uiRoomDismissComponent = ui.GetComponent<UIRoomDismissComponent>();


            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}