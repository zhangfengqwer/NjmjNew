using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIReadyComponentSystem: AwakeSystem<UIReadyComponent>
    {
        public override void Awake(UIReadyComponent self)
        {
            self.Awake();
        }
    }

    public class UIReadyComponent: Component
    {
        private Button changeTableBtn;
        public Button readyBtn;
        private GameObject head;

        private Image readyHead;
        private Text readyName;
        private Text readyText;

        public readonly GameObject[] HeadPanel = new GameObject[4];

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            this.changeTableBtn = rc.Get<GameObject>("ChangeTableBtn").GetComponent<Button>();
            this.readyBtn = rc.Get<GameObject>("ReadyBtn").GetComponent<Button>();

            this.head = rc.Get<GameObject>("Head");

            HeadPanel[0] = head.Get<GameObject>("Bottom");
            HeadPanel[1] = head.Get<GameObject>("Right");
            HeadPanel[2] = head.Get<GameObject>("Top");
            HeadPanel[3] = head.Get<GameObject>("Left");

            this.changeTableBtn.onClick.Add(OnChangeTable);
            this.readyBtn.onClick.Add(OnReady);
        }

        private async void OnReady()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerReady() { Uid = PlayerInfoComponent.Instance.uid });
        }

        private async void OnChangeTable()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_ChangeTable());
        }

        /// <summary>
        /// 设置准备界面的头像
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="readyPanel"></param>
        /// <param name="index"></param>
        public void SetPanel(int index)
        {
            GameObject gameObject = this.HeadPanel[index];
            //绑定关联

            this.readyHead = gameObject.Get<GameObject>("Image").GetComponent<Image>();
            this.readyName = gameObject.Get<GameObject>("Name").GetComponent<Text>();
            this.readyText = gameObject.Get<GameObject>("Text").GetComponent<Text>();

            UpdatePanel();
        }

        /// <summary>
        /// 设置头像，姓名
        /// </summary>
        private async void UpdatePanel()
        {
            G2C_PlayerInfo playerInfo =
                    (G2C_PlayerInfo) await SessionWrapComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = this.GetParent<Gamer>().UserID });
            readyName.text = this.GetParent<Gamer>().UserID + "";
            readyHead.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(playerInfo.PlayerInfo.Icon);
        }

        /// <summary>
        /// 玩家准备
        /// </summary>
        /// <param name="messageUid"></param>
        public void SetReady(long uid)
        {
            readyText.text = "已准备";

            if (uid == PlayerInfoComponent.Instance.uid)
            {
                readyBtn.interactable = false;
            }
        }

        public void ResetPanel()
        {
            readyName.text = "";
            readyHead.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "icon_default");
            readyText.text = "";
        }
    }
}