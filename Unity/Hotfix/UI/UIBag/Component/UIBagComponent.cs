using ETModel;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIBagSystem : StartSystem<UIBagComponent>
    {
        public override void Start(UIBagComponent self)
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
        private GameObject useBg;
        private Button sureBtn;
        private Button cancelBtn;
        private Text useTxt;
        private GameObject bagItem = null;
        private List<GameObject> bagItemList = new List<GameObject>();
        private GameObject bgItem = null;
        private List<GameObject> bgItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<Bag> bagList;
        private Bag item;
        private PropInfo propInfo;
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
            useBg = rc.Get<GameObject>("UseBg");
            sureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            cancelBtn = rc.Get<GameObject>("CancelBtn").GetComponent<Button>();
            useTxt = rc.Get<GameObject>("UseTxt").GetComponent<Text>();
            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIBag);
            });
            bagItem = CommonUtil.getGameObjByBundle(UIType.UIBagItem);
            bgItem = CommonUtil.getGameObjByBundle(UIType.UIBagBgL);
            useBg.SetActive(false);

            useBtn.onClick.Add(() =>
            {
                useBg.SetActive(true);
                useTxt.text = new StringBuilder().Append("是否使用道具")
                                                 .Append("\"")
                                                 .Append(propInfo.prop_name)
                                                 .Append("\"").ToString();
            });
            sureBtn.onClick.Add(() =>
            {
                UseItem(item);
            });
            cancelBtn.onClick.Add(() =>
            {
                useBg.SetActive(false);
            });
            GetBagInfoList();
        }

        private async void GetBagInfoList()
        {
            long uid = PlayerInfoComponent.Instance.uid;
            UINetLoadingComponent.showNetLoading();
            G2C_BagOperation g2cBag = (G2C_BagOperation)await SessionWrapComponent.Instance.Session.Call(new C2G_BagOperation() { UId = uid });
            UINetLoadingComponent.closeNetLoading();

            bagList = g2cBag.ItemList;
            PlayerInfoComponent.Instance.SetBagInfoList(bagList);
            if (item != null && item.ItemId == 105 && !IsCurPropUseUp())
            {
                useBg.SetActive(false);
            }
            CreateItemList(g2cBag.ItemList);
        }

        private bool IsCurPropUseUp()
        {
            if(item != null)
            {
                for(int i = 0;i< bagList.Count; ++i)
                {
                    if (item.ItemId == bagList[i].ItemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CreateItemList(List<Bag> itemList)
        {
            GameObject obj = null;
            for (int i = 0; i < itemList.Count; ++i)
            {
                if (i < bagItemList.Count)
                {
                    bagItemList[i].SetActive(true);
                    obj = bagItemList[i];
                }
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
                if (item != null && IsCurPropUseUp())
                    SetItemInfo(item);
                else
                    SetItemInfo(itemList[0]);
                uiList[i].GetComponent<UIBagItemComponent>().SetItemInfo(itemList[i], i + 1);
            }
            SetMoreHide(itemList.Count);
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshUI()
        {
            GetBagInfoList();
        }

        private void SetMoreHide(int index)
        {
            for (int i = index; i < bagItemList.Count; ++i)
                bagItemList[i].SetActive(false);
        }

        public void SetItemInfo(Bag item)
        {
            this.item = item;
            propInfo = PropConfig.getInstance().getPropInfoById((int)item.ItemId);
            if (propInfo == null)
                Debug.LogError("道具信息错误");
            useBtn.gameObject.SetActive(propInfo.type == 1);
            uiItemIcon.sprite = CommonUtil.getSpriteByBundle("image_shop", propInfo.prop_id.ToString());
            descTxt.text = propInfo.desc;
        }

        private async void UseItem(Bag item)
        {
            try
            {
                switch (item.ItemId)
                {
                    // 喇叭
                    case 105:
                    {
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseLaBa);
                    }
                    break;

                    default:
                    {
                            UINetLoadingComponent.showNetLoading();
                            G2C_UseItem g2cBag = (G2C_UseItem)await SessionWrapComponent.Instance.Session.Call(new C2G_UseItem() { UId = PlayerInfoComponent.Instance.uid, ItemId = (int)item.ItemId });
                            UINetLoadingComponent.closeNetLoading();

                            if (g2cBag.result == 1)
                            {
                                GetBagInfoList();
                                useBg.SetActive(false);

                                switch (item.ItemId)
                                {
                                    // 表情包
                                    case 104:
                                        {
                                            PlayerInfoComponent.Instance.GetPlayerInfo().EmogiTime = g2cBag.time;
                                            ToastScript.createToast("使用成功");
                                        }
                                        break;

                                    // VIP体验卡
                                    case 107:
                                    case 108:
                                    case 109:
                                        {
                                            PlayerInfoComponent.Instance.GetPlayerInfo().VipTime = g2cBag.time;
                                            ToastScript.createToast("使用成功");
                                        }
                                        break;

                                    // 话费礼包
                                    case 111:
                                        {
                                            GameUtil.changeDataWithStr(g2cBag.reward);
                                            float huafei = CommonUtil.splitStr_End(g2cBag.reward, ':') / 100.0f;
                                            ToastScript.createToast("恭喜获得" + huafei + "元话费");
                                        }
                                        break;
                                }

                                GameUtil.changeData(item.ItemId, -1);
                            }
                            else
                            {
                                ToastScript.createToast(g2cBag.Message);
                            }
                        }
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("------------------" + ex.ToString());
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            uiList.Clear();
            bagItemList.Clear();
            item = null;
        }
    }
}
