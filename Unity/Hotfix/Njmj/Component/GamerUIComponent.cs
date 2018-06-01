using System;
using System.Threading.Tasks;
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
        private Text shengLvText;
        private Text jinbiText;
        private Text uidText;
        private GameObject headInfo;

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
//            ResetPrompt();
        
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

            if (index != 0)
            {
                this.headInfo = this.head.transform.GetChild(0).gameObject;
                this.shengLvText = this.headInfo.Get<GameObject>("Shenglv").GetComponent<Text>();
                this.jinbiText = this.headInfo.Get<GameObject>("Jinbi").GetComponent<Text>();
                this.uidText = this.headInfo.Get<GameObject>("Uid").GetComponent<Text>();
                this.headInfo.SetActive(false);
                head.GetComponent<Button>().onClick.Add(OnShowHeadInfo);
            }

//            this.readyHead = readyPanel.Get<GameObject>("Image").GetComponent<Image>();
//            this.readyName = readyPanel.Get<GameObject>("Name").GetComponent<Text>();
//            this.readyText = readyPanel.Get<GameObject>("Text").GetComponent<Text>();

            UpdatePanel();
        }

        private void OnShowHeadInfo()
        {
            if (this.headInfo.activeSelf)
            {
                headInfo.SetActive(false);
            }
            else
            {
                headInfo.SetActive(true);
            }
        }

        public void ResetReadyPanel()
        {
            if (readyName == null) return;
            readyName.text = "";
            readyHead.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "icon_default");
            readyText.text = "";

        }

        /// <summary>
        /// 更新面板
        /// </summary>
        public void UpdatePanel()
        {
            if (this.Panel != null)
            {
                SetUserInfo();
            }
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        public void GameStart()
        {
//            ResetPrompt();
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="id"></param>
        private async void SetUserInfo()
        {
            PlayerInfo playerInfo = this.GetParent<Gamer>().PlayerInfo;

            if (this.Panel != null || playerInfo == null)
            {
                name.text = playerInfo.Name;
                head.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(playerInfo.Icon);

                if (Index != 0)
                {
                    uidText.text = playerInfo.Name;
                    jinbiText.text = $"金币:<color=#FFF089FF>{playerInfo.GoldNum}</color>";

                    int i;
                    if (playerInfo.TotalGameCount == 0)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = playerInfo.WinGameCount / playerInfo.TotalGameCount;
                    }
                    shengLvText.text = $"胜率:<color=#FFF089FF>{i}%</color>";
                }

            }
        }

        /// <summary>
        /// 设置准备界面
        /// </summary>
        /// <param name="gameObject"></param>
        public async Task SetHeadPanel(GameObject gameObject)
        {
            Gamer gamer = this.GetParent<Gamer>();
            this.readyHead = gameObject.Get<GameObject>("Image").GetComponent<Image>();
            this.readyName = gameObject.Get<GameObject>("Name").GetComponent<Text>();
            this.readyText = gameObject.Get<GameObject>("Text").GetComponent<Text>();

            G2C_PlayerInfo playerInfo = (G2C_PlayerInfo) await SessionWrapComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = gamer.UserID });
            gamer.PlayerInfo = playerInfo.PlayerInfo;
            readyName.text = playerInfo.PlayerInfo.Name + "";

            HeadManager.setHeadSprite(readyHead, playerInfo.PlayerInfo.Icon);
//            readyHead.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(playerInfo.PlayerInfo.Icon);

            if (gamer.IsReady)
            {
            
                gameObject.transform.Find("Text").GetComponent<Text>().text = "已准备";
//                gameObject.Get<GameObject>("Text").GetComponent<Text>().text = "已准备";
            }
            else
            {
                readyText.text = "";
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

        public void SetReady()
        {
            readyText.text = "已准备";
        }
    }
}
