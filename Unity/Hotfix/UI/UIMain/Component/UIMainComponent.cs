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
                UpdateInfoTest();
            });

            exchangeBtn.onClick.Add(() =>
            {
                //打开兑换界面
                Log.Debug("打开兑换界面");
            });

            activeBtn.onClick.Add(() =>
            {
                //打开活动界面
                Log.Debug("打开活动界面");
            });

            shopBtn.onClick.Add(() =>
            {
                //打开商城
                Log.Debug("打开商城界面");
                //ShopConfig unitConfig = (ShopConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(ShopConfig), 1);
                //Debug.Log(JsonHelper.ToJson(unitConfig));
                if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>() != null)
                    Game.Scene.GetComponent<UIComponent>().Get(UIType.UIShop).GetComponent<UIShopComponent>().SetUIHideOrOpen(true);
                else
                    Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
            });

            taskBtn.onClick.Add(() =>
            {
                //打开任务面板
                Log.Debug("打开任务界面");
            });

            awardBtn.onClick.Add(() =>
            {
                //打开领奖界面
                Log.Debug("打开领奖界面");

                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIHelp);
            });

            enterRoomBtn.onClick.Add(OnEnterRoom);

            playerIcon.GetComponent<Button>().onClick.Add(() =>
            {
                //打开用户基本信息界面
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                SetUIHideOrOpen(false);
            });

            #endregion

            #region set playerInfo 

            //向服务器发送消息请求玩家信息，然后设置玩家基本信息
            SetPlayerInfo();

            #endregion
        }

        private async void UpdateInfoTest()
        {
            PlayerInfoComponent playerInfoComponent = Game.Scene.GetComponent<PlayerInfoComponent>();
            long uid = playerInfoComponent.uid;
            playerInfoComponent.GetPlayerInfo().GoldNum += 100;
            playerInfoComponent.GetPlayerInfo().Name = "张";
            playerInfoComponent.GetPlayerInfo().Icon = "Icon2";
            G2C_UpdatePlayerInfo g2cUpdatePlayerInfo = (G2C_UpdatePlayerInfo)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdatePlayerInfo() { Uid = uid, playerInfo = playerInfoComponent.GetPlayerInfo() });
            UpDatePlayerInfo(g2cUpdatePlayerInfo.playerInfo);
        }

        private async void OnEnterRoom()
        {
            Game.Scene.GetComponent<SessionWrapComponent>().Session.Send(new C2G_EnterRoom());
        }

        private async void SetPlayerInfo()
        {
            PlayerInfoComponent playerInfoComponent = Game.Scene.GetComponent<PlayerInfoComponent>();
            long uid = playerInfoComponent.uid;
            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionWrapComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = uid });
            Log.Info(JsonHelper.ToJson(g2CPlayerInfo));
            PlayerInfo info = g2CPlayerInfo.PlayerInfo;
            playerInfoComponent.SetPlayerInfo(info);
            UpDatePlayerInfo(info);
        }

        private void UpDatePlayerInfo(PlayerInfo info)
        {
            Sprite icon = Game.Scene.GetComponent<UIIconComponent>().GetSprite(info.Icon);
            if (icon != null)
                playerIcon.sprite = icon;
            else
                Log.Warning("icon数据为空，请重新注册");
            playerNameTxt.text = info.Name;
            goldNumTxt.text = info.GoldNum.ToString();
            wingNumTxt.text = info.WingNum.ToString();
        }

        public void SetUIHideOrOpen(bool isHide)
        {
            GetParent<UI>().GameObject.SetActive(isHide);
        }
    }
}