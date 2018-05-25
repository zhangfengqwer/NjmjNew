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
        private Activity info;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIActivityTitle = rc.Get<GameObject>("UIActivityTitle").GetComponent<Button>();
            TitleTxt = rc.Get<GameObject>("TitleTxt").GetComponent<Text>();
            UIActivityTitle.onClick.Add(() =>
            {
                //根据不同的活动ID显示不同的活动面板
            });
        }

        public void SetInfo(Activity info)
        {
            this.info = info;
            TitleTxt.text = info.Title;
        }
    }
}
