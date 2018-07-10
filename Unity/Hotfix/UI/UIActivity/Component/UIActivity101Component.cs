using ETModel;
using Hotfix;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIActivity101System : AwakeSystem<UIActivity101Component>
    {
        public override void Awake(UIActivity101Component self)
        {
            self.Awake();
        }
    }

    public class UIActivity101Component : Component
    {
        private Button ChongzhiBtn;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            ChongzhiBtn = rc.Get<GameObject>("ChongzhiBtn").GetComponent<Button>();
            ChongzhiBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIActivity);
            });
        }
    }
}
