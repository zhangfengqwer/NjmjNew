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
        private GameObject huafei_1;
        private GameObject huafei_5;
        private GameObject huafei_10;
        private GameObject huafei_20;

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

            huafei_1 = rc.Get<GameObject>("huafei_1");
            huafei_5 = rc.Get<GameObject>("huafei_5");
            huafei_10 = rc.Get<GameObject>("huafei_10");
            huafei_20 = rc.Get<GameObject>("huafei_20");

            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            huafei_1.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_huafei1);
            huafei_5.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_huafei5);
            huafei_10.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_huafei10);
            huafei_20.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_huafei20);

            Button_close.onClick.Add(onClick_close);
        }

        public void onClick_close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseHuaFei);
        }

        public void onClick_huafei1()
        {
            RequestUseHuaFei(1);
        }

        public void onClick_huafei5()
        {
            RequestUseHuaFei(5);
        }

        public void onClick_huafei10()
        {
            RequestUseHuaFei(10);
        }

        public void onClick_huafei20()
        {
            RequestUseHuaFei(20);
        }
        
        private async void RequestUseHuaFeiState()
        {
            G2C_UseHuaFeiState g2cUseHuaFeiState = (G2C_UseHuaFeiState)await SessionWrapComponent.Instance.Session.Call(new C2G_UseHuaFeiState { Uid = PlayerInfoComponent.Instance.uid });

            int HuaFei_1_RestCount = g2cUseHuaFeiState.HuaFei_1_RestCount;
            int HuaFei_5_RestCount = g2cUseHuaFeiState.HuaFei_5_RestCount;
            int HuaFei_10_RestCount = g2cUseHuaFeiState.HuaFei_10_RestCount;
            int HuaFei_20_RestCount = g2cUseHuaFeiState.HuaFei_20_RestCount;

            Log.Debug("话费1次数：" + HuaFei_1_RestCount.ToString());
            Log.Debug("话费5次数：" + HuaFei_5_RestCount.ToString());
            Log.Debug("话费10次数：" + HuaFei_10_RestCount.ToString());
            Log.Debug("话费20次数：" + HuaFei_20_RestCount.ToString());
        }

        private async void RequestUseHuaFei(int huafei)
        {
            G2C_UseHuaFei g2cUseHuaFei = (G2C_UseHuaFei)await SessionWrapComponent.Instance.Session.Call(new C2G_UseHuaFei { Uid = PlayerInfoComponent.Instance.uid, HuaFei = huafei });

            if (g2cUseHuaFei.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseHuaFei.Message);

                return;
            }

            ToastScript.createToast(huafei.ToString() + "元话费兑换成功");
        }
    }
}
