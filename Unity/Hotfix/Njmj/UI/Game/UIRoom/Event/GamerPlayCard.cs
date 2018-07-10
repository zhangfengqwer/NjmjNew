using System;
using ETModel;

namespace ETHotfix
{
    [Event(ETModel.EventIdType.GamerPlayCard)]
    public class GamerPlayCard: AEvent<int, int>
    {
        public override void Run(int weight, int index)
        {
            try
            {
                UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = ui.GetComponent<GamerComponent>();
                if (gamerComponent.CurrentPlayUid == PlayerInfoComponent.Instance.uid)
                {
                    Log.Debug("玩家出牌：" + weight + "index:" + index);
                    if (gamerComponent.IsPlayed)
                    {
//                        ToastScript.createToast("拦截多次出牌");
                        return;
                    }

                    if (weight >= 41)
                    {
                        Log.Warning("不能出花牌");
                        return;
                    }
                    gamerComponent.IsPlayed = true;
                    SessionComponent.Instance.Session.Send(new Actor_GamerPlayCard() { weight = weight, index = index });
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}