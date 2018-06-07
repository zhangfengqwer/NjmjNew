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
    public class UIDuanwuActivityItemSystem : AwakeSystem<UIDuanwuActivityItemComponent>
    {
        public override void Awake(UIDuanwuActivityItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIDuanwuActivityItemComponent : Component
    {
        private Image Icon;
        private Button ActBtn;
        private GameObject IsComplete;
        private DuanwuActivity info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            ActBtn = rc.Get<GameObject>("ActBtn").GetComponent<Button>();
            ActBtn.onClick.Add(() =>
            {
                //设置当前选中的活动类型
                Game.Scene.GetComponent<UIComponent>().Get("").GetComponent<UIDuanwuActivityComponent>().SetCurActivityInfo(info);
                if (!info.IsGet)
                {
                    //领取奖励
                    GetReward();
                }
            });
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        private async void GetReward()
        {
            UINetLoadingComponent.showNetLoading();
            ////type = 3 领取粽子
            //G2C_DuanwuDataBase g2cDuanwu = (G2C_DuanwuDataBase)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_DuanwuDataBase { UId = PlayerInfoComponent.Instance.uid, Type = 3, ZongZiCount = info.Reward });
            //更新任务
            UINetLoadingComponent.closeNetLoading();

            //端午活动一级界面粽子个数旁显示 +XXX粽子
            Game.Scene.GetComponent<UIComponent>().Get("").GetComponent<UIDuanwuActivityComponent>().ShowAddZongziCount(info.Reward);
        }

        /// <summary>
        /// 设置子活动数据显示
        /// </summary>
        /// <param name="info"></param>
        public void SetItemInfo(DuanwuActivity info)
        {
            this.info = info;
            if (info.IsComplete)
            {
                if (info.IsGet)
                {
                    IsComplete.SetActive(false);
                }
                else
                {
                    IsComplete.SetActive(true);
                }
            }
            string iconName = $"Duanwu_{info.TaskId}";
            Icon.sprite = CommonUtil.getSpriteByBundle("Duanwu", iconName);
        }
    }
}
