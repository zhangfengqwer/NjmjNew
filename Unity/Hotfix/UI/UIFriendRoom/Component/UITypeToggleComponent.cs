using ETModel;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITypeToggleSystem : AwakeSystem<UITypeToggleComponent>
    {
        public override void Awake(UITypeToggleComponent self)
        {
            self.Awake();
        }
    }

    public class UITypeToggleComponent : Component
    {
        private Toggle TypeToggle;
        private Text ValueTxt;

        private FriendRoomType info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            TypeToggle = rc.Get<GameObject>("TypeToggle").GetComponent<Toggle>();
            ValueTxt =  rc.Get<GameObject>("ValueTxt").GetComponent<Text>();

            TypeToggle.onValueChanged.Add((isOn) =>
            {
                if (isOn)
                {
                    //选择类型
                    Log.Debug("===" + info.m_content + "===");
                    Log.Debug("---" + info.m_type + "---");
                }
            });
        }

        public void SetToggleInfo(FriendRoomType info, ToggleGroup toggleGroup,int index)
        {
            if(index == 0)
            {
                TypeToggle.isOn = true;
            }
            else
            {
                TypeToggle.isOn = false;
            }
            this.info = info;
            TypeToggle.group = toggleGroup;
            ValueTxt.text = info.m_content;
        }
    }
}
