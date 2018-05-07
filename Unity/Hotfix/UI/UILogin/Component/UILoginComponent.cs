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
	    private InputField account;
	    private InputField password;

	    private GameObject loginBtn;
	    private GameObject registerBtn;

        public void Awake()
		{
		    ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
		    loginBtn = rc.Get<GameObject>("LoginBtn");
		    registerBtn = rc.Get<GameObject>("RegisterBtn");

		    loginBtn.GetComponent<Button>().onClick.Add(OnLogin);
		    registerBtn.GetComponent<Button>().onClick.Add(OnRegister);

		    this.account = rc.Get<GameObject>("Account").GetComponent<InputField>();
		    this.password = rc.Get<GameObject>("Password").GetComponent<InputField>();
        }

	    private async void OnRegister()
	    {
	        SoundComponent soundComponent = ETModel.Game.Scene.GetComponent<SoundComponent>();

	        soundComponent.PlayClip("card_nan1_1");

	        SessionWrap realmSessionWrap = null;

	        try
	        {
	            if (string.IsNullOrEmpty(account.text) || string.IsNullOrEmpty(password.text))
	            {
	                Log.Error("账号或密码为空");
	                return;
	            }

	            IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);
	            Session realmSession = Game.Scene.ModelScene.GetComponent<NetOuterComponent>().Create(connetEndPoint);

	            realmSessionWrap = new SessionWrap(realmSession);
	            R2C_Register r2CRegister = (R2C_Register)await realmSessionWrap.Call(new C2R_Register() { Account = this.account.text, Password = this.password.text });
	            realmSessionWrap.Dispose();

	            if (r2CRegister.Error != ErrorCode.ERR_Success)
	            {
	                Log.Error(r2CRegister.Message);
	            }
	            else
	            {
	                Log.Info("注册成功");
	            }
	        }
	        catch (Exception e)
	        {
	            realmSessionWrap?.Dispose();
	            Log.Error(e);
	        }
	    }

        public async void OnLogin()
		{
			SessionWrap sessionWrap = null;
			try
			{
				IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

				string text = this.account.GetComponent<InputField>().text;

				Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
				sessionWrap = new SessionWrap(session);
				R2C_Login r2CLogin = (R2C_Login) await sessionWrap.Call(new C2R_Login() { Account = account.text, Password = password.text });
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
				G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionWrapComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

				Log.Info("登陆gate成功!");

				// 创建Player
				Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
				PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
				playerComponent.MyPlayer = player;

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
