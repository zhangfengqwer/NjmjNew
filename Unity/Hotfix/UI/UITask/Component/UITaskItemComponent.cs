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
        }

        public void SetTaskItemInfo(TaskInfo info,int curProgress)
        {
            string iconName = new StringBuilder().Append("Task")
                                                 .Append(info.Id).ToString();
            taskNameTxt.text = info.TaskName;
            descTxt.text = info.Desc;
            taskIcon.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(iconName);
            rewardTxt.text = new StringBuilder().Append("金币")
                                                .Append(info.Reward).ToString();

            targetTxt.text = new StringBuilder().Append(curProgress)
                                                .Append("/")
                                                .Append(info.Target).ToString();
        }
    }
}
