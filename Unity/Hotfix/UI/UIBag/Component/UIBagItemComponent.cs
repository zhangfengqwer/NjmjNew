using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIBagItemSystem : AwakeSystem<UIBagItemComponent>
    {
        public override void Awake(UIBagItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIBagItemComponent : Component
    {
        private GameObject uiBagItem;
        private Text countTxt;
        private GameObject uiBagBgL;
        private Image uiBagIcon;
        private Bag item;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            uiBagItem = rc.Get<GameObject>("UIBagItem");
            countTxt = rc.Get<GameObject>("CountTxt").GetComponent<Text>();
            uiBagBgL = rc.Get<GameObject>("UIBagBgL");
            uiBagIcon = rc.Get<GameObject>("UIBagIcon").GetComponent<Image>();

            uiBagItem.GetComponent<Button>().onClick.Add(() =>
            {
                //显示物品信息
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIBag).GetComponent<UIBagComponent>().SetItemInfo(item);
            });
        }

        public void SetItemInfo(Bag item,int index)
        {
            this.item = item;
            if (index % 3 == 1)
                uiBagBgL.SetActive(true);
            else
                uiBagBgL.SetActive(false);
            uiBagIcon.sprite = CommonUtil.getSpriteByBundle("image_shop", item.ItemId.ToString());
            countTxt.text = item.Count.ToString();
        }
    }
}
