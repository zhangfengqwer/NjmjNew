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
	public class UiCommonPanelComponentSystem : AwakeSystem<UICommonPanelComponent>
	{
		public override void Awake(UICommonPanelComponent self)
		{
			self.Awake();
		}
	}
	
	public class UICommonPanelComponent : Component
	{
        public delegate void OnClickOkEvent();
        public OnClickOkEvent onCallBack_ok = null;

        public delegate void OnClickCloseEvent();
        public OnClickCloseEvent onCallBack_close = null;

        private Button Button_OK;
        private Button Button_close;

        private Text Text_title;
        private Text Text_content;

        public void Awake()
		{
        }

        public static UICommonPanelComponent showCommonPanel(string title,string content)
        {
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UICommonPanel);

            if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel) != null)
            {
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel).GetComponent<UICommonPanelComponent>() != null)
                {
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel).GetComponent<UICommonPanelComponent>().initData(title, content);

                    return Game.Scene.GetComponent<UIComponent>().Get(UIType.UICommonPanel).GetComponent<UICommonPanelComponent>();
                }
            }

            return null;
        }

        public void setOnClickOkEvent(OnClickOkEvent onClickOkEvent)
        {
            onCallBack_ok = onClickOkEvent;
        }

        public void setOnClickCloseEvent(OnClickCloseEvent onClickCloseEvent)
        {
            onCallBack_close = onClickCloseEvent;
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
            if (onCallBack_close == null)
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
            }
            else
            {
                onCallBack_close();
            }
        }

        public void onClickOK()
        {
            if (onCallBack_ok == null)
            { 
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
            }
            else
            {
                onCallBack_ok();
            }
        }
    }
}
