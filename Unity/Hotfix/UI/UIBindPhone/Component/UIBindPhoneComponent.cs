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
	public class UiBindPhoneComponentSystem : AwakeSystem<UIBindPhoneComponent>
	{
		public override void Awake(UIBindPhoneComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIBindPhoneComponent : Component
	{
        bool isDispose = false;
        private InputField inputField_Phone;
	    private InputField inputField_YanZhengMa;

        private Button Button_OK;
	    private Button Button_YanZhengMa;
        private Button Button_back;

        private Text text_yanzhengmadaojishi;

        bool isSuccess = false;

        public void Awake()
		{
            ToastScript.clear();

            initData();
        }
        
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            isDispose = true;
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();
            Button_YanZhengMa = rc.Get<GameObject>("Button_YanZhengMa").GetComponent<Button>();
            Button_back = rc.Get<GameObject>("Button_back").GetComponent<Button>();

            inputField_Phone = rc.Get<GameObject>("InputField_Phone").GetComponent<InputField>();
            inputField_YanZhengMa = rc.Get<GameObject>("InputField_YanZhengMa").GetComponent<InputField>();

            text_yanzhengmadaojishi = rc.Get<GameObject>("Text_yanzhengmadaojishi").GetComponent<Text>();

            Button_OK.onClick.Add(onClickBindPhone);
            Button_YanZhengMa.onClick.Add(onClickGetPhoneCode);
            Button_back.onClick.Add(onClickBack);
        }
        
        public void onClickBindPhone()
        {
            if (inputField_Phone.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入手机号");
                return;
            }

            if (inputField_YanZhengMa.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入验证码");
                return;
            }

            if (!VerifyRuleUtil.CheckPhone(inputField_Phone.text))
            {
                ToastScript.createToast("请输入正确的手机号");
                return;
            }

            OnBindPhone(inputField_Phone.text, inputField_YanZhengMa.text);
        }

        public void onClickBack()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIBindPhone);
        }

        public async void onClickGetPhoneCode()
        {
            if (inputField_Phone.text.CompareTo("") == 0)
            {
                ToastScript.createToast("请输入手机号");
                return;
            }

            if (!VerifyRuleUtil.CheckPhone(inputField_Phone.text))
            {
                ToastScript.createToast("请输入正确的手机号");
                return;
            }

            Button_YanZhengMa.transform.localScale = Vector3.zero;
            text_yanzhengmadaojishi.transform.localScale = new Vector3(1,1,1);

            startPhoneCodeTimer();            
            
            try
            {
                UINetLoadingComponent.showNetLoading();
                G2C_SendSms g2cSendSms = (G2C_SendSms)await SessionWrapComponent.Instance.Session.Call(new C2G_SendSms { Uid = PlayerInfoComponent.Instance.uid, Phone = inputField_Phone.text });
                UINetLoadingComponent.closeNetLoading();

                if (g2cSendSms.Error != ErrorCode.ERR_Success)
                {
                    ToastScript.createToast(g2cSendSms.Message);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async void startPhoneCodeTimer()
        {
            int time = 60;
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                --time;

                if (isSuccess)
                {
                    return;
                }

                if(isDispose)
                {
                    return;
                }

                text_yanzhengmadaojishi.text = time.ToString();
            }

            text_yanzhengmadaojishi.transform.localScale = Vector3.zero;
            Button_YanZhengMa.transform.localScale = new Vector3(1,1,1);
        }

        public async void OnBindPhone(string phone ,string code)
		{
			try
			{
                UINetLoadingComponent.showNetLoading();
                G2C_BindPhone g2cBindPhone = (G2C_BindPhone)await SessionWrapComponent.Instance.Session.Call(new C2G_BindPhone { Uid = PlayerInfoComponent.Instance.uid, Phone = phone, Code = code });
                UINetLoadingComponent.closeNetLoading();

                if (g2cBindPhone.Error != ErrorCode.ERR_Success)
                {
                    ToastScript.createToast(g2cBindPhone.Message);
                    return;
                }

                ToastScript.createToast("绑定手机号成功");
                PlayerInfoComponent.Instance.GetPlayerInfo().Phone = phone;
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIPlayerInfo).GetComponent<UIPlayerInfoComponent>().Update();
                isSuccess = true;
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIBindPhone);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
    }
}
