using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIDuanwuActivitySystem : StartSystem<UIDuanwuActivityComponent>
    {
        public override void Start(UIDuanwuActivityComponent self)
        {
            self.Start();
        }
    }

    public class UIDuanwuActivityComponent : Component
    {
        private Text Des;
        private Text Reward;
        private Text Progress;
        private Text ActivityFinishTime;
        private Text OwnZongziCount;
        private GameObject TaskGrid;
        private GameObject TreasureGrid;
        private Button RefreshTaskBtn;
        private Button TreasureBtn;
        private Button CloseDuanwuBtn;//点击关闭端午活动界面
        private GameObject Treasure;
        private Button CloseTreasureBtn;//点击关闭宝箱界面

        #region duanwu
        private GameObject taskItem = null;
        private List<GameObject> taskItemList = new List<GameObject>();
        private List<UI> uilist = new List<UI>();
        private List<DuanwuActivity> duanwuActInfoList = new List<DuanwuActivity>();
        private string activityType = "";
        public DuanwuData duanwuData;
        #endregion

        #region treasure
        private GameObject treasureItem = null;
        private List<GameObject> treasureItemList = new List<GameObject>();
        private List<UI> treasurUiList = new List<UI>();
        private List<TreasureInfo> treasureInfoList = new List<TreasureInfo>();
        #endregion

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Des = rc.Get<GameObject>("Des").GetComponent<Text>();
            Reward = rc.Get<GameObject>("Reward").GetComponent<Text>();
            Progress = rc.Get<GameObject>("Progress").GetComponent<Text>();
            ActivityFinishTime = rc.Get<GameObject>("ActivityFinishTime").GetComponent<Text>();
            OwnZongziCount = rc.Get<GameObject>("OwnZongziCount").GetComponent<Text>();

            RefreshTaskBtn.onClick.Add(() =>
            {
                if (duanwuData != null && duanwuData.RefreshCount > 0)
                {
                    int goldCost = 0;
                    switch (duanwuData.RefreshCount)
                    {
                        case 0:
                            ToastScript.createToast("今日刷新次数已用完");
                            break;
                        case 1:
                            goldCost = 50000;
                            break;
                        case 2:
                            goldCost = 30000;
                            break;
                        case 3:
                            goldCost = 10000;
                            break;
                        default:
                            Log.Debug("刷新次数超出,检查代码逻辑");
                            break;
                    }
                    RefreshActivityType(goldCost);
                }
            });

            TreasureBtn.onClick.Add(() =>
            {
                //显示宝箱界面
                Treasure.SetActive(true);
            });

            CloseDuanwuBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove("duanwu");
            });

            CloseTreasureBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove("baoxiang");
            });

            GetActivityTypeRequest();
        }

        /// <summary>
        /// 向服务器请求活动类型（）
        /// </summary>
        private async void GetActivityTypeRequest()
        {
            UINetLoadingComponent.showNetLoading();
            //type = 1 则为单纯请求数据
            G2C_DuanwuDataBase g2cDuanwu = (G2C_DuanwuDataBase)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_DuanwuDataBase { UId = PlayerInfoComponent.Instance.uid, Type = 1 });
            UINetLoadingComponent.closeNetLoading();
            duanwuData = g2cDuanwu.DuanwuData;

            //如果玩家随机活动类型为空，则添加随机六个活动
            if (string.IsNullOrEmpty(g2cDuanwu.DuanwuData.ActivityType))
            {
                activityType = GetRandomIndex();
                //type = 2 更改活动类型
                G2C_DuanwuDataBase g2cDuanwu1 = (G2C_DuanwuDataBase)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_DuanwuDataBase { UId = PlayerInfoComponent.Instance.uid, Type = 2, ActivityType = activityType });
            }
            else
            {
                activityType = g2cDuanwu.DuanwuData.ActivityType;
            }
            GetDuanwuTaskInfoList();
        }

        private async void RefreshActivityType(int goldCost)
        {
            activityType = GetRandomIndex();
            //type = 3 刷新活动
            G2C_DuanwuDataBase g2cDuanwu = (G2C_DuanwuDataBase)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_DuanwuDataBase { UId = PlayerInfoComponent.Instance.uid, Type = 4, ActivityType = activityType, GoldCost = goldCost });
        }

        /// <summary>
        /// 向服务器请求端午任务列表
        /// </summary>
        private async void GetDuanwuTaskInfoList()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_DuanwuActivity g2cduanwu = (G2C_DuanwuActivity)await SessionWrapComponent.Instance.Session.Call(new C2G_DuanwuActivity() { UId = PlayerInfoComponent.Instance.uid });
            duanwuActInfoList = g2cduanwu.DuanwuActivityList;
            UINetLoadingComponent.closeNetLoading();

            Init();
            CreateDuanwuList();
        }

        /// <summary>
        /// 界面初始化数据
        /// </summary>
        private void Init()
        {
            //活动结束时间
            ActivityFinishTime.text = "";
            OwnZongziCount.text = "粽子个数:" + duanwuData.ZongziCount;
        }

        /// <summary>
        /// 创建端午活动任务列表
        /// </summary>
        private void CreateDuanwuList()
        {
            GameObject obj = null;
            for(int i = 0;i< activityType.Length; ++i)
            {
                DuanwuActivity activity = duanwuActInfoList[activityType[i]];
                if (i < taskItemList.Count)
                {
                    obj = taskItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(taskItem);
                    obj.transform.SetParent(TaskGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIDuanwuActivityItemComponent>();
                    uilist.Add(ui);
                    taskItemList.Add(obj);
                }
                uilist[i].GetComponent<UIDuanwuActivityItemComponent>().SetItemInfo(activity);
            }
        }

        /// <summary>
        /// 显示领取粽子的动画
        /// </summary>
        /// <param name="count"></param>
        public void ShowAddZongziCount(int count)
        {
            //显示领取了多少粽子，暂不处理
        }

        /// <summary>
        /// 设置当前点击的活动具体信息
        /// </summary>
        /// <param name="activity"></param>
        public void SetCurActivityInfo(DuanwuActivity activity)
        {
            Des.text = activity.Desc;
            Reward.text = activity.Reward.ToString();
            Progress.text = activity.CurProgress + "/" + activity.Target;
        }

        #region 宝箱
        private async void GetTreasureData()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_DuanwuTreasure g2ctreasure = (G2C_DuanwuTreasure)await SessionWrapComponent.Instance.Session.Call(new C2G_DuanwuTreasure { });
            UINetLoadingComponent.closeNetLoading();
            treasureInfoList = g2ctreasure.TreasureInfoList;
        }

        private void CreateTreasureList()
        {
            GameObject obj = null;
            for(int i = 0;i< treasurUiList.Count; ++i)
            {
                if(i < treasureItemList.Count)
                {
                    obj = treasureItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(treasureItem);
                    obj.transform.SetParent(TreasureGrid.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    treasureItemList.Add(obj);
                }
            }
        }

        #endregion

        /// <summary>
        /// 从12个活动里面随机获取六个活动
        /// </summary>
        /// <returns></returns>
        private string GetRandomIndex()
        {
            List<int> randomIndexList = new List<int>();
            int i = 6;
            StringBuilder result = new StringBuilder();
            while (i > 0)
            {
                int index = Common_Random.getRandom(0, 11);
                if (!randomIndexList.Contains(index))
                {
                    randomIndexList.Add(index);
                    result.Append(index);
                    i--;
                }
            }
            return result.ToString();
        }

        //清理内存
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
