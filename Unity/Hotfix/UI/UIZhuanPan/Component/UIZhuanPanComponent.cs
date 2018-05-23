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
	public class UiZhuanPanComponentSystem : AwakeSystem<UIZhuanPanComponent>
	{
		public override void Awake(UIZhuanPanComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIZhuanPanComponent : Component
	{
        private Button Button_ChouJiang;
        private Button Button_close;

        int ZhuanPanCount = 0;
        int LuckyValue = 0;

        public void Awake()
		{
            ToastScript.clear();

            initData();

            RequestGetZhuanPanState();
        }

        public void initData()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_ChouJiang = rc.Get<GameObject>("Button_ChouJiang").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();

            Button_ChouJiang.onClick.Add(onClick_ChouJiang);
            Button_close.onClick.Add(onClickClose);
        }
        
        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIUseLaBa);
        }

        public void onClick_ChouJiang()
        {
            RequestUseZhuanPan();
        }

        private async void RequestGetZhuanPanState()
        {
            G2C_GetZhuanPanState g2cGetZhuanPanState = (G2C_GetZhuanPanState)await SessionWrapComponent.Instance.Session.Call(new C2G_GetZhuanPanState { Uid = PlayerInfoComponent.Instance.uid });

            if (g2cGetZhuanPanState.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cGetZhuanPanState.Message);

                return;
            }

            ZhuanPanCount = g2cGetZhuanPanState.ZhuanPanCount;
            LuckyValue = g2cGetZhuanPanState.LuckyValue;

            Log.Debug("转盘次数=" + ZhuanPanCount.ToString());
            Log.Debug("幸运值=" + LuckyValue.ToString());
        }

        private async void RequestUseZhuanPan()
        {
            G2C_UseZhuanPan g2cUseZhuanPan = (G2C_UseZhuanPan)await SessionWrapComponent.Instance.Session.Call(new C2G_UseZhuanPan { Uid = PlayerInfoComponent.Instance.uid});

            if (g2cUseZhuanPan.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseZhuanPan.Message);

                return;
            }

            GameUtil.changeDataWithStr(g2cUseZhuanPan.reward);
        }
    }
}
