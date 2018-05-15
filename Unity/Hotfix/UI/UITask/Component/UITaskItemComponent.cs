using ETModel;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITaskItemSystem : AwakeSystem<UITaskItemComponent>
    {
        public override void Awake(UITaskItemComponent self)
        {
            self.Awake();
        }
    }

    public class UITaskItemComponent : Component
    {
        private GameObject completeTxt;
        private Text targetTxt;
        public Text rewardTxt;
        private GameObject goingTxt;
        private Text descTxt;
        private Text taskNameTxt;
        private Image taskIcon;
        private Button getBtn;
        private TaskInfo taskProgress;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            completeTxt = rc.Get<GameObject>("CompleteTxt");
            targetTxt = rc.Get<GameObject>("TargetTxt").GetComponent<Text>();
            goingTxt = rc.Get<GameObject>("GoingTxt");
            descTxt = rc.Get<GameObject>("DescTxt").GetComponent<Text>();
            rewardTxt = rc.Get<GameObject>("RewardTxt").GetComponent<Text>();
            taskIcon = rc.Get<GameObject>("TaskIcon").GetComponent<Image>();
            taskNameTxt = rc.Get<GameObject>("TaskNameTxt").GetComponent<Text>();
            getBtn = rc.Get<GameObject>("GetBtn").GetComponent<Button>();
            getBtn.onClick.Add(() =>
            {
                Debug.Log("已领取");
                taskProgress.IsGet = true;
                GetReward();
            });
        }

        private async void GetReward()
        {
            G2C_UpdateTaskProgress g2cTask = (G2C_UpdateTaskProgress)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdateTaskProgress { UId = PlayerInfoComponent.Instance.uid, TaskPrg = taskProgress });
            RefreshUI(g2cTask);
        }

        private void RefreshUI(G2C_UpdateTaskProgress g2cTask)
        {
            completeTxt.SetActive(g2cTask.TaskPrg.IsGet);
            getBtn.gameObject.SetActive(!g2cTask.TaskPrg.IsGet);
        }

        public void SetTaskItemInfo(TaskInfo info)
        {
            taskProgress = info;
            string iconName = new StringBuilder().Append("Task_")
                                                 .Append(info.Id).ToString();
            taskNameTxt.text = info.TaskName;
            descTxt.text = info.Desc;
            taskIcon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(iconName);
            rewardTxt.text = new StringBuilder().Append("金币")
                                                .Append(info.Reward).ToString();
            Debug.Log(JsonHelper.ToJson(taskProgress));
            if(taskProgress.IsComplete)
            {
                goingTxt.SetActive(false);
                if (taskProgress.IsGet)
                {
                    completeTxt.SetActive(true);
                    getBtn.gameObject.SetActive(false);
                }
                else
                {
                    completeTxt.SetActive(false);
                    getBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                goingTxt.SetActive(true);
                completeTxt.SetActive(false);
                getBtn.gameObject.SetActive(false);
                targetTxt.text = new StringBuilder().Append(info.Progress)
                                                .Append("/")
                                                .Append(info.Target).ToString();
            }
        }

    }
}
