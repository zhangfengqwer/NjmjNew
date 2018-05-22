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

        private Button Button_close;

        public void Awake()
		{
            ToastScript.clear();

            initData();

            RequestUseHuaFeiState();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            huafei_5 = rc.Get<GameObject>("huafei_5");

            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();
            
            huafei_5.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_huafei5);

            Button_close.onClick.Add(onClick_close);
        }

        public void onClick_close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseHuaFei);
        }
        
        public void onClick_huafei5()
        {
            if (string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone))
            {
                ToastScript.createToast("请先绑定手机号");
                return;
            }

            RequestUseHuaFei(5);
        }
        
        private async void RequestUseHuaFeiState()
        {
            G2C_UseHuaFeiState g2cUseHuaFeiState = (G2C_UseHuaFeiState)await SessionWrapComponent.Instance.Session.Call(new C2G_UseHuaFeiState { Uid = PlayerInfoComponent.Instance.uid});
            
            int HuaFei_5_RestCount = g2cUseHuaFeiState.HuaFei_5_RestCount;
            
            Log.Debug("话费5次数：" + HuaFei_5_RestCount.ToString());
        }

        private async void RequestUseHuaFei(int huafei)
        {
            G2C_UseHuaFei g2cUseHuaFei = (G2C_UseHuaFei)await SessionWrapComponent.Instance.Session.Call(new C2G_UseHuaFei { Uid = PlayerInfoComponent.Instance.uid, HuaFei = huafei, Phone = PlayerInfoComponent.Instance.GetPlayerInfo().Phone });

            if (g2cUseHuaFei.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseHuaFei.Message);

                return;
            }

            ToastScript.createToast(huafei.ToString() + "元话费兑换成功");
        }
    }
}
