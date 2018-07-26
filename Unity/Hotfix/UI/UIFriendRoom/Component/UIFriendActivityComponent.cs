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

        private int count = 5;

        public void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

           /* CloseBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();*/
            GetBtn = rc.Get<GameObject>("GetBtn").GetComponent<Button>();
            ShowText = rc.Get<GameObject>("ShowText").GetComponent<Text>();

            //CloseBtn.onClick.Add(() =>
            //{
            //    this.GetParent<UI>().GameObject.SetActive(false);
            //});

            GetBtn.onClick.Add(() =>
            {
                GetTreasure();
            });

            //GetFriendActInfo();
        }

        private async void GetTreasure()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetFriendTreasure g2cFrd = (G2C_GetFriendTreasure)await SessionComponent.Instance.Session.Call(new C2G_GetFriendTreasure() { UId = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

            if(g2cFrd.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cFrd.Message);
                return;
            }

            GetFriendActInfo();
        }

        private async void GetFriendActInfo()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_FriendActInfo g2cFrd = (G2C_FriendActInfo)await SessionComponent.Instance.Session.Call(new C2G_FriendActInfo() { UId = PlayerInfoComponent.Instance.uid });
            UINetLoadingComponent.closeNetLoading();

            ShowText.text = g2cFrd.ConsumCount + "/" + count; 

            if(g2cFrd.GetCount >= 5)
            {
                GetBtn.enabled = false;
            }
            else
            {
                GetBtn.enabled = true;
            }
        }
    }
}
