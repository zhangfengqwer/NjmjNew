using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using Hotfix;
using Unity_Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiRealNameComponentSystem : AwakeSystem<UIRealNameComponent>
	{
		public override void Awake(UIRealNameComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIRealNameComponent : Component
	{
        private Text Text_name;
	    private Text Text_idNumber;

        private Button Button_OK;
        private Button Button_close;

        InputField InputField_name;
        InputField InputField_idNumber;

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
            Text_idNumber = rc.Get<GameObject>("Text_idNumber").GetComponent<Text>();

            InputField_name = Text_name.transform.Find("InputField").GetComponent<InputField>();
            InputField_idNumber = Text_idNumber.transform.Find("InputField").GetComponent<InputField>();

            Button_OK.onClick.Add(onClickOK);
            Button_close.onClick.Add(onClickClose);
        }
        
        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRealName);
        }

        public void onClickOK()
        {
            if (InputField_name.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入姓名");
                return;
            }

            if (InputField_idNumber.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入身份证号");
                return;
            }

            if (!VerifyRuleUtil.CheckRealName(InputField_name.text))
            {
                ToastScript.createToast("请输入正确的姓名");
                return;
            }

            if (!VerifyRuleUtil.CheckIDCard(InputField_idNumber.text))
            {
                ToastScript.createToast("请输入正确的身份证号码");
                return;
            }

            RequestRealName();
        }

        private async void RequestRealName()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_RealName g2cRealName = (G2C_RealName)await SessionComponent.Instance.Session.Call(new C2G_RealName { Uid = PlayerInfoComponent.Instance.uid , Name = InputField_name .text, IDNumber = InputField_idNumber.text });
            UINetLoadingComponent.closeNetLoading();

            if (g2cRealName.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cRealName.Message);

                return;
            }

            ToastScript.createToast("认证成功");
            PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName = true;

            GameUtil.changeData(1,3000);

            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIPlayerInfo).GetComponent<UIPlayerInfoComponent>().Update();
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRealName);
        }
    }
}
