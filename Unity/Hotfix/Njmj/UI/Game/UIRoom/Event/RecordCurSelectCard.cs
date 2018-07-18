using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [Event(ETModel.EventIdType.RecordCurSelectCard)]
    public class RecordCurSelectCard : AEvent<GameObject>
    {
        public override void Run(GameObject a)
        {
            try
            {
                UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                GamerComponent gamerComponent = ui.GetComponent<GamerComponent>();
              
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}