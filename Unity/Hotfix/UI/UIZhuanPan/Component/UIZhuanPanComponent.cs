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
        bool isDispose = false;

        private Button Button_ChouJiang;
        private Button Button_close;
        private Button Button_wenhao;

        private GameObject Image_bg;
        private GameObject xingyunzhi;
        private GameObject Item;
        public static UIZhuanPanComponent Instance;

        int ZhuanPanCount = 0;
        int LuckyValue = 0;

        string m_curReward;
        bool m_canClick = true;

        public void Awake()
        {
            Instance = this;
            ToastScript.clear();

            initData();

            RequestGetZhuanPanState();
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

            Button_ChouJiang = rc.Get<GameObject>("Button_ChouJiang").GetComponent<Button>();
            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();
            Button_wenhao = rc.Get<GameObject>("Button_wenhao").GetComponent<Button>();

            Image_bg = rc.Get<GameObject>("Image_bg");
            xingyunzhi = rc.Get<GameObject>("xingyunzhi");
            Item = rc.Get<GameObject>("Item");

            Button_ChouJiang.onClick.Add(onClick_ChouJiang);
            Button_close.onClick.Add(onClickClose);
            Button_wenhao.onClick.Add(showGuiZe);

            Image_bg.transform.Find("Text_tip1/Btn_share").GetComponent<Button>().onClick.Add(onClickShare);

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

            if (OtherData.getIsShiedShare())
            {
                Image_bg.transform.Find("Text_tip1").localScale = Vector3.zero;
            }

            CommonUtil.SetTextFont(Button_close.transform.parent.gameObject);
            UIAnimation.ShowLayer(Button_close.transform.parent.gameObject);
        }

        public void onClickClose()
        {
            if (!m_canClick)
            {
                return;
            }

            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIZhuanPan);
        }

        public void onClickShare()
        {
            if (!m_canClick)
            {
                return;
            }

            PlatformHelper.WXShareFriendsCircle("AndroidCallBack", "OnWxShareFriends", "");
            //PlatformHelper.WXShareFriends("AndroidCallBack", "OnWxShareFriends", OtherData.ShareUrl + "|" + "我是标题" + "|" + "我是内容");
        }

        public void onClick_ChouJiang()
        {
            if (!m_canClick)
            {
                return;
            }

            if (ZhuanPanCount <= 0)
            {
                ToastScript.createToast("您的抽奖次数不足");
                return;
            }

            RequestUseZhuanPan();
        }

        public void showGuiZe()
        {
            string content = "1、每完成一次对局可获得一次转盘机会，每日最高3次\r\n\r\n2、贵族用户每日额外赠送一次机会\r\n\r\n";
            if (!OtherData.getIsShiedShare())
            {
                content += "3、以上转盘次数用完后分享游戏可额外获得一次机会";
            }

            UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("规则", content);
            script.setOnClickOkEvent(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
            });

            script.setOnClickCloseEvent(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
            });

            script.Text_content.alignment = TextAnchor.MiddleLeft;
        }

        public async void RequestShare()
        {
            ToastScript.createToast("分享成功");

            UINetLoadingComponent.showNetLoading();
            G2C_Share g2cShare = (G2C_Share)await SessionComponent.Instance.Session.Call(new C2G_Share { Uid = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

            RequestGetZhuanPanState();
        }

        private async void RequestGetZhuanPanState()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetZhuanPanState g2cGetZhuanPanState = (G2C_GetZhuanPanState)await SessionComponent.Instance.Session.Call(new C2G_GetZhuanPanState { Uid = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

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
            UINetLoadingComponent.showNetLoading();
            G2C_UseZhuanPan g2cUseZhuanPan = (G2C_UseZhuanPan)await SessionComponent.Instance.Session.Call(new C2G_UseZhuanPan { Uid = PlayerInfoComponent.Instance.uid});
            UINetLoadingComponent.closeNetLoading();

            if (g2cUseZhuanPan.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cUseZhuanPan.Message);

                return;
            }

            m_canClick = false;
            m_curReward = g2cUseZhuanPan.reward;
            GameUtil.changeDataWithStr(g2cUseZhuanPan.reward);
            {
                --ZhuanPanCount;
                ++LuckyValue;

                if (ZhuanPanCount <= 0)
                {
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(4, false);
                }
                else
                {

                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(4, true, ZhuanPanCount);
                }

                if (LuckyValue == 99)
                {
                    LuckyValue = 0;
                }

                xingyunzhi.transform.Find("Text_MyLuckyValue").GetComponent<Text>().text = LuckyValue.ToString();
                Button_ChouJiang.transform.Find("Text_restCount").GetComponent<Text>().text = ZhuanPanCount.ToString();
            }

            startXuanZhuan(g2cUseZhuanPan.itemId);
        }

        public async void startXuanZhuan(int itemId)
        {
            isDispose = false;

            for (int m = 1; m <= 5; m++)
            {
                for (int i = 0; i < ZhuanPanConfig.getInstance().getZhuanPanInfoList().Count; i++)
                {
                    if (isDispose)
                    {
                        return;
                    }

                    ZhuanPanInfo zhuanpanInfo = ZhuanPanConfig.getInstance().getZhuanPanInfoList()[i];
                    GameObject item = Item.transform.Find("Item_" + zhuanpanInfo.itemId).gameObject;
                    xuanzhongItem(zhuanpanInfo.itemId);

                    if ((m == 5) && (zhuanpanInfo.itemId == itemId))
                    {
                        //ToastScript.createToast("恭喜您获得" + item.transform.Find("Text_reward").GetComponent<Text>().text);
                        m_canClick = true;
                        ShowRewardUtil.Show(m_curReward);
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
