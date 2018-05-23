using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChengjiuItemSystem : AwakeSystem<UIChengjiuItemComponent>
    {
        public override void Awake(UIChengjiuItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIChengjiuItemComponent : Component
    {
        private GameObject ChengjiuItemBtn;
        private TaskInfo info;
        
        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            ChengjiuItemBtn = rc.Get<GameObject>("ChengjiuItemBtn");

            ChengjiuItemBtn.GetComponent<Button>().onClick.Add(() =>
            {
                //显示提示框
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu)
                .GetComponent<UIChengjiuComponent>().SetDetail(info);
            });
        }

        public void SetInfo(TaskInfo info)
        {
            this.info = info;
            string icon = new StringBuilder().Append("chengjiu1_")
                                             .Append(info.Id).ToString();
            ChengjiuItemBtn.GetComponent<Image>().sprite =
                CommonUtil.getSpriteByBundle("uichengjiuicon", icon);
        }
    }
}
