using ETModel;
using Hotfix;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIItemSystem: AwakeSystem<UIItemComponent>
    {
        public override void Awake(UIItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIItemComponent : Component
    {
        private Image icon;
        private Text nameTxt;
        private Text priceTxt;
        private Button buyBtn;
        private ReferenceCollector rc;
        private int index;
        private ShopInfo shopInfo;

        public void Awake()
        {
            rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            priceTxt = rc.Get<GameObject>("PriceTxt").GetComponent<Text>();
            buyBtn = rc.Get<GameObject>("BuyBtn").GetComponent<Button>();

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);

            buyBtn.onClick.Add(async () =>
            {
                if(shopInfo.CurrencyType == 2)
                {
                    
                    //用元宝购买
                    long yuan = PlayerInfoComponent.Instance.GetPlayerInfo().WingNum;
                    if (GameUtil.isVIP())
                    {
                        ShowBuy(yuan, shopInfo.VipPrice,"元宝购买");
                    }
                    else
                    {
                        ShowBuy(yuan, shopInfo.Price,"元宝购买");
                    }
                }
                else if(shopInfo.CurrencyType == 1)
                {
                    //用元宝购买
                    long gold = PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum;
                    if (GameUtil.isVIP())
                    {
                        ShowBuy(gold, shopInfo.VipPrice,"金币购买");
                    }
                    else
                    {
                        ShowBuy(gold, shopInfo.Price,"金币购买");
                    }
                }
                else
                {
                    if (!PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName && !OtherData.getIsShiedRealName())
                    {
                        ToastScript.createToast("请先完成实名认证！");
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIShop);
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.UIRealName);
                        return;
                    }

                    {
                        UINetLoadingComponent.showNetLoading();
                        G2C_IsCanRecharge g2cIsCanRecharge = (G2C_IsCanRecharge)await SessionComponent.Instance.Session.Call(new C2G_IsCanRecharge { UId = PlayerInfoComponent.Instance.uid });
                        UINetLoadingComponent.closeNetLoading();

                        if (g2cIsCanRecharge.Error != ErrorCode.ERR_Success)
                        {
                            ToastScript.createToast(g2cIsCanRecharge.Message);

                            return;
                        }
                        else
                        {
                            //ToastScript.createToast("可以充值");
                        }
                    }

                    //接购买SDK
                    //ToastScript.createToast("暂时未开放人民币购买");
                    //可以购买
                    if (!ChannelHelper.IsThirdChannel())
                    {
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().Pay(shopInfo);
                    }
                    else
                    {
                        PlatformHelper.pay(PlatformHelper.GetChannelName(), "AndroidCallBack", "GetPayResult", SetRequest(shopInfo).ToJson());
                    }
                }
            });
        }

        public static JsonData SetRequest(ShopInfo shopInfo)
        {
            JsonData data = new JsonData();
            data["uid"] = PlayerInfoComponent.Instance.uid;
            data["goods_id"] = shopInfo.Id;
            data["goods_num"] = 1;
            data["goods_name"] = shopInfo.Name;
            data["price"] = shopInfo.Price;
//            data["price"] = "0.01";
            return data;
        }

        public void ShowBuy(long own,long price,string content)
        {
            string tip = "";
            bool isCanBuy = false;
            if (own >= price)
            {
                //可以购买
                tip = new StringBuilder().Append("确定花费")
                                                .Append(price)
                                                .Append(content)
                                                .Append(shopInfo.Name)
                                                .Append("吗").ToString();
                isCanBuy = true;
            }
            else
            {
                //元宝不够
                tip = "您还没有足够的元宝，现在去充值吧！";
                isCanBuy = false;
            }
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().BuyTip(shopInfo, tip, isCanBuy);
        }

        public void SetCommonItem(ShopInfo info)
        {
            shopInfo = info;
            nameTxt.text = info.Name;
            icon.sprite = CommonUtil.getSpriteByBundle("Image_Shop", info.Icon);
            if (info.ShopType == (int)ShopType.Wing)
                priceTxt.text = new StringBuilder().Append(info.Price).Append("元").ToString();
            else
                priceTxt.text = info.Price.ToString();
        }

        public void SetWingItem(ShopInfo info)
        {
            SetCommonItem(info);
            int prop_id = CommonUtil.splitStr_Start(info.Items, ':');
            int prop_num = CommonUtil.splitStr_End(info.Items, ':');
        }

        public void SetGoldItem(ShopInfo info)
        {
            SetCommonItem(info);
            Text disCountTxt = rc.Get<GameObject>("DisCountTxt").GetComponent<Text>();
            Text descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
            string[] strArr = info.Desc.Split(';');
            string[] itemsArr = info.Items.Split(';');
            int prop_id = CommonUtil.splitStr_Start(info.Items, ':');
            int prop_num = CommonUtil.splitStr_End(info.Items, ':');
            
            descTxt.text = strArr[0];
            disCountTxt.text = strArr[1];
        }

        Button openBtn;
        Button closeBtn;
        public void SetItem(ShopInfo info,int index,ShopType shopType)
        {
            SetCommonItem(info);
            int prop_id = CommonUtil.splitStr_Start(info.Items, ':');
            int prop_num = CommonUtil.splitStr_End(info.Items, ':');
            this.index = index;
            float height = 0;
            if(openBtn == null && closeBtn == null)
            {
                openBtn = rc.Get<GameObject>("OpenBtn").GetComponent<Button>();
                closeBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();
                switch (shopType)
                {
                    case ShopType.Prop:
                        Text descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
                        Text disCountTxt = rc.Get<GameObject>("DisCountTxt").GetComponent<Text>();
                        if (prop_id == 112 && info.CurrencyType == 1)
                        {
                            this.GetParent<UI>().GameObject.transform.Find("Yuan").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("Image_Shop", "icon_jinbi");
                        }
                        else
                        {
                            this.GetParent<UI>().GameObject.transform.Find("Yuan").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("Image_Shop", "icon_yb");
                        }
                        string[] strArr = info.Desc.Split(';');
                        descTxt.text = strArr[0];
                        disCountTxt.text = strArr[1];
                        height = 150;
                        break;
                    case ShopType.Vip:
                        nameTxt.text = info.Desc;
                        height = 200;
                        break;
                }
                openBtn.onClick.Add(() =>
                {
                    openBtn.gameObject.SetActive(false);
                    closeBtn.gameObject.SetActive(true);
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().SetOpenItemPos(index, shopType, height);

                });
                closeBtn.onClick.Add(() =>
                {
                    closeBtn.gameObject.SetActive(false);
                    openBtn.gameObject.SetActive(true);
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().SetCloseItemPos(index, shopType, height);
                });
            }
            
        }
    }
}
