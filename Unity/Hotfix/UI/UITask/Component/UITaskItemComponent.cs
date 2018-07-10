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
                GetReward();
            });
        }

        private async void UpdatePlayerInfoData()
        {

            UINetLoadingComponent.showNetLoading();
            G2C_UpdatePlayerInfo g2c =(G2C_UpdatePlayerInfo) await SessionComponent.Instance.Session
                .Call(new C2G_UpdatePlayerInfo
                {
                    Uid = PlayerInfoComponent.Instance.uid,
                    playerInfo = PlayerInfoComponent.Instance.GetPlayerInfo()
                });
            UINetLoadingComponent.closeNetLoading();
        }

        private async void GetReward()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetTaskReward g2cGetItem = (G2C_GetTaskReward)await SessionComponent.Instance.Session.Call(new C2G_GetTaskReward { UId = PlayerInfoComponent.Instance.uid, TaskInfo = taskProgress, GetType = 1 });
            UINetLoadingComponent.closeNetLoading();

            GameUtil.changeData(1, taskProgress.Reward);
            if(g2cGetItem.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cGetItem.Message);
            }
            else
            {
                RefreshUI(true);
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UITask).GetComponent<UITaskComponent>().DeCount();
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain)
                    .GetComponent<UIMainComponent>().refreshUI();
            }
        }

        private void RefreshUI(bool isGet)
        {
            completeTxt.SetActive(isGet);
            getBtn.gameObject.SetActive(!isGet);
        }

        public void SetTaskItemInfo(TaskInfo info)
        {
            //Debug.Log(JsonHelper.ToJson(info));
            taskProgress = info;
            string iconName = new StringBuilder().Append("Task_")
                                                 .Append(info.Id).ToString();
            taskNameTxt.text = info.TaskName;
            descTxt.text = info.Desc;
            taskIcon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(iconName);
            rewardTxt.text = new StringBuilder().Append("金币")
                                                .Append(info.Reward).ToString();
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
                    GetParent<UI>().GameObject.transform.SetAsFirstSibling();
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
