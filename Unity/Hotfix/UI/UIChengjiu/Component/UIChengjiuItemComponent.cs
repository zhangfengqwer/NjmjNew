using System.Text;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChengjiuItemSystem : AwakeSystem<UIChengjiuItemComponent>
    {
        public override void Awake(UIChengjiuItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIChengjiuItemComponent : Component
    {
        private GameObject ChengjiuItemBtn;
        private TaskInfo info;
        private GameObject Complete;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            ChengjiuItemBtn = rc.Get<GameObject>("ChengjiuItemBtn");
            Complete = rc.Get<GameObject>("Complete");

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);

            ChengjiuItemBtn.GetComponent<Button>().onClick.Add(() =>
            {
                if (info.IsComplete)
                {
                    if (!info.IsGet)
                    {
                        //未领取
                        info.IsGet = true;
                        GetReward();
                    }
                    else
                    {
                        //显示提示框
                        Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu)
                        .GetComponent<UIChengjiuComponent>().SetDetail(info, true);
                    }
                }
                else
                {
                    //显示提示框
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu)
                    .GetComponent<UIChengjiuComponent>().SetDetail(info, false);
                }
            });
        }

        private async void GetReward()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetTaskReward g2cGet = (G2C_GetTaskReward)await SessionComponent.Instance.Session.Call(new C2G_GetTaskReward { UId = PlayerInfoComponent.Instance.uid, TaskInfo = info, GetType = 2});
            UINetLoadingComponent.closeNetLoading();

            if(g2cGet.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cGet.Message);
            }
            else
            {
                GameUtil.changeData(1, info.Reward);
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain)
                .GetComponent<UIMainComponent>().refreshUI();

                string str = "1:" + info.Reward;
                ShowRewardUtil.Show(str);
                string icon = new StringBuilder().Append("chengjiu_")
                                                 .Append(info.Id).ToString();
                Debug.Log("已经领取成就奖励");
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIChengjiu).GetComponent<UIChengjiuComponent>().DeCount();
                Complete.SetActive(false);
                ChengjiuItemBtn.GetComponent<Image>().sprite =
                CommonUtil.getSpriteByBundle("uichengjiuicon", icon);
            }
        }

        public void SetInfo(TaskInfo info)
        {
            this.info = info;
            string icon = new StringBuilder().Append("chengjiu1_")
                                             .Append(info.Id).ToString();
            if(info.IsComplete)
            {
                //未领取
                if (!info.IsGet)
                {
                    Complete.SetActive(true);
                }
                else
                {
                    icon = new StringBuilder().Append("chengjiu_")
                                                 .Append(info.Id).ToString();
                    Complete.SetActive(false);
                }
            }
            ChengjiuItemBtn.GetComponent<Image>().sprite =
                CommonUtil.getSpriteByBundle("uichengjiuicon", icon);
        }
    }
}
