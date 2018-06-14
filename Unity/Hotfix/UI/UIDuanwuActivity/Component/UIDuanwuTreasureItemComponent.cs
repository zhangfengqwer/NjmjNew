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
    public class UIDuanwuTreasureItemSystem : AwakeSystem<UIDuanwuTreasureItemComponent>
    {
        public override void Awake(UIDuanwuTreasureItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIDuanwuTreasureItemComponent : Component
    {
        private Image Icon;
        private Text Limit;
        public TreasureInfo info;
        private int buyCount;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            Limit = rc.Get<GameObject>("Limit").GetComponent<Text>();

            Icon.GetComponent<Button>().onClick.Add(() =>
            {
                if(buyCount == info.LimitCount)
                {
                    ToastScript.createToast("购买次数已达上限");
                    return;
                }
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIDuanwuTreasure).GetComponent<UIDuanwuTreasureComponent>().SetReward(this,info);
            });
        }

        public void RefreshUI(DuanwuTreasureLogInfo info)
        {
            Limit.text = $"已购<color=#810000FF>{info.buyCount}</color>次";
        }

        /// <summary>
        /// 设置子活动数据显示
        /// </summary>
        /// <param name="info"></param>
        public void SetItemInfo(TreasureInfo info,int buyCount)
        {
            try
            {
                this.info = info;
                this.buyCount = buyCount;
                Limit.text = $"<color=#810000FF>{buyCount}</color>/{info.LimitCount}";

                string iconName = "";
                if (info.TreasureId <= 3)
                {
                    iconName = "3";
                }
                else if (info.TreasureId <= 6)
                {
                    iconName = "6";
                }
                else
                {
                    iconName = "9";
                }

                Icon.sprite = CommonUtil.getSpriteByBundle("Image_Duanwu", iconName);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
