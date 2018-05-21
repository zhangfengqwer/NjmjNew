using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponentSystem: AwakeSystem<UIMainComponent>
    {
        public override void Awake(UIMainComponent self)
        {
            self.Awake();
        }
    }

    public class UIMainComponent: Component
    {
        private Text playerNameTxt;
        private Text goldNumTxt;
        private Text wingNumTxt;

        private Button rankBtn;
        private Button exchangeBtn;
        private Button activeBtn;
        private Button shopBtn;
        private Button taskBtn;
        private Button awardBtn;
        private Button enterRoomBtn;

        private Image playerIcon;

        public void Awake()
        {
            #region get

            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            playerNameTxt = rc.Get<GameObject>("PlayerNameTxt").GetComponent<Text>();
            goldNumTxt = rc.Get<GameObject>("GoldNumTxt").GetComponent<Text>();
            wingNumTxt = rc.Get<GameObject>("WingNumTxt").GetComponent<Text>();
            playerIcon = rc.Get<GameObject>("PlayerIcon").GetComponent<Image>();

            rankBtn = rc.Get<GameObject>("RankBtn").GetComponent<Button>();
            exchangeBtn = rc.Get<GameObject>("ExchangeBtn").GetComponent<Button>();
            activeBtn = rc.Get<GameObject>("ActiveBtn").GetComponent<Button>();
            shopBtn = rc.Get<GameObject>("ShopBtn").GetComponent<Button>();
            taskBtn = rc.Get<GameObject>("TaskBtn").GetComponent<Button>();
            awardBtn = rc.Get<GameObject>("AwardBtn").GetComponent<Button>();
            enterRoomBtn = rc.Get<GameObject>("EnterRoomBtn").GetComponent<Button>();

            #endregion

            #region buttonClick

            rankBtn.onClick.Add(() =>
            {
                //打开排行榜
                Log.Debug("打开排行榜");
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIBag);
            });

            exchangeBtn.onClick.Add(() =>
            {
                //打开兑换界面
                Log.Debug("打开兑换界面");
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIEmail);
            });

            activeBtn.onClick.Add(() =>
            {
                //打开活动界面
                Log.Debug("打开活动界面");

                UI ui = Game.Scene.GetComponent<UIComponent>().Create(UIType.UIGameResult);
                GameResultNeedData data = new GameResultNeedData();
                data.isZiMo = true;
                ui.GetComponent<UIGameResultComponent>().setData(data);
            });

            shopBtn.onClick.Add(() =>
            {
                //打开商城
                Log.Debug("打开商城界面");
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
            });

            taskBtn.onClick.Add(() =>
            {
                //打开任务面板
                Log.Debug("打开任务界面");
                RequestTaskInfo();
            });

            awardBtn.onClick.Add(() =>
            {
                //打开领奖界面
                Log.Debug("打开领奖界面");

                // Game.Scene.GetComponent<UIComponent>().Create(UIType.UIHelp);

                // Game.Scene.GetComponent<UIComponent>().Create(UIType.UIDaily);

                RequestRealName();
            });

            enterRoomBtn.onClick.Add(OnEnterRoom);

            playerIcon.GetComponent<Button>().onClick.Add(() =>
            {
                //test 添加元宝
                /*UpDatePlayerInfo();*/
                //打开用户基本信息界面
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                SetUIHideOrOpen(false);
            });

            #endregion

            #region set Info 

            //向服务器发送消息请求玩家信息，然后设置玩家基本信息
            SetPlayerInfo();
            GetRankInfo();
            #endregion

            CommonUtil.ShowUI(UIType.UIDaily);
        }

		public async void GetRankInfo()
        {
            G2C_Rank g2cRank = (G2C_Rank)await Game.Scene.GetComponent<SessionWrapComponent>()
                .Session.Call(new C2G_Rank { });
            //设置排行榜信息
            Debug.Log(JsonHelper.ToJson(g2cRank.rankList));
        }

		 private async void RequestRealName()
        {
            G2C_RealName g2cRealName = (G2C_RealName)await SessionWrapComponent.Instance.Session.Call(new C2G_RealName { Uid = PlayerInfoComponent.Instance.uid,Name = "黄品", IDNumber = "320724199310256015" });

            if (g2cRealName.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cRealName.Message);
                return;
            }
            else
            {
                ToastScript.createToast("实名认证成功");
            }
        }
        private async void RequestTaskInfo()
        {
            G2C_Task g2cTask = (G2C_Task)await SessionWrapComponent.Instance.Session.Call(new C2G_Task { uid = PlayerInfoComponent.Instance.uid });
            PlayerInfoComponent.Instance.SetTaskInfoList(g2cTask.TaskProgressList);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UITask);
        }

        //public async void UpDatePlayerInfo()
        //{
        //    PlayerInfoComponent playerInfoComponent = Game.Scene.GetComponent<PlayerInfoComponent>();
        //    long uid = playerInfoComponent.uid;
        //    playerInfoComponent.GetPlayerInfo().WingNum = 1000;
        //    G2C_UpdatePlayerInfo g2cUpdatePlayerInfo = (G2C_UpdatePlayerInfo)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdatePlayerInfo() { Uid = uid, playerInfo = playerInfoComponent.GetPlayerInfo() });
        //    UpDatePlayerInfo(g2cUpdatePlayerInfo.playerInfo);
        //}

        private async void OnEnterRoom()
        {
            G2C_EnterRoom enterRoom = (G2C_EnterRoom)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(
                new C2G_EnterRoom());

          

        }

        private async void SetPlayerInfo()
        {
            long uid = PlayerInfoComponent.Instance.uid;
            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionWrapComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = uid });
            if (g2CPlayerInfo == null)
            {
                Debug.Log("用户信息错误");
                return;
            }
            PlayerInfoComponent.Instance.SetPlayerInfo(g2CPlayerInfo.PlayerInfo);
            refreshUI();
        }

        public void SetUIHideOrOpen(bool isHide)
        {
            GetParent<UI>().GameObject.SetActive(isHide);
        }

        public void refreshUI()
        {
            PlayerInfo info = PlayerInfoComponent.Instance.GetPlayerInfo();
            Sprite icon = Game.Scene.GetComponent<UIIconComponent>().GetSprite(info.Icon);
            if (icon != null)
                playerIcon.sprite = icon;
            else
                Log.Warning("icon数据为空，请重新注册");
            playerNameTxt.text = info.Name;
            goldNumTxt.text = info.GoldNum.ToString();
            wingNumTxt.text = info.WingNum.ToString();
        }
    }
}