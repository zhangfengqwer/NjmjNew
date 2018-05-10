using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
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
        private Toggle wingToggle;
        private Toggle goldToggle;
        private Toggle proToggle;
        private Toggle vipToggle;
        private Button returnBtn;
        private GameObject wingGrid;
        private GameObject goldGrid;
        private GameObject propGrid;
        private GameObject vipGrid;
        private List<ShopInfo> shopInfoList = new List<ShopInfo>();
        private Dictionary<int, List<ShopInfo>> shopInfoDic = new Dictionary<int, List<ShopInfo>>();
        private List<GameObject> itemList = new List<GameObject>();
        private Dictionary<ShopType, List<GameObject>> itemDic = new Dictionary<ShopType, List<GameObject>>();
        private List<UI> uiList = new List<UI>();
        private Dictionary<ShopType, List<UI>> uiDic = new Dictionary<ShopType, List<UI>>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            PlayerInfoComponent playerInfoCom = Game.Scene.GetComponent<PlayerInfoComponent>();
            wingToggle = rc.Get<GameObject>("WingToggle").GetComponent<Toggle>();
            goldToggle = rc.Get<GameObject>("GoldToggle").GetComponent<Toggle>();
            proToggle = rc.Get<GameObject>("ProToggle").GetComponent<Toggle>();
            vipToggle = rc.Get<GameObject>("VipToggle").GetComponent<Toggle>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            wingGrid = rc.Get<GameObject>("WingGrid");
            goldGrid = rc.Get<GameObject>("GoldGrid");
            propGrid = rc.Get<GameObject>("PropGrid");
            vipGrid = rc.Get<GameObject>("VipGrid");
            shopInfoList = playerInfoCom.GetShopInfoList();
            AddShopInfoByType();
            List<ShopInfo> shopTypeInfoList = new List<ShopInfo>();

            wingToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    shopInfoList = GetShopInfoByType((int)ShopType.Wing);
                    GameObject bundle = GetItemBundleByType(UIType.UIWingItem);
                    CreateShopInfoList(shopInfoList, bundle,ShopType.Wing,wingGrid.transform);
                }
            });

            wingToggle.onValueChanged.Invoke(true);
            goldToggle.onValueChanged.Invoke(false);
            proToggle.onValueChanged.Invoke(false);
            vipToggle.onValueChanged.Invoke(false);

            goldToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    shopInfoList = GetShopInfoByType((int)ShopType.Gold);
                    GameObject bundle = GetItemBundleByType(UIType.UIGoldItem);
                    CreateShopInfoList(shopInfoList, bundle, ShopType.Gold, goldGrid.transform);
                }
            });

            proToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    shopInfoList = GetShopInfoByType((int)ShopType.Prop);
                }
            });

            vipToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    shopInfoList = GetShopInfoByType((int)ShopType.Vip);
                }
            });

        }

        private GameObject GetItemBundleByType(string itemType)
        {
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{itemType}.unity3d");
            return (GameObject)resourcesComponent.GetAsset($"{itemType}.unity3d", $"{itemType}");
        }

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
                        break;
                    case ShopType.Vip:
                        break;
                }
            }
        }

        private List<GameObject> GetItemListBytype(ShopType type)
        {
            if (itemDic.ContainsKey(type))
                return itemDic[type];
            return null;
        }

        private List<UI> GetUiListByType(ShopType type)
        {
            if (uiDic.ContainsKey(type))
                return uiDic[type];
            return null;
        }

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

        private List<ShopInfo> GetShopInfoByType(int type)
        {
            return shopInfoDic[type];
        }
    }
}
