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
    public class UIJuTypeToggleSystem : AwakeSystem<UIJuTypeToggleComponent>
    {
        public override void Awake(UIJuTypeToggleComponent self)
        {
            self.Awake();
        }
    }

    public class UIJuTypeToggleComponent : Component
    {
        private Toggle JuTypeToggle;
        private GameObject key;
        private Text ValueTxt;
        private Text KeyValueTxt;

        private FriendRoomJuShu info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            JuTypeToggle = rc.Get<GameObject>("JuTypeToggle").GetComponent<Toggle>();
            ValueTxt =  rc.Get<GameObject>("ValueTxt").GetComponent<Text>();
            KeyValueTxt =  rc.Get<GameObject>("KeyValueTxt").GetComponent<Text>();

            JuTypeToggle.onValueChanged.Add((isOn) =>
            {
                if (isOn)
                {
                    //选择类型
                    //Log.Debug("===" + info.m_yaoshi + "===");
                    //Log.Debug("---" + info.m_jushu + "---");
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UICreateFriendRoom).GetComponent<UICreateFriendRoomComponent>().SetCurJu(info.m_jushu);
                }
            });
        }

        public void SetToggleInfo(FriendRoomJuShu info, ToggleGroup toggleGroup,int index)
        {
            this.info = info;
            if (index == 0)
            {
                JuTypeToggle.isOn = true;
            }
            else
            {
                JuTypeToggle.isOn = false;
            }
            JuTypeToggle.group = toggleGroup;
            ValueTxt.text = info.m_jushu + "局/";
            KeyValueTxt.text = info.m_yaoshi.ToString();
        }
    }
}
