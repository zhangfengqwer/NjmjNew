using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiAccountBindComponentSystem : AwakeSystem<UIAccountBindComponent>
	{
		public override void Awake(UIAccountBindComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIAccountBindComponent : Component
	{
        private Button Button_OK;
        private Button Button_close;

        private Text Text_uid;
        

        public void Awake()
		{
        }

        public void initData(string uid)
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Text_uid = rc.Get<GameObject>("Text_uid").GetComponent<Text>();
            Text_uid.text = uid;

            Button_OK.onClick.Add(onClickOK);
            Button_close.onClick.Add(onClickClose);
        }
        
        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIAccountBind);
        }

        public void onClickOK()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIAccountBind);
        }
    }
}
