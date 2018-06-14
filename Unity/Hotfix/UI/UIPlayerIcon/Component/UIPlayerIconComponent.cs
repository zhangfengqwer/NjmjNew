using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPlayerIconSystem : AwakeSystem<UIPlayerIconComponent>
    {
        public override void Awake(UIPlayerIconComponent self)
        {
            self.Awake();
        }
    }
    public class UIPlayerIconComponent : Component
    {
        private GameObject CurIconFrame;

        private Button returnBtn;
        private Button saveBtn;
        private GameObject grid;
        private Image curIcon;
        private GameObject tip;
        private Button sureBtn;
        private int iconCount = 10;
        private GameObject iconObj;
        private string curIconStr = "";
        private List<GameObject> iconList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<string> iconStr = new List<string>();

        public async void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            CurIconFrame = rc.Get<GameObject>("CurIconFrame");

            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            saveBtn = rc.Get<GameObject>("SaveBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            curIcon = rc.Get<GameObject>("CurIcon").GetComponent<Image>();
            tip = rc.Get<GameObject>("Tip");
            sureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            iconObj = CommonUtil.getGameObjByBundle(UIType.UIIconItem);
            curIconStr = Game.Scene.GetComponent<PlayerInfoComponent>().GetPlayerInfo().Icon;

            // 设置头像
            {
                HeadManager.setHeadSprite(curIcon, PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
            }

            GetOwnIcon();

            CreatePlayerIconList();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIIcon);
            });

            saveBtn.onClick.Add(() =>
            {
                tip.SetActive(true);
                UpdateIcon();
            });

            sureBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIIcon);
            });

            if (GameUtil.isVIP())
            {
                CurIconFrame.transform.Find("HeadKuang").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", "touxiangkuang_vip");
            }
        }

        private void GetOwnIcon()
        {
            iconStr.Clear();
            iconCount = 0;
            iconStr = new List<string> { "f_icon1", "f_icon2", "f_icon3", "f_icon4", "f_icon5", "m_icon1", "m_icon2", "m_icon3", "m_icon4", "m_icon5" };
            string ownIcon = PlayerInfoComponent.Instance.ownIcon;
            iconCount += iconStr.Count;
            if (!string.IsNullOrEmpty(ownIcon))
            {
                List<string> list_str = new List<string>();
                CommonUtil.splitStr(ownIcon, list_str, ';');
                iconCount += list_str.Count;
                for (int i = 0; i < list_str.Count; ++i)
                {
                    iconStr.Add(list_str[i]);
                }
            }
        }

        private async void UpdateIcon()
        {
            Game.Scene.GetComponent<PlayerInfoComponent>().GetPlayerInfo().Icon = curIcon.sprite.name.ToString();

            UINetLoadingComponent.showNetLoading();
            G2C_UpdatePlayerInfo g2CPlayerInfo = (G2C_UpdatePlayerInfo)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdatePlayerInfo()
            {
                Uid = PlayerInfoComponent.Instance.uid,
                playerInfo = PlayerInfoComponent.Instance.GetPlayerInfo()
            });
            UINetLoadingComponent.closeNetLoading();

            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().refreshUI();
            Game.Scene.GetComponent<UIComponent>().Get(UIType.UIPlayerInfo).GetComponent<UIPlayerInfoComponent>().Update();
        }

        private void CreatePlayerIconList()
        {
            GameObject obj = null;
            for(int i = 0;i< iconCount; ++i)
            {
                if (i < iconList.Count)
                {
                    obj = iconList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(iconObj);
                    obj.transform.SetParent(grid.transform);
                    obj.transform.localScale = Vector3.one;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIPlayerIconItemComponent>();
                    iconList.Add(obj);
                    uiList.Add(ui);
                }
                if (curIconStr == iconStr[i])
                    uiList[i].GetComponent<UIPlayerIconItemComponent>().SetSelect(true);
                else
                    uiList[i].GetComponent<UIPlayerIconItemComponent>().SetSelect(false);
                Sprite sprite = CommonUtil.getSpriteByBundle("playericon",iconStr[i]);
                uiList[i].GetComponent<UIPlayerIconItemComponent>().SetIcon(sprite,i);
            }
        }

        public void SetSelectState(int index)
        {
            curIcon.sprite = uiList[index].GetComponent<UIPlayerIconItemComponent>().GetIcon();
            for (int i = 0;i< iconList.Count;++i)
            {
                GameObject obj = iconList[i];
                if (i == index)
                    uiList[i].GetComponent<UIPlayerIconItemComponent>().SetSelect(true);
                else
                    uiList[i].GetComponent<UIPlayerIconItemComponent>().SetSelect(false);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            iconList.Clear();
            uiList.Clear();
            iconStr.Clear();
        }
    }
}
