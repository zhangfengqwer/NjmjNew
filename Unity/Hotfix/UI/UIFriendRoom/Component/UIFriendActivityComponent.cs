using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendActivitySystem : StartSystem<UIFriendActivityComponent>
    {
        public override void Start(UIFriendActivityComponent self)
        {
            self.Start();
        }
    }

    public class UIFriendActivityComponent : Component
    {
        private Button CloseBtn;
        private Button GetBtn;
        private Text ShowText;
        private Text LeftTxt;
        private Text TimeTxt;

        private int count = 5;
        private int MaxCount = 5;

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

           /* CloseBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();*/
            GetBtn = rc.Get<GameObject>("GetBtn").GetComponent<Button>();
            ShowText = rc.Get<GameObject>("ShowText").GetComponent<Text>();
            LeftTxt  = rc.Get<GameObject>("LeftTxt").GetComponent<Text>();
            TimeTxt  = rc.Get<GameObject>("TimeTxt").GetComponent<Text>();

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);
            //CloseBtn.onClick.Add(() =>
            //{
            //    this.GetParent<UI>().GameObject.SetActive(false);
            //});

            GetBtn.onClick.Add(() =>
            {
                GetBtn.enabled = false;
                GetTreasure();
            });

            GetFriendActInfo();
        }

        private async void GetTreasure()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetFriendTreasure g2cFrd = (G2C_GetFriendTreasure)await SessionComponent.Instance.Session.Call(new C2G_GetFriendTreasure() { UId = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

            if (g2cFrd.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cFrd.Message);
                SetButtonEnable();
                return;
            }

            GameUtil.changeDataWithStr(g2cFrd.Reward);
            ShowRewardUtil.Show(g2cFrd.Reward);
            DeCount();
            GetFriendActInfo();

            SetButtonEnable();
        }

        private async void SetButtonEnable()
        {
            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(2000);
            if (GetBtn != null)
            {
                GetBtn.enabled = true;
            }
        }

        int notGetcount = 0;
        private int GetNoGetCount(G2C_FriendActInfo info)
        {
            if (info.ConsumCount >= 5 && info.GetCount < 5)
            {
                if (((int)(info.ConsumCount / 5)) >= (5 - info.GetCount))
                {
                    notGetcount = 5 - info.GetCount;
                }
                else
                {
                    notGetcount = (int)(info.ConsumCount / 5);
                }
            }
            else
            {
                notGetcount = 0;
            }
            return notGetcount;
        }

        public void DeCount()
        {
            --notGetcount;
            if (notGetcount <= 0)
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(3, false);
            }
            else
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(3, true, notGetcount);
            }
        }

        private async void GetFriendActInfo()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_FriendActInfo g2cFrd = (G2C_FriendActInfo)await SessionComponent.Instance.Session.Call(new C2G_FriendActInfo() { UId = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

            notGetcount = 0;
            GetNoGetCount(g2cFrd);

            if (notGetcount != 0)
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(3, true, notGetcount);
            }
            else
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetRedTip(3, false);
            }

            ShowText.text = g2cFrd.ConsumCount.ToString() + "/" + count;
            LeftTxt.text = "剩余次数" + "<Color=#F5E724FF>" + (MaxCount - g2cFrd.GetCount) + "</Color>" + "次";
            if (g2cFrd.GetCount >= 5)
            {
                GetBtn.enabled = false;
                GetBtn.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                GetBtn.enabled = true;
                GetBtn.GetComponent<Image>().color = Color.white;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            notGetcount = 0;
        }
    }
}
