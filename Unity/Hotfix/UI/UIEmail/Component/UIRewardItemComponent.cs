using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRewardItemSystem : AwakeSystem<UIRewardItemComponent>
    {
        public override void Awake(UIRewardItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIRewardItemComponent : Component
    {
        private Image rewardItem;
        private Text itemNum;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            rewardItem = rc.Get<GameObject>("UIRewardItem").GetComponent<Image>();
            itemNum = rc.Get<GameObject>("ItemNum").GetComponent<Text>();
        }

        public void SetRewardInfo(string spriteName,string num)
        {
            rewardItem.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(spriteName);
            itemNum.text = num;
        }
    }
}
