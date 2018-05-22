﻿using System;
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
                Log.Debug("玩家出牌：" + weight + "index:" + index);
                SessionWrapComponent.Instance.Session.Send(new Actor_GamerPlayCard() { weight = weight, index = index });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}