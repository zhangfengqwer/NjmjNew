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
    public class UIDuanwuTreasureSystem : AwakeSystem<UIDuanwuTreasureComponent>
    {
        public override void Awake(UIDuanwuTreasureComponent self)
        {
            self.Awake();
        }
    }

    public class UIDuanwuTreasureComponent : Component
    {
        private Image Icon;
        private Button TreasureBtn;
        private TreasureInfo info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();

            TreasureBtn = rc.Get<GameObject>("TreasureBtn").GetComponent<Button>();
            TreasureBtn.onClick.Add(() =>
            {
               if(Game.Scene.GetComponent<UIComponent>().Get("").GetComponent<UIDuanwuActivityComponent>().duanwuData.ZongziCount >= info.Price)
                {
                    Log.Debug("可领取");
                }
               else
                {
                    ToastScript.createToast("粽子数量不够，请完成活动获取更多粽子");
                }
            });
        }

        private async void BuyTreasure()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_BuyDuanwuTreasure g2cbuytreasure = (G2C_BuyDuanwuTreasure)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_BuyDuanwuTreasure { UId = PlayerInfoComponent.Instance.uid, Price = info.Price, Reward = info.Reward });
            UINetLoadingComponent.closeNetLoading();
            
            //
        }

        /// <summary>
        /// 设置子活动数据显示
        /// </summary>
        /// <param name="info"></param>
        public void SetItemInfo(TreasureInfo info)
        {
            this.info = info;
            string iconName = $"DuanwuTreasure_{info.TreasureId}";
            Icon.sprite = CommonUtil.getSpriteByBundle("Duanwu", iconName);
        }
    }
}
