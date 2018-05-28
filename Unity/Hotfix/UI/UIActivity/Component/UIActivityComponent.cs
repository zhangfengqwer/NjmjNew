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
        private GameObject TestGrid;

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
            TestGrid = rc.Get<GameObject>("TestGrid");

            //await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);

            GetActivityItemList();

            //返回
            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIActivity);
            });

            //点击显示通知栏
            NoticeBtn.onClick.Add(() =>
            {
                NoticeBtn.transform.GetChild(0).gameObject.SetActive(true);
                ActivityBtn.transform.GetChild(0).gameObject.SetActive(false);
                //CreateNoticeItems();
                CreateNoticeItemsTest();
            });

            //点击显示活动栏
            ActivityBtn.onClick.Add(() =>
            {
                GetActivityItemList();
            });
            
        }

        /// <summary>
        /// 获得父物体（单个活动页面用到）
        /// </summary>
        /// <returns></returns>
        public Transform GetActivityParent()
        {
            return Panel.transform;
        }

        /// <summary>
        /// button实现点击按钮实现不同的功能列表
        /// </summary>
        private void GetActivityItemList()
        {
            NoticeBtn.transform.GetChild(0).gameObject.SetActive(false);
            ActivityBtn.transform.GetChild(0).gameObject.SetActive(true);
            CreateActivityItems(ActivityConfig.getInstance().getActivityInfoList());
        }

        /// <summary>
        /// 创建活动列表
        /// </summary>
        /// <param name="activityList"></param>
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
                    if (i == 0)
                        ui.GetComponent<UIActivityItemComponent>().OnClick(activityList[i].id);
                    acUiList.Add(ui);
                }
                acUiList[i].GetComponent<UIActivityItemComponent>().SetInfo(activityList[i]);
            }
        }

        /// <summary>
        /// 创建通知列表(test)
        /// </summary>
        private void CreateNoticeItemsTest()
        {
            GameObject obj = null;
            for (int i = 0; i < NoticeConfig.getInstance().getDataList().Count; ++i)
            {
                NoticeInfo info = NoticeConfig.getInstance().getDataList()[i];
                if (i < objList.Count)
                    obj = objList[i];
                else
                {
                    obj = GameObject.Instantiate(noticeItem);
                    obj.transform.SetParent(TestGrid.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;

                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UINoticeItemComponent>();
                    objList.Add(obj);
                    uiList.Add(ui);
                }
                //if (i == 0)
                //    info.content += "欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入";
                uiList[i].GetComponent<UINoticeItemComponent>().SetText(info);
                uiList[i].GetComponent<UINoticeItemComponent>().SetLine();
            }

            #region 
            float height = TestGrid.transform.childCount * 170f;
            TestGrid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 240);
            for (int i = 0; i < TestGrid.transform.childCount; ++i)
            {
                Debug.Log("物体高度：" + TestGrid.transform.GetChild(i).GetComponent<RectTransform>().rect.height);
                float y = (float)(-170) * (i) -
                    (uiList[i].GetComponent<UINoticeItemComponent>().GetTextRow()) * 34;
                TestGrid.transform.GetChild(i).transform.localPosition = new Vector3(0, y, 0.0f);
            }
            #endregion
        }

        /// <summary>
        /// 创建通知列表
        /// </summary>
        private void CreateNoticeItems()
        {
            GameObject obj = null;
            for (int i = 0; i < NoticeConfig.getInstance().getDataList().Count; ++i)
            {
                NoticeInfo info = NoticeConfig.getInstance().getDataList()[i];
                if (i < objList.Count)
                    obj = objList[i];
                else
                {
                    obj = GameObject.Instantiate(noticeItem);
                    obj.transform.SetParent(grid.transform);
                    obj.transform.localPosition = new Vector3(10, 10 + 158 * i, 0);
                    obj.transform.localScale = Vector3.one;
                    
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UINoticeItemComponent>();
                    objList.Add(obj);
                    uiList.Add(ui);
                    
                }
                uiList[i].GetComponent<UINoticeItemComponent>().SetText(info);
                uiList[i].GetComponent<UINoticeItemComponent>().SetLine();
            }

            #region 
            //float height = grid.transform.childCount * 158f;
            //grid.GetComponent<RectTransform>().rect.Set(-631, 318.3497f, 100, 1500);
            //grid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            //for(int i = 0;i< grid.transform.childCount; ++i)
            //{
            //    float y = (float)(-158) * (i)-
            //        (uiList[i].GetComponent<UINoticeItemComponent>().GetTextRow() - 1) * 34;
            //    grid.transform.GetChild(i).transform.localPosition = new Vector3(594.0f, y, 0.0f);
            //}
            #endregion
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            activityItemList.Clear();
            acUiList.Clear();
            objList.Clear();
            uiList.Clear();
        }
    }
}
