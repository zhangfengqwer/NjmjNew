using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponentSystem : AwakeSystem<UIMainComponent>
    {
        public override void Awake(UIMainComponent self)
        {
            self.Awake();
        }
    }

    public class UIMainComponent : Component
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

        private Image playerIcon;

        public void Awake()
        {
            #region get
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            playerNameTxt = rc.Get<Text>("PlayerNameTxt");
            goldNumTxt = rc.Get<Text>("GoldNumTxt");
            wingNumTxt = rc.Get<Text>("WingNumTxt");
            playerIcon = rc.Get<Image>("PlayerIcon");

            rankBtn = rc.Get<GameObject>("RankBtn").GetComponent<Button>();
            exchangeBtn = rc.Get<GameObject>("ExchangeBtn").GetComponent<Button>();
            activeBtn = rc.Get<GameObject>("ActiveBtn").GetComponent<Button>();
            shopBtn = rc.Get<GameObject>("ShopBtn").GetComponent<Button>();
            taskBtn = rc.Get<GameObject>("TaskBtn").GetComponent<Button>();
            awardBtn = rc.Get<GameObject>("AwardBtn").GetComponent<Button>();
            #endregion

            #region buttonClick
            rankBtn.onClick.Add(() =>
            {
                //打开排行榜
                Log.Debug("打开排行榜");
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
            });
            #endregion

            #region set playerInfo 
            //向服务器发送消息请求玩家信息，然后设置玩家基本信息
            SetPlayerInfo();
            #endregion
        }

        private void SetPlayerInfo()
        {

        }

    }
}
