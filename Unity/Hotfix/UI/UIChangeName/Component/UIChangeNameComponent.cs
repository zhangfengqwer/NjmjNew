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
	public class UiChangeNameComponentSystem : AwakeSystem<UIChangeNameComponent>
	{
		public override void Awake(UIChangeNameComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIChangeNameComponent : Component
	{
        private Text Text_name;

        private Button Button_OK;
        private Button Button_close;

        InputField InputField_name;

        public void Awake()
		{
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Text_name = rc.Get<GameObject>("Text_name").GetComponent<Text>();

            InputField_name = Text_name.transform.Find("InputField").GetComponent<InputField>();

            Button_OK.onClick.Add(onClickOK);
            Button_close.onClick.Add(onClickClose);
        }
        
        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChangeName);
        }

        public void onClickOK()
        {
            if (InputField_name.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入昵称");
                return;
            }

            RequestChangeName();
        }

        private async void RequestChangeName()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_ChangeName g2cChangeName = (G2C_ChangeName)await SessionWrapComponent.Instance.Session.Call(new C2G_ChangeName { Uid = PlayerInfoComponent.Instance.uid , Name = InputField_name .text});
            UINetLoadingComponent.closeNetLoading();

            if (g2cChangeName.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cChangeName.Message);

                return;
            }
            ToastScript.createToast("修改成功");
            //更改内存信息
            PlayerInfoComponent.Instance.GetPlayerInfo().Name = InputField_name.text;
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().refreshUI();
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIPlayerInfo).GetComponent<UIPlayerInfoComponent>().Update();
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChangeName);
        }
    }
}
