using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class GamerUIComponentStartSystem : StartSystem<GamerUIComponent>
    {
        public override void Start(GamerUIComponent self)
        {
            self.Start();
        }
    }

    /// <summary>
    /// 玩家UI组件
    /// </summary>
    public class GamerUIComponent : Component
    {
        //UI面板
        public GameObject Panel { get; private set; }

        //玩家昵称
        public string NickName { get { return name.text; } }

        private Image head;
        private Text prompt;
        private Text name;

        private Image readyHead;
        private Text readyName;
        private Text readyText;

        public int Index { get; set; }

        public void Start()
        {
//           
        }

        /// <summary>
        /// 重置面板
        /// </summary>
        public void ResetPanel()
        {
            ResetPrompt();
        
            this.name.text = "空位";

            this.Panel = null;
            this.name = null;
        }

        /// <summary>
        /// 设置面板
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="gameObject"></param>
        /// <param name="index"></param>
        public void SetPanel(GameObject panel, GameObject readyPanel, int index)
        {
            panel.SetActive(true);
            this.Panel = panel;
            this.Index = index;
            //绑定关联
            this.head = this.Panel.Get<GameObject>("Head").GetComponent<Image>();
            this.name = this.Panel.Get<GameObject>("Name").GetComponent<Text>();
            this.prompt = this.Panel.Get<GameObject>("Prompt").GetComponent<Text>();

            this.readyHead = readyPanel.Get<GameObject>("Image").GetComponent<Image>();
            this.readyName = readyPanel.Get<GameObject>("Name").GetComponent<Text>();
            this.readyText = readyPanel.Get<GameObject>("Text").GetComponent<Text>();

            UpdatePanel();
        }


        /// <summary>
        /// 更新面板
        /// </summary>
        public void UpdatePanel()
        {
            if (this.Panel != null)
            {
                ResetPrompt();
                SetUserInfo();
            }
        }

        /// <summary>
        /// 重置提示
        /// </summary>
        public void ResetPrompt()
        {
            prompt.text = "";
            readyText.text = "";
            if (this.GetParent<Gamer>().UserID == PlayerInfoComponent.Instance.uid)
            {
                UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                UIRoomComponent uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                uiRoomComponent.readyBtn.interactable = true;
            }
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        public void GameStart()
        {
            ResetPrompt();
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="id"></param>
        private async void SetUserInfo()
        {
            G2C_PlayerInfo playerInfo = (G2C_PlayerInfo)await SessionWrapComponent.Instance.Session.Call(
                new C2G_PlayerInfo() { uid = this.GetParent<Gamer>().UserID });

            if (this.Panel != null)
            {
                name.text = this.GetParent<Gamer>().UserID + "";
                readyName.text = this.GetParent<Gamer>().UserID + "";
                readyHead.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(playerInfo.PlayerInfo.Icon);
                head.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(playerInfo.PlayerInfo.Icon);
                this.GetParent<Gamer>().Head = playerInfo.PlayerInfo.Icon;
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

        }

    }
}
