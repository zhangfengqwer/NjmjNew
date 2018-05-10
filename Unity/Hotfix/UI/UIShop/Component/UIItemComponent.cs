using ETModel;
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

        public void Awake()
        {
            rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            priceTxt = rc.Get<GameObject>("PriceTxt").GetComponent<Text>();
            buyBtn = rc.Get<GameObject>("BuyBtn").GetComponent<Button>();

            buyBtn.onClick.Add(() =>
            {

            });
        }

        public void SetCommonItem(ShopInfo info)
        {
            UIIconComponent iconComp = Game.Scene.GetComponent<UIIconComponent>();
            icon.sprite = iconComp.GetSprite(info.Id.ToString());
            nameTxt.text = info.Name;
            priceTxt.text = info.Price.ToString();
        }

        public void SetGoldItem(ShopInfo info)
        {
            SetCommonItem(info);
            Text disCountTxt = rc.Get<GameObject>("DisCountTxt").GetComponent<Text>();
            Text descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
            string[] strArr = info.Desc.Split(';');
            descTxt.text = strArr[0];
            disCountTxt.text = strArr[1];
        }
        
        public void SetItem(ShopInfo info,int index,ShopType shopType)
        {
            SetCommonItem(info);
            this.index = index;
            float height = 0;
            Button openBtn = rc.Get<GameObject>("OpenBtn").GetComponent<Button>();
            Button closeBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();
            switch (shopType)
            {
                case ShopType.Prop:
                    Text descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
                    Text disCountTxt = rc.Get<GameObject>("DisCountTxt").GetComponent<Text>();
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
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().SetOpenItemPos(index, shopType,height);

            });
            closeBtn.onClick.Add(() =>
            {
                closeBtn.gameObject.SetActive(false);
                openBtn.gameObject.SetActive(true);
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().SetCloseItemPos(index,shopType,height);
            });
        }
    }
}
