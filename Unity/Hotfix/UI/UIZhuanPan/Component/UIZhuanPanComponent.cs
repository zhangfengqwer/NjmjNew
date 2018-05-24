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

        private GameObject xingyunzhi;
        private GameObject Item;

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

            xingyunzhi = rc.Get<GameObject>("xingyunzhi");
            Item = rc.Get<GameObject>("Item");

            Button_ChouJiang.onClick.Add(onClick_ChouJiang);
            Button_close.onClick.Add(onClickClose);

            for (int i = 0; i < ZhuanPanConfig.getInstance().getZhuanPanInfoList().Count; i++)
            {
                ZhuanPanInfo zhuanpanInfo = ZhuanPanConfig.getInstance().getZhuanPanInfoList()[i];
                GameObject item = Item.transform.Find("Item_" + zhuanpanInfo.itemId).gameObject;

                if (zhuanpanInfo.prop_id == 1)
                {
                    item.transform.Find("Image_icon").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_zhuanpan", "icon_gold");
                    item.transform.Find("Text_reward").GetComponent<Text>().text = ("金币" + (int)zhuanpanInfo.prop_num);
                }
                else if (zhuanpanInfo.prop_id == 3)
                {
                    item.transform.Find("Image_icon").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_zhuanpan", "icon_huafei");
                    item.transform.Find("Text_reward").GetComponent<Text>().text = ("话费" + zhuanpanInfo.prop_num + "元");
                }
            }
        }
        
        public void onClickClose()
        {
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIZhuanPan);
        }

        public void onClick_ChouJiang()
        {
            //if(ZhuanPanCount <= 0)
            //{
            //    ToastScript.createToast("您的抽奖次数不足");
            //    return;
            //}

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

            xingyunzhi.transform.Find("Text_MyLuckyValue").GetComponent<Text>().text = LuckyValue.ToString();
            Button_ChouJiang.transform.Find("Text_restCount").GetComponent<Text>().text = ZhuanPanCount.ToString();
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
            {
                --ZhuanPanCount;
                ++LuckyValue;

                xingyunzhi.transform.Find("Text_MyLuckyValue").GetComponent<Text>().text = LuckyValue.ToString();
                Button_ChouJiang.transform.Find("Text_restCount").GetComponent<Text>().text = ZhuanPanCount.ToString();
            }

            startXuanZhuan(g2cUseZhuanPan.itemId);
        }

        public async void startXuanZhuan(int itemId)
        {
            for (int m = 1; m <= 5; m++)
            {
                for (int i = 0; i < ZhuanPanConfig.getInstance().getZhuanPanInfoList().Count; i++)
                {
                    ZhuanPanInfo zhuanpanInfo = ZhuanPanConfig.getInstance().getZhuanPanInfoList()[i];
                    GameObject item = Item.transform.Find("Item_" + zhuanpanInfo.itemId).gameObject;
                    xuanzhongItem(zhuanpanInfo.itemId);

                    if ((m == 5) && (zhuanpanInfo.itemId == itemId))
                    {
                        ToastScript.createToast("恭喜您获得" + item.transform.Find("Text_reward").GetComponent<Text>().text);
                        return;
                    }

                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(50);
                }
            }
        }

        void xuanzhongItem(int itemId)
        {
            for (int i = 0; i < ZhuanPanConfig.getInstance().getZhuanPanInfoList().Count; i++)
            {
                ZhuanPanInfo zhuanpanInfo = ZhuanPanConfig.getInstance().getZhuanPanInfoList()[i];
                GameObject item = Item.transform.Find("Item_" + zhuanpanInfo.itemId).gameObject;

                if (zhuanpanInfo.itemId == itemId)
                {
                    item.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_zhuanpan", "item_xuanzhong");
                }
                else
                {
                    item.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_zhuanpan", "item_weixuanzhong");
                }
            }
        }
    }
}
