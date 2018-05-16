using ETModel;
using Hotfix;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIBagSystem : AwakeSystem<UIBagComponent>
    {
        public override void Awake(UIBagComponent self)
        {
            self.Awake();
        }
    }

    public class UIBagComponent : Component
    {
        /*UseBtn DescTxt UIItemIcon Grid BgGrid ReturnBtn
         */
        private Button useBtn;
        private Text descTxt;
        private Image uiItemIcon;
        private GameObject grid;
        private GameObject bgGrid;
        private Button returnBtn;
        private GameObject bagItem = null;
        private List<GameObject> bagItemList = new List<GameObject>();
        private GameObject bgItem = null;
        private List<GameObject> bgItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        //private int row = 3;//初始三行
        //private int itemCount = 3;//每行个数

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            useBtn = rc.Get<GameObject>("UseBtn").GetComponent<Button>();
            descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
            uiItemIcon = rc.Get<GameObject>("UIItemIcon").GetComponent<Image>();
            grid = rc.Get<GameObject>("Grid");
            bgGrid = rc.Get<GameObject>("BgGrid");
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIBag);
            });
            bagItem = CommonUtil.getGameObjByBundle(UIType.UIBagItem);
            bgItem = CommonUtil.getGameObjByBundle(UIType.UIBagBgL);

            GetBagInfoList();
        }

        private async void GetBagInfoList()
        {
            long uid = PlayerInfoComponent.Instance.uid;
            G2C_BagOperation g2cBag =(G2C_BagOperation) await SessionWrapComponent.Instance.Session.Call(new C2G_BagOperation() { UId = uid });
            CreateItemList(g2cBag.ItemList);
        }

        private void CreateItemList(List<Item> itemList)
        {
            GameObject obj = null;
            for (int i = 0; i < itemList.Count; ++i)
            {
                if (i < bagItemList.Count)
                    obj = bagItemList[i];
                else
                {
                    obj = GameObject.Instantiate(bagItem);
                    obj.transform.SetParent(grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    bagItemList.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIBagItemComponent>();
                    uiList.Add(ui);
                }
                SetItemInfo(itemList[0]);
                uiList[i].GetComponent<UIBagItemComponent>().SetItemInfo(itemList[i], i + 1);
            }
        }

        public void SetItemInfo(Item item)
        {
            PropInfo propInfo = PropConfig.getInstance().getPropInfoById((int)item.ItemId);
            uiItemIcon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(propInfo.prop_id.ToString());
            descTxt.text = propInfo.desc;
        }

        //private void SetBagItemL(int count)
        //{
        //    if(count > (row * itemCount))
        //    {
        //        int bgCount = (count - row * itemCount) / itemCount;
        //        if ((count - row * itemCount) % itemCount != 0)
        //            bgCount += 1;
        //        GameObject obj = null;
        //        for(int i = 0;i< bgCount; ++i)
        //        {
        //            if (i < bgItemList.Count)
        //                obj = bgItemList[i];
        //            else
        //            {
        //                obj = GameObject.Instantiate(bgItem);
        //                obj.transform.SetParent(bgGrid.transform);
        //                obj.transform.localScale = Vector3.one;
        //                obj.transform.localPosition = Vector3.zero;
        //                bgItemList.Add(obj);
        //            }
        //        }
        //    }
        //}

        public override void Dispose()
        {
            base.Dispose();
            uiList.Clear();
            bagItemList.Clear();
        }
    }
}
