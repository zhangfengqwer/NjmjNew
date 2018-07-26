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
        private Button Button_cancel;
        private Button Button_HuaFei;
        private Button Button_Key;

        int HuaFei_5_RestCount = 0;
        int YuanBao_RestCount = 0;

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
            Button_HuaFei = rc.Get<GameObject>("Button_HuaFei").GetComponent<Button>();
            Button_Key = rc.Get<GameObject>("Button_Key").GetComponent<Button>();

            Button_HuaFei.onClick.Add(onClick_huafei5);
            Button_Key.onClick.Add(onClick_duihuanyuanbao);
            Button_cancel.onClick.Add(onClick_close);

            CommonUtil.SetTextFont(Button_HuaFei.transform.parent.gameObject);
            UIAnimation.ShowLayer(Button_HuaFei.transform.parent.gameObject);
        }

        public void onClick_close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseHuaFei);
        }
        
        public void onClick_huafei5()
        {
            if (PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum < 500)
            {
                ToastScript.createToast("您的话费余额不足");

                return;
            }

            if (string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone))
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseHuaFei);
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIBindPhone);

                return;
            }

            RequestUseHuaFei(5 * 100,1);
        }

        public void onClick_duihuanyuanbao()
        {
            if (PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum < 100)
            {
                ToastScript.createToast("您的话费余额不足");

                return;
            }

            RequestUseHuaFei(1 * 100,2);
        }

        private async void RequestUseHuaFeiState()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_UseHuaFeiState g2cUseHuaFeiState = (G2C_UseHuaFeiState)await SessionComponent.Instance.Session.Call(new C2G_UseHuaFeiState { Uid = PlayerInfoComponent.Instance.uid});
            UINetLoadingComponent.closeNetLoading();

            HuaFei_5_RestCount = g2cUseHuaFeiState.HuaFei_5_Count;
            YuanBao_RestCount = g2cUseHuaFeiState.YuanBao_Count;

            Button_HuaFei.transform.Find("Text").GetComponent<Text>().text = "兑换 " + HuaFei_5_RestCount + "/1";
            Button_Key.transform.Find("Text").GetComponent<Text>().text = "兑换 " + YuanBao_RestCount + "/10";
        }

        private async void RequestUseHuaFei(int huafei,int type)
        {
            UINetLoadingComponent.showNetLoading();
            G2C_UseHuaFei g2cUseHuaFei = (G2C_UseHuaFei)await SessionComponent.Instance.Session.Call(new C2G_UseHuaFei { Uid = PlayerInfoComponent.Instance.uid, HuaFei = huafei, Phone = PlayerInfoComponent.Instance.GetPlayerInfo().Phone,Type = type });
            UINetLoadingComponent.closeNetLoading();

            if (g2cUseHuaFei.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseHuaFei.Message);

                return;
            }

            GameUtil.changeData(3, -huafei);

            // 兑换话费
            if (type == 1)
            {
                ++HuaFei_5_RestCount;
                ToastScript.createToast((huafei / 100.0f).ToString() + "元话费兑换成功");
            }
            // 兑换元宝
            else if (type == 2)
            {
                ++YuanBao_RestCount;
                GameUtil.changeDataWithStr(g2cUseHuaFei.Reward);
                ShowRewardUtil.Show(g2cUseHuaFei.Reward);
            }

            Button_HuaFei.transform.Find("Text").GetComponent<Text>().text = "兑换 " + HuaFei_5_RestCount + "/1";
            Button_Key.transform.Find("Text").GetComponent<Text>().text = "兑换 " + YuanBao_RestCount + "/10";
        }
    }
}
