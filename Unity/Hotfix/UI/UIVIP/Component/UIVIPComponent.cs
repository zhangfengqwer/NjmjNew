using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIVIPSystem : AwakeSystem<UIVIPComponent>
    {
        public override void Awake(UIVIPComponent self)
        {
            self.Awake();
        }
    }

    public class UIVIPComponent : Component
    {
        private Button De;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            De = rc.Get<GameObject>("De").GetComponent<Button>();

            De.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIVIP);
            });
        }
    }
}
