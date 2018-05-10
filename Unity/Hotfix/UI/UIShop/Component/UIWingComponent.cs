using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIWingItemSystem : AwakeSystem<UIWingComponent>
    {
        public override void Awake(UIWingComponent self)
        {
            self.Awake();
        }
    }
    public class UIWingComponent : Component
    {
        private Image icon;
        private Text nameTxt;
        private Text priceTxt;
        private Button buyBtn;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            priceTxt = rc.Get<GameObject>("PriceTxt").GetComponent<Text>();
            buyBtn = rc.Get<GameObject>("BuyBtn").GetComponent<Button>();

            buyBtn.onClick.Add(() =>
            {

            });
        }

        public void SetShopInfo(ShopInfo info)
        {
            UIIconComponent iconComp = Game.Scene.GetComponent<UIIconComponent>();
            icon.sprite = iconComp.GetSprite(info.Id.ToString());
            nameTxt.text = info.Name;
            priceTxt.text = info.Price.ToString();
        }
    }
}
