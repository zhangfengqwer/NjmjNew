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
	public class UiNetErrorComponentSystem : AwakeSystem<UINetErrorComponent>
	{
		public override void Awake(UINetErrorComponent self)
		{
			self.Awake();
		}
	}
	
	public class UINetErrorComponent : Component
	{
        private Button Button_OK;
        private Button Button_close;

        private Text Text_title;
        private Text Text_content;

        public void Awake()
		{
            initData();
        }
        
        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Text_title = rc.Get<GameObject>("Text_title").GetComponent<Text>();
            Text_content = rc.Get<GameObject>("Text_content").GetComponent<Text>();

            Button_OK.onClick.Add(onClickOK);
            Button_close.onClick.Add(onClickClose);
        }

        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().RemoveAll();
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
        }

        public void onClickOK()
        {
            Game.Scene.GetComponent<UIComponent>().RemoveAll();
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
        }
    }
}
