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
        private TreasureInfo info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            Limit = rc.Get<GameObject>("Limit").GetComponent<Text>();

            Icon.GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIDuanwuTreasure).GetComponent<UIDuanwuTreasureComponent>().SetReward(info);
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
                Limit.text = $"已购<color=#810000FF>{buyCount}</color>次";

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
