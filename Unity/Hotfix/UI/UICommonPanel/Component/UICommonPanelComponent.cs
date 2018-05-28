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
	public class UiCommonPanelComponentSystem : AwakeSystem<UIUseLaBaComponent>
	{
		public override void Awake(UIUseLaBaComponent self)
		{
			self.Awake();
		}
	}
	
	public class UICommonPanelComponent : Component
	{
        private Button Button_OK;
        private Button Button_close;

        private Text Text_title;
        private Text Text_content;

        public void Awake()
		{
        }

        public static void showCommonPanel(string title,string content)
        {
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UICommonPanel);

            // 刷新主界面
            if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel) != null)
            {
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel).GetComponent<UICommonPanelComponent>() != null)
                {
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel).GetComponent<UICommonPanelComponent>().initData(title, content);
                }
            }
        }

        public void initData(string title, string content)
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Text_title = rc.Get<GameObject>("Text_title").GetComponent<Text>();
            Text_content = rc.Get<GameObject>("Text_content").GetComponent<Text>();

            Button_OK.onClick.Add(onClickOK);
            Button_close.onClick.Add(onClickClose);

            Text_title.text = title;
            Text_content.text = content;
        }

        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
        }

        public void onClickOK()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
        }
    }
}
