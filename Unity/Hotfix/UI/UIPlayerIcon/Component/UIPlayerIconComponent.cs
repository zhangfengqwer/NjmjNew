﻿using ETModel;
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
        private Button returnBtn;
        private Button saveBtn;
        private GameObject grid;
        private Image curIcon;
        private const int iconCount = 10;
        private GameObject iconObj;
        private string curIconStr = "";
        private List<GameObject> iconList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private string[] iconStr = new string[] { "f_icon1", "f_icon2", "f_icon3" , "f_icon4" , "f_icon5","m_icon1", "m_icon2", "m_icon3", "m_icon4", "m_icon5" };

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            saveBtn = rc.Get<GameObject>("SaveBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            curIcon = rc.Get<GameObject>("CurIcon").GetComponent<Image>();
            iconObj = CommonUtil.getGameObjByBundle(UIType.UIIconItem);
            curIconStr = Game.Scene.GetComponent<PlayerInfoComponent>().GetPlayerInfo().Icon;
            curIcon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(curIconStr);
            CreatePlayerIconList();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIIcon);
            });

            saveBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<PlayerInfoComponent>().GetPlayerInfo().Icon = curIcon.sprite.name.ToString();
                PlayerInfo playerInfo = Game.Scene.GetComponent<PlayerInfoComponent>().GetPlayerInfo();
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().UpDatePlayerInfo();
            });
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
                    obj.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIPlayerIconItemComponent>();
                    iconList.Add(obj);
                    uiList.Add(ui);
                }
                if (curIconStr == iconStr[i])
                    uiList[i].GetComponent<UIPlayerIconItemComponent>().SetSelect(true);
                else
                    uiList[i].GetComponent<UIPlayerIconItemComponent>().SetSelect(false);
                Sprite sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(iconStr[i]);
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
        }
    }
}