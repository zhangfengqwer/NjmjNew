using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChengjiuSystem : AwakeSystem<UIChengjiuComponent>
    {
        public override void Awake(UIChengjiuComponent self)
        {
            self.Awake();
        }
    }
    public class UIChengjiuComponent : Component
    {
        private Button ReturnBtn;
        private GameObject Grid;
        private Text CurProgress;
        private Image ChengIcon;
        private Button CloseBtn;
        private Text RewardTxt;
        private Text ProgressTxt;
        private Text ContentTxt;
        private Text NameTxt;
        private GameObject Detail;

        private GameObject item;
        private List<GameObject> itemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            ReturnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            Grid = rc.Get<GameObject>("Grid");
            CurProgress = rc.Get<GameObject>("CurProgress").GetComponent<Text>();
            Detail = rc.Get<GameObject>("Detail");
            ChengIcon = rc.Get<GameObject>("ChengIcon").GetComponent<Image>();
            CloseBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();
            ContentTxt = rc.Get<GameObject>("ContentTxt").GetComponent<Text>();
            NameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            ProgressTxt = rc.Get<GameObject>("ProgressTxt").GetComponent<Text>();
            RewardTxt = rc.Get<GameObject>("RewardTxt").GetComponent<Text>();
            item = CommonUtil.getGameObjByBundle(UIType.UIChengjiuItem);

            ReturnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChengjiu);
            });

            CloseBtn.onClick.Add(() =>
            {
                Detail.SetActive(false);
            });
            CreateItems();
        }

        private void CreateItems()
        {
            GameObject obj = null;
            for(int i = 0;i< PlayerInfoComponent.Instance.GetChengjiuList().Count; ++i)
            {
                TaskInfo info = PlayerInfoComponent.Instance.GetChengjiuList()[i];
                if (i < itemList.Count)
                    obj = itemList[i];
                else
                {
                    obj = GameObject.Instantiate(item);
                    obj.transform.SetParent(Grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIChengjiuItemComponent>();
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UIChengjiuItemComponent>().SetInfo(info);
            }
        }

        public void SetDetail(TaskInfo info)
        {
            Detail.SetActive(true);
            ProgressTxt.text = new StringBuilder().Append(0)
                                                  .Append("/")
                                                  .Append(info.Target).ToString();
            ContentTxt.text = info.Desc;
            NameTxt.text = info.TaskName;
            RewardTxt.text = new StringBuilder().Append("金币")
                                                .Append(info.Reward).ToString();
            string icon = new StringBuilder().Append("chengjiu_")
                                             .Append(info.Id).ToString();
            ChengIcon.sprite = CommonUtil.getSpriteByBundle("uichengjiuicon", icon);
        }
    }
}
