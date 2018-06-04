using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiDailyComponentSystem : AwakeSystem<UIDailyComponent>
	{
		public override void Awake(UIDailyComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIDailyComponent : Component
	{
        private GameObject Image_bg;
        private GameObject Item1;
        private GameObject Item2;
        private GameObject Item3;
        private GameObject Item4;
        private GameObject Item5;

        private Button Button_close;

        public void Awake()
		{
            ToastScript.clear();
            initData();

            RequestDailySignState();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Image_bg = rc.Get<GameObject>("Image_bg");
            Item1 = rc.Get<GameObject>("Item1");
            Item2 = rc.Get<GameObject>("Item2");
            Item3 = rc.Get<GameObject>("Item3");
            Item4 = rc.Get<GameObject>("Item4");
            Item5 = rc.Get<GameObject>("Item5");
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Item1.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_item1);
            Item2.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_item2);
            Item3.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_item3);
            Item4.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_item4);
            Item5.transform.Find("Button").GetComponent<Button>().onClick.Add(onClick_item5);
            Button_close.onClick.Add(onClick_close);
        }

        public void onClick_close()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIDaily);
        }

        public void onClick_item1()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIDaily);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);

            if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop) != null)
            {
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>() != null)
                {
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().ShowVipTab();
                }
            }
        }

        public void onClick_item2()
        {
            RequestDailySign();
        }

        public void onClick_item3()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIDaily);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
        }

        public void onClick_item4()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIDaily);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseHuaFei);
        }

        public void onClick_item5()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIDaily);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
        }

        private async void RequestDailySignState()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_DailySignState g2cDailySignState = (G2C_DailySignState)await SessionWrapComponent.Instance.Session.Call(new C2G_DailySignState { Uid = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

            bool TodayIsSign = g2cDailySignState.TodayIsSign;
            if (TodayIsSign)
            {
                Button btn = Item2.transform.Find("Button").GetComponent<Button>();
                btn.interactable = false;
                btn.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_daily", "DayDo_tomorrow");

                string TomorrowReward = g2cDailySignState.TomorrowReward;
                int prop_id = CommonUtil.splitStr_Start(TomorrowReward, ':');
                int prop_num = CommonUtil.splitStr_End(TomorrowReward, ':');
                Item2.transform.Find("Text").GetComponent<Text>().text = ("奖励 <color=#FF8604FF>" + prop_num + "</color>金币");
            }
            else
            {
                string TodayReward = g2cDailySignState.TodayReward;
                int prop_id = CommonUtil.splitStr_Start(TodayReward, ':');
                int prop_num = CommonUtil.splitStr_End(TodayReward, ':');
                Item2.transform.Find("Text").GetComponent<Text>().text = ("奖励 <color=#FF8604FF>" + prop_num + "</color>金币");
            }
        }

        private async void RequestDailySign()
        {

            G2C_DailySign g2cDailySign = (G2C_DailySign)await SessionWrapComponent.Instance.Session.Call(new C2G_DailySign { Uid = PlayerInfoComponent.Instance.uid });

            if (g2cDailySign.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cDailySign.Message);

                return;
            }

            string reward = g2cDailySign.Reward;
            GameUtil.changeDataWithStr(reward);
            ToastScript.createToast("领取成功");

            PlayerInfoComponent.Instance.GetPlayerInfo().IsSign = true;

            {
                Button btn = Item2.transform.Find("Button").GetComponent<Button>();
                btn.interactable = false;
                btn.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_daily", "DayDo_tomorrow");

                string TomorrowReward = g2cDailySign.TomorrowReward;
                int prop_id = CommonUtil.splitStr_Start(TomorrowReward, ':');
                int prop_num = CommonUtil.splitStr_End(TomorrowReward, ':');
                Item2.transform.Find("Text").GetComponent<Text>().text = ("奖励 <color=#FF8604FF>" + prop_num + "</color>金币");
            }
        }
    }
}
