using ETModel;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ProtoContract]
    public enum ShopType
    {
        Wing = 1,
        Gold = 2,
        Prop = 3,
        Vip = 4
    }

    [ObjectSystem]
    public class UIShopSystem : AwakeSystem<UIShopComponent>
    {
        public override void Awake(UIShopComponent self)
        {
            self.Awake();
        }
    }

    public class UIShopComponent : Component
    {
        #region UI
        private Button wingBtn;
        private Button goldBtn;
        private Button proBtn;
        private Button vipBtn;
        private Button returnBtn;
        private GameObject wingGrid;
        private GameObject goldGrid;
        private GameObject propGrid;
        private GameObject vipGrid;
        #endregion

        #region field
        //通过不同的商店类型保存相应的点击按钮 1:商店类型 2:点击按钮
        private Dictionary<ShopType, GameObject> buttonDic = new Dictionary<ShopType, GameObject>();
        //商品信息列表
        private List<ShopInfo> shopInfoList = new List<ShopInfo>();
        //全部的商品信息字典 int:商店类型
        private Dictionary<int, List<ShopInfo>> shopInfoDic = new Dictionary<int, List<ShopInfo>>();
        //物品obj缓存列表
        private List<GameObject> itemList = new List<GameObject>();
        //全部商品obj缓存列表 
        private Dictionary<ShopType, List<GameObject>> itemDic = new Dictionary<ShopType, List<GameObject>>();
        //ui缓存列表（之后可能会优化，只是刷新设置数据）
        private List<UI> uiList = new List<UI>();
        //全部ui缓存字典
        private Dictionary<ShopType, List<UI>> uiDic = new Dictionary<ShopType, List<UI>>();
        private float size;
        #endregion 

        public void Awake()
        {
            #region get
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            PlayerInfoComponent playerInfoCom = Game.Scene.GetComponent<PlayerInfoComponent>();
            wingBtn = rc.Get<GameObject>("WingToggle").GetComponent<Button>();
            goldBtn = rc.Get<GameObject>("GoldToggle").GetComponent<Button>();
            proBtn = rc.Get<GameObject>("ProToggle").GetComponent<Button>();
            vipBtn = rc.Get<GameObject>("VipToggle").GetComponent<Button>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            wingGrid = rc.Get<GameObject>("WingGrid");
            goldGrid = rc.Get<GameObject>("GoldGrid");
            propGrid = rc.Get<GameObject>("PropGrid");
            vipGrid = rc.Get<GameObject>("VipGrid");
            shopInfoList = playerInfoCom.GetShopInfoList();
            #endregion

            #region AddButton
            buttonDic.Add(ShopType.Wing, wingBtn.gameObject);
            buttonDic.Add(ShopType.Gold, goldBtn.gameObject);
            buttonDic.Add(ShopType.Vip, vipBtn.gameObject);
            buttonDic.Add(ShopType.Prop, proBtn.gameObject);
            #endregion

            //打开界面的时候刷新
            SetGrid();

            AddShopInfoByType();

            #region buttonClickEvt
            wingBtn.onClick.Add(() =>
            {
                ButtonClick(ShopType.Wing,UIType.UIWingItem,wingGrid.transform);
            });

            goldBtn.onClick.Add(() =>
            {
                ButtonClick(ShopType.Gold, UIType.UIGoldItem, goldGrid.transform);
            });

            proBtn.onClick.Add(() =>
            {
                ButtonClick(ShopType.Prop, UIType.UIPropItem, propGrid.transform);
            });

            returnBtn.onClick.Add(() =>
            {
                SetUIHideOrOpen(false);
            });

            wingBtn.onClick.Invoke();
            #endregion
        }

        public void SetUIHideOrOpen(bool isHide)
        {
            GetParent<UI>().GameObject.SetActive(isHide);
        }

        public void SetOpenItemPos(int index,ShopType type,float height)
        {
            if (type == ShopType.Prop)
            {
                propGrid.GetComponent<GridLayoutGroup>().enabled = false;
                propGrid.GetComponent<ContentSizeFitter>().enabled = false;
                float dis = propGrid.GetComponent<RectTransform>().rect.height + 150;
                propGrid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dis);
                for(int i = index + 1; i< propGrid.transform.childCount; ++i)
                {
                    propGrid.transform.GetChild(i).localPosition = new Vector3(propGrid.transform.GetChild(i).localPosition.x, propGrid.transform.GetChild(i).localPosition.y - 120, 0);
                }
            }
        }

        private void SetGrid()
        {
            SetPropItemDesActiveFalse();
            if (size != 0)
                propGrid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }

        public void SetCloseItemPos(int index)
        {
            float dis = propGrid.GetComponent<RectTransform>().rect.height - 150;
            propGrid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dis);
            for (int i = index + 1; i < propGrid.transform.childCount; ++i)
            {
                propGrid.transform.GetChild(i).localPosition = new Vector3(propGrid.transform.GetChild(i).localPosition.x, propGrid.transform.GetChild(i).localPosition.y + 120, 0);
            }
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <param name="shopType"></param>
        /// <param name="uiType"></param>
        /// <param name="parent"></param>
        private void ButtonClick(ShopType shopType,string uiType,Transform parent)
        {
            ClickShopType(shopType);
            shopInfoList = GetShopInfoByType((int)shopType);
            if (shopInfoList == null)
                shopInfoList = new List<ShopInfo>();
            GameObject bundle = GetItemBundleByType(uiType);
            CreateShopInfoList(shopInfoList, bundle, shopType, parent);
        }

        /// <summary>
        /// 点击不同的商店类型(显隐关系)
        /// </summary>
        /// <param name="type"></param>
        private void ClickShopType(ShopType type)
        {
            foreach(var item in buttonDic)
            {
                if(item.Key == type)
                {
                    item.Value.transform.Find("Select").gameObject.SetActive(true);
                }
                else
                {
                    item.Value.transform.Find("Select").gameObject.SetActive(false);
                }
            }   
        }

        /// <summary>
        /// 通过item的类型获得不同的bundle包
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private GameObject GetItemBundleByType(string itemType)
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{itemType}.unity3d");
            return (GameObject)resourcesComponent.GetAsset($"{itemType}.unity3d", $"{itemType}");
        }

        /// <summary>
        /// 创建商店列表
        /// </summary>
        /// <param name="shopInfoList"></param>
        /// <param name="itembundle"></param>
        /// <param name="type"></param>
        /// <param name="tr"></param>
        private void CreateShopInfoList(List<ShopInfo> shopInfoList,GameObject itembundle,ShopType type,Transform tr)
        {
            GameObject obj = null;
            uiList = GetUiListByType(type);
            itemList = GetItemListBytype(type);
            if (itemList == null)
                itemList = new List<GameObject>();
            if (uiList == null)
                uiList = new List<UI>();
            for (int i = 0; i < shopInfoList.Count; ++i)
            {
                if (i < itemList.Count)
                {
                    itemList[i].SetActive(true);
                    obj = itemList[i];
                }
                else
                {
                    obj = UnityEngine.Object.Instantiate(itembundle);
                    obj.transform.SetParent(tr);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIItemComponent>();
                    if (itemDic.ContainsKey(type))
                        itemDic[type].Add(obj);
                    else
                    {
                        itemList.Add(obj);
                        itemDic.Add(type, itemList);
                    }
                    if (uiDic.ContainsKey(type))
                        uiDic[type].Add(ui);
                    else
                    {
                        uiList.Add(ui);
                        uiDic.Add(type, uiList);
                    }
                }
                switch (type)
                {
                    case ShopType.Wing:
                        uiList[i].GetComponent<UIItemComponent>().SetCommonItem(shopInfoList[i]);
                        break;
                    case ShopType.Gold:
                        uiList[i].GetComponent<UIItemComponent>().SetGoldItem(shopInfoList[i]);
                        break;
                    case ShopType.Prop:
                        uiList[i].GetComponent<UIItemComponent>().SetPropItem(shopInfoList[i],i);
                        break;
                    case ShopType.Vip:
                        break;
                }
            }
            if (type == ShopType.Prop)
                size = propGrid.GetComponent<RectTransform>().rect.height;
        }
        
        private void SetPropItemDesActiveFalse()
        {
            if(propGrid.transform.childCount> 0)
            {
                for(int i = 0;i< propGrid.transform.childCount; ++i)
                {
                    GameObject obj = propGrid.transform.GetChild(i).gameObject;
                    if (obj.transform.Find("CloseBtn").gameObject.activeInHierarchy)
                        obj.transform.Find("CloseBtn").gameObject.SetActive(false);
                    obj.transform.Find("OpenBtn").gameObject.SetActive(true);
                }
            }
            
        }

        /// <summary>
        /// 通过商店类型获得不同的item列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<GameObject> GetItemListBytype(ShopType type)
        {
            if (itemDic.ContainsKey(type))
                return itemDic[type];
            return null;
        }

        /// <summary>
        /// 通过类型获得缓存的UI
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<UI> GetUiListByType(ShopType type)
        {
            if (uiDic.ContainsKey(type))
                return uiDic[type];
            return null;
        }

        /// <summary>
        /// 通过商店类型把不同的商店数据保存下来
        /// </summary>
        private void AddShopInfoByType()
        {
            for(int i = 0;i< shopInfoList.Count; ++i)
            {
                ShopInfo shopInfo = shopInfoList[i];
                if (shopInfoDic.ContainsKey(shopInfo.ShopType))
                {
                    List<ShopInfo> list = new List<ShopInfo>();
                    list = shopInfoDic[shopInfo.ShopType];
                    if (!list.Contains(shopInfo))
                        shopInfoDic[shopInfo.ShopType].Add(shopInfo);
                    else
                        Log.Error("已经存在该物品");
                }
                else
                {
                    shopInfoDic.Add(shopInfo.ShopType, new List<ShopInfo>());
                    shopInfoDic[shopInfo.ShopType].Add(shopInfo);
                }
            }
        }

        /// <summary>
        /// 通过类型获得商品信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<ShopInfo> GetShopInfoByType(int type)
        {
            return shopInfoDic[type];
        }
    }
}
