using ETModel;
using Hotfix;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIActivityItemSystem : AwakeSystem<UIActivityItemComponent>
    {
        public override void Awake(UIActivityItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIActivityItemComponent : Component
    {
        private Button UIActivityTitle;
        private Text TitleTxt;
        private ActivityInfo info;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIActivityTitle = rc.Get<GameObject>("UIActivityTitle").GetComponent<Button>();
            TitleTxt = rc.Get<GameObject>("TitleTxt").GetComponent<Text>();
            UIActivityTitle.onClick.Add(() =>
            {
                //根据不同的活动ID显示不同的活动面板
                string panelName = "UIActivity_" + info.id;
                GameObject obj = CommonUtil.getGameObjByBundle(panelName);
                GameObject activity101 = GameObject.Instantiate(obj);
                Transform parent = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIActivity).GetComponent<UIActivityComponent>().GetActivityParent();
                activity101.transform.SetParent(parent);
                activity101.transform.localScale = Vector3.one;
                activity101.transform.localPosition = Vector3.zero;
                UI ui = ComponentFactory.Create<UI, GameObject>(activity101);
                ui.AddComponent<UIActivity101Component>();
            });
        }

        public void SetInfo(ActivityInfo info)
        {
            this.info = info;
            TitleTxt.text = info.title;
        }
    }
}
