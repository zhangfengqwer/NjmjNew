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
                OnClick(info.id);
            });
        }

        public void OnClick(int id)
        {
            //根据不同的活动ID显示不同的活动面板
            string panelName = "UIActivity_" + id;
            if (id == 102 || id == 103)
            {
                ToastScript.createToast("活动暂未开放");
                return;
            }
            GameObject obj = CommonUtil.getGameObjByBundle(panelName);
            GameObject activity = GameObject.Instantiate(obj, Game.Scene.GetComponent<UIComponent>().Get(UIType.UIActivity).GetComponent<UIActivityComponent>().GetActivityParent());
            //activity.transform.GetComponent<RectTransform>().setan
            UI ui = ComponentFactory.Create<UI, GameObject>(activity);
            if (id == 101)
                ui.AddComponent<UIActivity101Component>();
        }

        public void SetInfo(ActivityInfo info)
        {
            this.info = info;
            TitleTxt.text = info.title;
        }
    }
}
