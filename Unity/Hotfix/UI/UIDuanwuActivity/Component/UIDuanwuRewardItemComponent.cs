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
    public class UIDuanwuRewardItemSystem : AwakeSystem<UIDuanwuRewardItemComponent>
    {
        public override void Awake(UIDuanwuRewardItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIDuanwuRewardItemComponent : Component
    {
        private Image Icon;
        private Text Reward;
        private RewardStruct info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            Reward = rc.Get<GameObject>("Reward").GetComponent<Text>();
        }

        /// <summary>
        /// 设置奖励显示
        /// </summary>
        /// <param name="info"></param>
        public void SetItemInfo(RewardStruct info)
        {
            try
            {
                this.info = info;
                Sprite sprite = CommonUtil.getSpriteByBundle("Image_Shop", info.iconName);
                if(sprite == null)
                {
                    sprite = CommonUtil.getSpriteByBundle("PlayerIcon", info.iconName);
                }
                Icon.sprite = sprite;
                Reward.text = "x" + info.rewardNum;
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
