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
	public class UiAccountBindComponentSystem : AwakeSystem<UIUseLaBaComponent>
	{
		public override void Awake(UIUseLaBaComponent self)
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
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Text_uid = rc.Get<GameObject>("Text_uid").GetComponent<Text>();

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

        private async void RequestUseLaBa()
        {
            //G2C_UseLaBa g2cUseLaBa = (G2C_UseLaBa)await SessionWrapComponent.Instance.Session.Call(new C2G_UseLaBa { Uid = PlayerInfoComponent.Instance.uid , Content = InputField_content.text});

            //if (g2cUseLaBa.Error != ErrorCode.ERR_Success)
            //{
            //    ToastScript.createToast(g2cUseLaBa.Message);

            //    return;
            //}

            //ToastScript.createToast("发送成功");

            //GameUtil.changeData(105,-1);

            //Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseLaBa);
        }
    }
}
