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
        private GameObject grid;
        private List<ShopInfo> shopInfoList = new List<ShopInfo>();
        private Dictionary<int, List<ShopInfo>> shopInfoDic = new Dictionary<int, List<ShopInfo>>();
        private List<GameObject> wingItemList = new List<GameObject>();
        private List<UI> UiList = new List<UI>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            PlayerInfoComponent playerInfoCom = Game.Scene.GetComponent<PlayerInfoComponent>();
            wingToggle = rc.Get<GameObject>("WingToggle").GetComponent<Toggle>();
            goldToggle = rc.Get<GameObject>("GoldToggle").GetComponent<Toggle>();
            proToggle = rc.Get<GameObject>("ProToggle").GetComponent<Toggle>();
            vipToggle = rc.Get<GameObject>("VipToggle").GetComponent<Toggle>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            shopInfoList = playerInfoCom.GetShopInfoList();
            AddShopInfoByType();
            List<ShopInfo> shopTypeInfoList = new List<ShopInfo>();

            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle($"{UIType.UIWingItem}.unity3d");
            GameObject winItemObj = (GameObject)resourcesComponent.GetAsset($"{UIType.UIWingItem}.unity3d", $"{UIType.UIWingItem}");

            wingToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    shopInfoList = GetShopInfoByType((int)ShopType.Wing);
                    GameObject obj = null;
                    for (int i = 0; i < shopInfoList.Count; ++i)
                    {
                        if(i < wingItemList.Count)
                        {
                            obj = wingItemList[i];
                        }
                        else
                        {
                            obj = UnityEngine.Object.Instantiate(winItemObj);
                            obj.transform.SetParent(grid.transform);
                            obj.transform.localScale = Vector3.one;
                            obj.transform.localPosition = Vector3.zero;
                            UI ui = ComponentFactory.CreateWithParent<UI, GameObject>(this, obj);
                            ui.AddComponent<UIWingComponent>();
                            wingItemList.Add(obj);
                            UiList.Add(ui);
                        }
                        UiList[i].GetComponent<UIWingComponent>().SetShopInfo(shopInfoList[i]);
                    }
                }
            });

            goldToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (isOn)
                {
                    shopInfoList = GetShopInfoByType((int)ShopType.Gold);
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
