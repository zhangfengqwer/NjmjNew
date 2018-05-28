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
                CreateNoticeItems();
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
        /// 创建通知列表
        /// </summary>
        private void CreateNoticeItems()
        {
            #region test
            //NoticeInfo info1 = new NoticeInfo();
            //info1.id = 104;
            //info1.content = "欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入欢迎加入南京麻将3欢迎加入南京麻将3";
            //NoticeConfig.getInstance().getDataList().Add(info1);
            //info1 = new NoticeInfo();
            //info1.id = 104;
            //info1.content = "欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入南京麻将3欢迎加入欢迎加入南京麻将3欢迎加入南京麻将3";
            //NoticeConfig.getInstance().getDataList().Add(info1);
            #endregion

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
                    int state = PlayerPrefs.GetInt(info.id.ToString());
                    if (state != 1)
                    {
                        obj.transform.SetAsFirstSibling();
                        objList.Insert(0, obj);
                        uiList.Insert(0, ui);
                    }
                    else
                    {
                        objList.Add(obj);
                        uiList.Add(ui);
                    }
                    ui.GetComponent<UINoticeItemComponent>().SetText(info);
                    ui.GetComponent<UINoticeItemComponent>().SetLine();
                }
            }

            #region 
            float TotalTextHeight = 0;
            float objHeight = noticeItem.GetComponent<RectTransform>().rect.height;
            for (int i = 0; i< uiList.Count; ++i)
            {
                TotalTextHeight += uiList[i].GetComponent<UINoticeItemComponent>().GetTextHeight();
            }
            float height = TestGrid.transform.childCount * objHeight + TotalTextHeight + 50;
            TestGrid.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            for (int i = 0; i < TestGrid.transform.childCount; ++i)
            {
                float y = 0;
                if (i == 0)
                {
                    y = (float)(-objHeight) * i;
                }
                else
                {
                    y = (float)TestGrid.transform.GetChild(i - 1).transform.localPosition.y - objHeight - (uiList[i - 1].GetComponent<UINoticeItemComponent>().GetTextHeight());
                }

                TestGrid.transform.GetChild(i).transform.localPosition = new Vector3(0, y, 0.0f);
               // Debug.Log("pos ：" + TestGrid.transform.GetChild(i).transform.localPosition);
            }
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
