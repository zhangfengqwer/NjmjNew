using ETModel;
using Hotfix;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIActivitySystem : StartSystem<UIActivityComponent>
    {
        public override void Start(UIActivityComponent self)
        {
            self.Start();
        }
    }

    public class UIActivityComponent : Component
    {
        private Button returnBtn;
        private GameObject grid;
        private Button NoticeBtn;
        private Button ActivityBtn;
        private GameObject Panel;
        private GameObject ActivityGrid;

        private GameObject noticeItem;
        private List<GameObject> objList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();

        private GameObject activityItem;
        private List<GameObject> activityItemList = new List<GameObject>();
        private List<UI> acUiList = new List<UI>();

        public void Start()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            grid = rc.Get<GameObject>("Grid");
            ActivityGrid = rc.Get<GameObject>("ActivityGrid");
            NoticeBtn = rc.Get<GameObject>("NoticeBtn").GetComponent<Button>();
            ActivityBtn = rc.Get<GameObject>("ActivityBtn").GetComponent<Button>();
            Panel = rc.Get<GameObject>("Panel");
            noticeItem = CommonUtil.getGameObjByBundle(UIType.UINoticeItem);
            activityItem = CommonUtil.getGameObjByBundle(UIType.UIActivityItem);
            //await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);

            GetActivityItemList();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIActivity);
            });

            NoticeBtn.onClick.Add(() =>
            {
                NoticeBtn.transform.GetChild(0).gameObject.SetActive(true);
                ActivityBtn.transform.GetChild(0).gameObject.SetActive(false);
                CreateNoticeItems();
            });

            ActivityBtn.onClick.Add(() =>
            {
                GetActivityItemList();
            });
            
        }

        public Transform GetActivityParent()
        {
            return Panel.transform;
        }

        private void GetActivityItemList()
        {
            NoticeBtn.transform.GetChild(0).gameObject.SetActive(false);
            ActivityBtn.transform.GetChild(0).gameObject.SetActive(true);
            CreateActivityItems(ActivityConfig.getInstance().getActivityInfoList());
        }

        private void CreateActivityItems(List<ActivityInfo> activityList)
        {
            GameObject obj = null;
            for(int i = 0;i< activityList.Count; ++i)
            {
                if (i < activityItemList.Count)
                    obj = activityItemList[i];
                else
                {
                    obj = GameObject.Instantiate(activityItem);
                    obj.transform.SetParent(ActivityGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    activityItemList.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIActivityItemComponent>();
                    uiList.Add(ui);
                }
                uiList[i].GetComponent<UIActivityItemComponent>().SetInfo(activityList[i]);
            }
        }

        private void CreateNoticeItems()
        {
            GameObject obj = null;
            for (int i = 0; i < NoticeConfig.getInstance().getDataList().Count; ++i)
            {
                NoticeInfo info = NoticeConfig.getInstance().getDataList()[i];
                obj = GameObject.Instantiate(noticeItem);
                obj.transform.SetParent(grid.transform);
                obj.transform.localPosition = new Vector3(10, 10 + 158 * i, 0);
                obj.transform.localScale = Vector3.one;
                #region 
                UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                ui.AddComponent<UINoticeItemComponent>();
                objList.Add(obj);
                uiList.Add(ui);
                ui.GetComponent<UINoticeItemComponent>().SetText(info);
                ui.GetComponent<UINoticeItemComponent>().SetLine();
                #endregion
            }
            //float height = grid.transform.childCount * 158f;
            //grid.GetComponent<RectTransform>().rect.Set(-631, 318.3497f, 100, 1500);
            //grid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            //for(int i = 0;i< grid.transform.childCount; ++i)
            //{
            //    float y = (float)(-158) * (i)-
            //        (uiList[i].GetComponent<UINoticeItemComponent>().GetTextRow() - 1) * 34;
            //    grid.transform.GetChild(i).transform.localPosition = new Vector3(594.0f, y, 0.0f);
            //}
        }
    }
}
