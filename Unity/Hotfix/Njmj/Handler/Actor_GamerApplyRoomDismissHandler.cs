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
                GameUtil.ShowFriendCommonTip("解散房间");
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIFriendRoomCommonTip).GetComponent<UIFriendRoomCommonTipComponent>().SetSure(() =>
                {
                    ToastScript.createToast("确认");
                });
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIFriendRoomCommonTip).GetComponent<UIFriendRoomCommonTipComponent>().SetCancel(() =>
                {
                    ToastScript.createToast("取消");
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}