using ETModel;
using Hotfix;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIActivityItemSystem : AwakeSystem<UIActivityItemComponent>
    {
        public override void Awake(UIActivityItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIActivityItemComponent : Component
    {
        public Button UIActivityTitle;
        private Text TitleTxt;
        public ActivityInfo info;

        private Dictionary<int, GameObject> activityDic = new Dictionary<int, GameObject>();

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIActivityTitle = rc.Get<GameObject>("UIActivityTitle").GetComponent<Button>();
            TitleTxt = rc.Get<GameObject>("TitleTxt").GetComponent<Text>();
            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);
            UIActivityTitle.onClick.Add(() =>
            {
                OnClick(info.id);
            });

            UIActivityTitle.GetComponent<Image>().color = Color.red;
        }

        public void OnClick(int id)
        {
            GameUtil.GetComponentByType<UIActivityComponent>(UIType.UIActivity).SetSelect(id);
            if (activityDic.ContainsKey(id))
            {
                SetActive(id);
            }
            else
            {
                //根据不同的活动ID显示不同的活动面板
                string panelName = "UIActivity_" + id;
                GameObject obj = CommonUtil.getGameObjByBundle(panelName);

                GameObject activity = GameObject.Instantiate(obj, Game.Scene.GetComponent<UIComponent>().Get(UIType.UIActivity).GetComponent<UIActivityComponent>().GetActivityParent());
                
                //activity.transform.GetComponent<RectTransform>().setan
                UI ui = ComponentFactory.Create<UI, GameObject>(activity);
                if (id == 101)
                {
                    ui.AddComponent<UIActivity101Component>();
                }
                if (id == 102)
                {
                    ui.AddComponent<UIFriendActivityComponent>();
                }

                if (!activityDic.ContainsKey(id))
                {
                    activityDic.Add(id, activity);
                }
            }
            
        }

        private void SetActive(int id)
        {
            foreach(var item in activityDic)
            {
                if(item.Key == id)
                {
                    item.Value.SetActive(true);
                    item.Value.transform.SetAsLastSibling();
                }
                else
                {
                    item.Value.SetActive(false);
                }
            }
        }

        public void SetInfo(ActivityInfo info)
        {
            this.info = info;
            TitleTxt.text = info.title;
        }
    }
}
