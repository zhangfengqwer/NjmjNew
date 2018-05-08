using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiLoginComponentSystem : AwakeSystem<UILoginComponent>
	{
		public override void Awake(UILoginComponent self)
		{
			self.Awake();
		}
	}
	
	public class UILoginComponent: Component
	{
        private GameObject panel_start;
        private GameObject panel_phoneLogin;

        private InputField inputField_Phone;
	    private InputField inputField_YanZhengMa;

        private Button btn_phone;
        private Button btn_wechat;
        private Button btn_login;
	    private Button btn_yanzhengma;
        private Button btn_backToStart;

        public void Awake()
		{
            ToastScript.clear();

            initData();
        }

	    private async void OnRegister()
	    {
	        SoundComponent soundComponent = ETModel.Game.Scene.GetComponent<SoundComponent>();

	        SessionWrap realmSessionWrap = null;

	        try
	        {
	            if (string.IsNullOrEmpty(inputField_Phone.text))
	            {
                    ToastScript.createToast("请输入手机号");
                    return;
	            }

                if (string.IsNullOrEmpty(inputField_YanZhengMa.text))
                {
                    ToastScript.createToast("请输入验证码");
                    return;
                }

                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
	            Session realmSession = Game.Scene.ModelScene.GetComponent<NetOuterComponent>().Create(connetEndPoint);

	            realmSessionWrap = new SessionWrap(realmSession);
	            R2C_Register r2CRegister = (R2C_Register)await realmSessionWrap.Call(new C2R_Register() { Account = inputField_Phone.text, Password = inputField_YanZhengMa.text });
	            realmSessionWrap.Dispose();

	            if (r2CRegister.Error != ErrorCode.ERR_Success)
	            {
	                Log.Error(r2CRegister.Message);
	            }
	            else
	            {
                    ToastScript.createToast("注册成功");

                    OnLoginPhone();
	            }
	        }
	        catch (Exception e)
	        {
	            realmSessionWrap?.Dispose();
	            Log.Error(e);
	        }
	    }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            panel_start = rc.Get<GameObject>("Start");
            panel_phoneLogin = rc.Get<GameObject>("PhoneLogin");

            btn_phone = rc.Get<GameObject>("Button_phone").GetComponent<Button>();
            btn_wechat = rc.Get<GameObject>("Button_wechat").GetComponent<Button>();
            btn_login = rc.Get<GameObject>("Button_Login").GetComponent<Button>();
            btn_yanzhengma = rc.Get<GameObject>("Button_YanZhengMa").GetComponent<Button>();
            btn_backToStart = rc.Get<GameObject>("Button_back").GetComponent<Button>();

            inputField_Phone = rc.Get<GameObject>("InputField_Phone").GetComponent<InputField>();
            inputField_YanZhengMa = rc.Get<GameObject>("InputField_YanZhengMa").GetComponent<InputField>();

            btn_phone.onClick.Add(onClickOpenPhoneLogin);
            btn_wechat.onClick.Add(onClickWechatLogin);
            btn_login.onClick.Add(OnLoginPhone);
            btn_yanzhengma.onClick.Add(OnRegister);
            btn_backToStart.onClick.Add(onClickBackStart);

            {
                // 隐藏手机登录界面
                panel_phoneLogin.transform.localScale = Vector3.zero;
            }
        }

        public void onClickOpenPhoneLogin()
        {
            panel_phoneLogin.transform.localScale = new Vector3(1, 1, 1);
        }

        public void onClickWechatLogin()
        {
            ToastScript.createToast("暂未开放");
        }

        public void onClickBackStart()
        {
            panel_phoneLogin.transform.localScale = Vector3.zero;
        }

        public async void OnLoginPhone()
		{
			SessionWrap sessionWrap = null;
			try
			{
				IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

				string text = inputField_Phone.GetComponent<InputField>().text;

				Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
				sessionWrap = new SessionWrap(session);
				R2C_Login r2CLogin = (R2C_Login) await sessionWrap.Call(new C2R_Login() { Account = inputField_Phone.text, Password = inputField_YanZhengMa.text });
				sessionWrap.Dispose();

			    if (r2CLogin.Error != ErrorCode.ERR_Success)
			    {
			        Log.Error(r2CLogin.Message);
			        return;
			    }
                connetEndPoint = NetworkHelper.ToIPEndPoint(r2CLogin.Address);
				Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
				Game.Scene.AddComponent<SessionWrapComponent>().Session = new SessionWrap(gateSession);
				ETModel.Game.Scene.AddComponent<SessionComponent>().Session = gateSession;
				G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionWrapComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key});

                ToastScript.createToast("登录成功");

                // 创建Player
                //                Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
                //				PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
                //				playerComponent.MyPlayer = player;

                Game.Scene.GetComponent<PlayerInfoComponent>().uid = g2CLoginGate.Uid;

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain); 
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UILogin);
			}
			catch (Exception e)
			{
				sessionWrap?.Dispose();
				Log.Error(e);
			}
		}
	}
}
