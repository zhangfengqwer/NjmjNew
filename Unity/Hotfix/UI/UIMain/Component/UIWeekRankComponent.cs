using System;
using DG.Tweening;
using ETModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Hotfix;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIWeekRankSystem : StartSystem<UIWeekRankComponent>
    {
        public override void Start(UIWeekRankComponent self)
        {
            self.Start();
        }
    }

    public class UIWeekRankComponent : Component
    {
        private Button Button_close;
        public async void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();
            Button_close.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIWeekRank);
            });
            UIAnimation.ShowLayer(GetParent<UI>().GameObject.transform.GetChild(0).gameObject);
        }
    }
}
