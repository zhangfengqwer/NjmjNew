using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiUseHuaFeiComponentSystem : AwakeSystem<UIUseHuaFeiComponent>
	{
		public override void Awake(UIUseHuaFeiComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIUseHuaFeiComponent : Component
	{
        private GameObject huafei_5;

        private Button Button_cancel;
        private Button Button_OK;

        public void Awake()
		{
            ToastScript.clear();

            initData();

            RequestUseHuaFeiState();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_cancel = rc.Get<GameObject>("Button_cancel").GetComponent<Button>();
            Button_OK = rc.Get<GameObject>("Button_OK").GetComponent<Button>();

            Button_OK.onClick.Add(onClick_huafei5);
            Button_cancel.onClick.Add(onClick_close);
        }

        public void onClick_close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseHuaFei);
        }
        
        public void onClick_huafei5()
        {
            if (string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone))
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseHuaFei);
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIBindPhone);

                return;
            }

            if (PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum < 500)
            {
                ToastScript.createToast("您的话费余额不足");

                return;
            }

            RequestUseHuaFei(5 * 100);
        }
        
        private async void RequestUseHuaFeiState()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_UseHuaFeiState g2cUseHuaFeiState = (G2C_UseHuaFeiState)await SessionWrapComponent.Instance.Session.Call(new C2G_UseHuaFeiState { Uid = PlayerInfoComponent.Instance.uid});
            UINetLoadingComponent.closeNetLoading();

            int HuaFei_5_RestCount = g2cUseHuaFeiState.HuaFei_5_RestCount;
            
            Log.Debug("话费5次数：" + HuaFei_5_RestCount.ToString());
        }

        private async void RequestUseHuaFei(int huafei)
        {
            UINetLoadingComponent.showNetLoading();
            G2C_UseHuaFei g2cUseHuaFei = (G2C_UseHuaFei)await SessionWrapComponent.Instance.Session.Call(new C2G_UseHuaFei { Uid = PlayerInfoComponent.Instance.uid, HuaFei = huafei, Phone = PlayerInfoComponent.Instance.GetPlayerInfo().Phone });
            UINetLoadingComponent.closeNetLoading();

            if (g2cUseHuaFei.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseHuaFei.Message);

                return;
            }
            GameUtil.changeData(3, -5 * 100);
            ToastScript.createToast((huafei / 100.0f).ToString() + "元话费兑换成功");
        }
    }
}
