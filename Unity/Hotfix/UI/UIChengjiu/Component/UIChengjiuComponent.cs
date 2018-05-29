using System.Collections.Generic;
using System.Text;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChengjiuSystem: AwakeSystem<UIChengjiuComponent>
    {
        public override void Awake(UIChengjiuComponent self)
        {
            self.Awake();
        }
    }

    public class UIChengjiuComponent: Component
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
        private GameObject AlGet;

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
            AlGet = rc.Get<GameObject>("AlGet");
            //返回
            ReturnBtn.onClick.Add(() => { Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChengjiu); });

            //关闭详细页面
            CloseBtn.onClick.Add(() => { Detail.SetActive(false); });

            RequestChengjiuList();
        }

        /// <summary>
        /// 像服务器请求成就列表
        /// </summary>
        private async void RequestChengjiuList()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_Chengjiu g2cChengjiu =
                    (G2C_Chengjiu) await SessionWrapComponent.Instance.Session.Call(new C2G_Chengjiu { Uid = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();
            CreateItems(g2cChengjiu.ChengjiuList);
            CurProgress.text = new StringBuilder().Append("<color=#E8DBAAFF>").Append("已获勋章:").Append("</color>")
                    .Append(GetCompleteChengjiu(g2cChengjiu.ChengjiuList)).Append("/").Append(g2cChengjiu.ChengjiuList.Count + 1).ToString();
        }

        /// <summary>
        /// 创建成就列表
        /// </summary>
        /// <param name="taskInfoList"></param>
        private void CreateItems(List<TaskInfo> taskInfoList)
        {
            GameObject obj = null;
            for (int i = 0; i < taskInfoList.Count; ++i)
            {
                TaskInfo info = taskInfoList[i];
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

        /// <summary>
        /// 设置点击详细信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isGet"></param>
        public void SetDetail(TaskInfo info, bool isGet)
        {
            Detail.SetActive(true);
            ProgressTxt.text = new StringBuilder().Append(info.Progress).Append("/").Append(info.Target).ToString();
            ContentTxt.text = info.Desc;
            NameTxt.text = info.TaskName;
            RewardTxt.text = new StringBuilder().Append("金币").Append(info.Reward).ToString();
            string icon = new StringBuilder().Append("chengjiu_").Append(info.Id).ToString();
            ChengIcon.sprite = CommonUtil.getSpriteByBundle("uichengjiuicon", icon);
            AlGet.SetActive(isGet);
            if (AlGet.gameObject.activeInHierarchy)
                ProgressTxt.text = new StringBuilder().Append(info.Target).Append("/").Append(info.Target).ToString();
        }

        /// <summary>
        /// 获得已完成成就
        /// </summary>
        /// <returns></returns>
        private int GetCompleteChengjiu(List<TaskInfo> chengjiuList)
        {
            int count = 0;
            for (int i = 0; i < chengjiuList.Count; ++i)
            {
                if (chengjiuList[i].IsComplete)
                    ++count;
            }

            return count;
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            itemList.Clear();
            uiList.Clear();
        }
    }
}