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
        private Text Reward;
        private GameObject IsComplete;
        public DuanwuActivity info;
        private bool isClick = false;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            ActBtn = rc.Get<GameObject>("ActBtn").GetComponent<Button>();
            Reward = rc.Get<GameObject>("Reward").GetComponent<Text>();
            IsComplete = rc.Get<GameObject>("IsComplete");

            isClick = false;

            ActBtn.onClick.Add(() =>
            {
                SetSelectState(true);
                try
                {
                    //设置当前选中的活动类型
                    UIDuanwuActivityComponent.Instance.SetCurActivityInfo(info);
                    if (info.IsComplete)
                    {
                        if (!info.IsGet)
                        {
                            //领取奖励
                            GetReward();
                        }
                        else
                        {
                            //已经领取
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            });
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        /// <param name="isSelect"></param>
        public void SetSelectState(bool isSelect)
        {
            if (isSelect)
            {
                ActBtn.GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else
            {
                ActBtn.GetComponent<Image>().color = new Color(0, 0, 0);
            }
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        private async void GetReward()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetDuanwuReward g2cDuanwu = (G2C_GetDuanwuReward)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_GetDuanwuReward { UId = PlayerInfoComponent.Instance.uid, Reward = info.Reward, TaskId = info.TaskId });
            UINetLoadingComponent.closeNetLoading();

            IsComplete.SetActive(false);
            if (g2cDuanwu.Error != ErrorCode.ERR_Success)
            {
                if (!isClick)
                {
                    ToastScript.createToast(g2cDuanwu.Message);
                    isClick = true;
                }
                return;
            }

            UIDuanwuActivityComponent.Instance.DeCount();
            UIDuanwuActivityComponent.Instance.RefreshUI(g2cDuanwu.ZongziCount);
            info.IsGet = g2cDuanwu.IsGet;
            //端午活动一级界面粽子个数旁显示 +XXX粽子
            UIDuanwuActivityComponent.Instance.ShowAddZongziCount(info.Reward);
        }

        /// <summary>  
        /// 设置子活动数据显示
        /// </summary>
        /// <param name="info"></param>
        public void SetItemInfo(DuanwuActivity info)
        {
            this.info = info;
            IsComplete.SetActive(false);
            Reward.text = info.Reward.ToString();
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
            if(info.TaskId <= 104)
            {
                Icon.sprite = CommonUtil.getSpriteByBundle("Image_Duanwu", "104");
            }
            else if(info.TaskId <= 108)
            {
                Icon.sprite = CommonUtil.getSpriteByBundle("Image_Duanwu", "108");
            }
            else
            {
                Icon.sprite = CommonUtil.getSpriteByBundle("Image_Duanwu", "112");
            }
        }
    }
}
