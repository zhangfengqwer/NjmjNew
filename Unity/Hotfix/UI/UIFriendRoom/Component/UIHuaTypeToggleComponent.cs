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
    public class UIHuaTypeToggleSystem : AwakeSystem<UIHuaTypeToggleComponent>
    {
        public override void Awake(UIHuaTypeToggleComponent self)
        {
            self.Awake();
        }
    }

    public class UIHuaTypeToggleComponent : Component
    {
        private Toggle HuaTypeToggle;
        private Text ValueTxt;

        private int beilv;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            HuaTypeToggle = rc.Get<GameObject>("HuaTypeToggle").GetComponent<Toggle>();
            ValueTxt =  rc.Get<GameObject>("ValueTxt").GetComponent<Text>();

            HuaTypeToggle.onValueChanged.Add((isOn) =>
            {
                if (isOn)
                {
                    //选择类型
                    //Log.Debug("===" + beilv + "===");
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UICreateFriendRoom).GetComponent<UICreateFriendRoomComponent>().SetCurHua(beilv);
                }
            });
        }

        public void SetToggleInfo(int beilv, ToggleGroup toggleGroup,int index)
        {
            this.beilv = beilv;
            if(index == 0)
            {
                HuaTypeToggle.isOn = true;
            }
            else
            {
                HuaTypeToggle.isOn = false;
            }
            HuaTypeToggle.group = toggleGroup;
            ValueTxt.text = beilv.ToString();
        }
    }
}
