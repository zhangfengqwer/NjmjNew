using ETModel;
using Hotfix;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIPlayerInfoComponentSystem: AwakeSystem<UIPlayerInfoComponent>
    {
        public override void Awake(UIPlayerInfoComponent self)
        {
            self.Awake();
        }
    }

    public class UIPlayerInfoComponent : Component
    {
        private Text nameTxt;
        private Text uIDTxt;
        private Text noBindPhoneTxt;
        private Text realNameTxt;
        private Text AlPlayTxt;
        private Text SuccessRateTxt;
        private Text MaxSuccessTxt;

        private Button returnBtn;
        private Button playerIcon;
        private Button changeNameBtn;
        private Button realNameBtn;
        private Button bindPhoneBtn;
        private Button ChangeAccountBtn;
        private Button De;
        private Text GoldNumTxt;
        private Text WingNumTxt;
        private Text HuafeiNumTxt;
        private Button AddBtn;
        private Button DuihuanBtn;
        private GameObject PlayerFrame;
        private Text VipTimeTxt;

        public async void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            uIDTxt = rc.Get<GameObject>("UIDTxt").GetComponent<Text>();
            realNameTxt = rc.Get<GameObject>("RealNameTxt").GetComponent<Text>();
            noBindPhoneTxt = rc.Get<GameObject>("NoBindPhoneTxt").GetComponent<Text>();
            HuafeiNumTxt = rc.Get<GameObject>("HuafeiNumTxt").GetComponent<Text>();
            AlPlayTxt = rc.Get<GameObject>("AlPlayTxt").GetComponent<Text>();
            SuccessRateTxt = rc.Get<GameObject>("SuccessRateTxt").GetComponent<Text>();
            MaxSuccessTxt = rc.Get<GameObject>("MaxSuccessTxt").GetComponent<Text>();
            VipTimeTxt = rc.Get<GameObject>("VipTimeTxt").GetComponent<Text>();

            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            playerIcon = rc.Get<GameObject>("PlayerIcon").GetComponent<Button>();
            changeNameBtn = rc.Get<GameObject>("ChangeNameBtn").GetComponent<Button>();
            realNameBtn = rc.Get<GameObject>("RealNameBtn").GetComponent<Button>();
            bindPhoneBtn = rc.Get<GameObject>("BindPhoneBtn").GetComponent<Button>();
            ChangeAccountBtn = rc.Get<GameObject>("ChangeAccountBtn").GetComponent<Button>();
            DuihuanBtn = rc.Get<GameObject>("DuihuanBtn").GetComponent<Button>();

            De = rc.Get<GameObject>("De").GetComponent<Button>();
            GoldNumTxt = rc.Get<GameObject>("GoldNumTxt").GetComponent<Text>();
            WingNumTxt = rc.Get<GameObject>("WingNumTxt").GetComponent<Text>();
            AddBtn = rc.Get<GameObject>("AddBtn").GetComponent<Button>();
            DuihuanBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseHuaFei);
            });

            AddBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIPlayerInfo);
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
            });

            PlayerFrame = rc.Get<GameObject>("PlayerFrame");

            bindPhoneBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIBindPhone);
            });

            realNameBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIRealName);
            });

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetUIHideOrOpen(true);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIPlayerInfo);
                if(Game.Scene.GetComponent<UIComponent>().Get(UIType.UIVIP) != null)
                {
                    if (Game.Scene.GetComponent<UIComponent>().Get(UIType.UIVIP).GameObject.activeInHierarchy)
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIVIP);
                }
            });
            
            changeNameBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIChangeName);
            });

            ChangeAccountBtn.onClick.Add(() =>
            {
                onClickChangeAccount();
            });

            De.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIVIP);
            });

            PlayerInfoComponent pc = Game.Scene.GetComponent<PlayerInfoComponent>();
            PlayerInfo playerInfo = pc.GetPlayerInfo();
            nameTxt.text = playerInfo.Name;
            uIDTxt.text = pc.uid.ToString();
            AlPlayTxt.text = $"已玩牌局：{ playerInfo.TotalGameCount}";
            string winRate = GameUtil.GetWinRate(playerInfo.TotalGameCount, playerInfo.WinGameCount).ToString();
            if (winRate.Equals(0))
                SuccessRateTxt.text = $"胜       率：{0}%";
            else
                SuccessRateTxt.text = $"胜       率：{winRate}%";
            MaxSuccessTxt.text = $"最大赢取：{playerInfo.MaxHua}";

            if (playerInfo.IsRealName)
                realNameTxt.text = "已实名";
            if (!string.IsNullOrEmpty(playerInfo.Phone))
                noBindPhoneTxt.text = "已绑定";

            // 设置头像
            {
                HeadManager.setHeadSprite(playerIcon.GetComponent<Image>(), PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
            }

            playerIcon.onClick.Add(() =>
            {
                CommonUtil.ShowUI(UIType.UIIcon);
            });

            Init();
        }

        private void Init()
        {
            if (GameUtil.isVIP())
            {
                VipTimeTxt.text = $"过期时间:{CommonUtil.splitStr_Start_str(PlayerInfoComponent.Instance.GetPlayerInfo().VipTime, ' ')}";
            }
            else
            {
                VipTimeTxt.text = "";
            }
            if (OtherData.getIsShiedBindPhone())
            {
                bindPhoneBtn.transform.localScale = Vector3.zero;
            }
            if (OtherData.getIsShiedRealName())
            {
                realNameBtn.transform.localScale = Vector3.zero;
            }
            bindPhoneBtn.gameObject.SetActive(string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone) && !OtherData.getIsShiedBindPhone());
            bindPhoneBtn.transform.parent.gameObject.SetActive(string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone) && !OtherData.getIsShiedBindPhone());
            changeNameBtn.gameObject.SetActive(PlayerInfoComponent.Instance.GetPlayerInfo().RestChangeNameCount > 0);
            changeNameBtn.transform.parent.gameObject.SetActive(PlayerInfoComponent.Instance.GetPlayerInfo().RestChangeNameCount > 0);
            realNameBtn.gameObject.SetActive(!PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName && !OtherData.getIsShiedRealName());
            realNameBtn.transform.parent.gameObject.SetActive(!PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName && !OtherData.getIsShiedRealName());
            realNameTxt.gameObject.SetActive(!OtherData.getIsShiedRealName());
            noBindPhoneTxt.gameObject.SetActive(!OtherData.getIsShiedBindPhone());
            GoldNumTxt.text = PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum.ToString();
            WingNumTxt.text = PlayerInfoComponent.Instance.GetPlayerInfo().WingNum.ToString();
            HuafeiNumTxt.text = (PlayerInfoComponent.Instance.GetPlayerInfo().HuaFeiNum / 100.0f).ToString();
            if (GameUtil.isVIP())
            {
                PlayerFrame.transform.Find("HeadKuang").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", "touxiangkuang_vip");
            }
        }

        public void Update()
        {
            HeadManager.setHeadSprite(playerIcon.GetComponent<Image>(), PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
            nameTxt.text = PlayerInfoComponent.Instance.GetPlayerInfo().Name;

            if(PlayerInfoComponent.Instance.GetPlayerInfo().RestChangeNameCount <= 0)
            {
                changeNameBtn.gameObject.SetActive(false);
                changeNameBtn.transform.parent.gameObject.SetActive(false);
            }

            if (GameUtil.isVIP())
            {
                VipTimeTxt.text = $"过期时间:{CommonUtil.splitStr_Start_str(PlayerInfoComponent.Instance.GetPlayerInfo().VipTime, ' ')}";
            }
            else
            {
                VipTimeTxt.text = "";
            }

            changeNameBtn.gameObject.SetActive(PlayerInfoComponent.Instance.GetPlayerInfo().RestChangeNameCount > 0);
            changeNameBtn.transform.parent.gameObject.SetActive(PlayerInfoComponent.Instance.GetPlayerInfo().RestChangeNameCount > 0);
            realNameBtn.gameObject.SetActive(!PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName && !OtherData.getIsShiedRealName());
            realNameBtn.transform.parent.gameObject.SetActive(!PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName && !OtherData.getIsShiedRealName());
            realNameTxt.gameObject.SetActive(!OtherData.getIsShiedRealName());
            noBindPhoneTxt.gameObject.SetActive(!OtherData.getIsShiedBindPhone());

            if (PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName)
            {
                realNameTxt.text = "已实名";
                realNameBtn.gameObject.SetActive(false);
                realNameBtn.transform.parent.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone))
            {
                noBindPhoneTxt.text = "已绑定";
                bindPhoneBtn.gameObject.SetActive(false);
                bindPhoneBtn.transform.parent.gameObject.SetActive(false);
            }
            GoldNumTxt.text = PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum.ToString();
        }

        public static async void onClickChangeAccount()
        {
            Session sessionWrap = null;
            try
            {
                //IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

                IPEndPoint connetEndPoint = NetConfig.getInstance().ToIPEndPointWithYuMing();
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = ComponentFactory.Create<Session, ETModel.Session>(session);
                R2C_ChangeAccount r2CData = (R2C_ChangeAccount)await SessionComponent.Instance.Session.Call(new C2R_ChangeAccount() { Uid = PlayerInfoComponent.Instance.uid });
                sessionWrap.Dispose();

                PlayerPrefs.SetString("Phone", "");
                PlayerPrefs.SetString("Token", "");

                Game.Scene.GetComponent<UIComponent>().RemoveAll();
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
                //给android发送切换账号回调
                PlatformHelper.setLogoutCallback("", "", "");

                HeartBeat.getInstance().stopHeartBeat();
            }
            catch (Exception e)
            {
                PlayerPrefs.SetString("Phone", "");
                PlayerPrefs.SetString("Token", "");

                Game.Scene.GetComponent<UIComponent>().RemoveAll();
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);

                HeartBeat.getInstance().stopHeartBeat();

                sessionWrap?.Dispose();
                Log.Error(e);
            }
        }
    }
}
