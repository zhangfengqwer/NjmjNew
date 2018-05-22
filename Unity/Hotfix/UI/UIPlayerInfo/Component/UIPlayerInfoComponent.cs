using ETModel;
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

        private Button returnBtn;
        private Button playerIcon;
        private Button changeNameBtn;
        private Button realNameBtn;
        private Button bindPhoneBtn;
        private Button ChangeAccountBtn;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            uIDTxt = rc.Get<GameObject>("UIDTxt").GetComponent<Text>();
            realNameTxt = rc.Get<GameObject>("RealNameTxt").GetComponent<Text>();
            noBindPhoneTxt = rc.Get<GameObject>("NoBindPhoneTxt").GetComponent<Text>();

            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            playerIcon = rc.Get<GameObject>("PlayerIcon").GetComponent<Button>();
            changeNameBtn = rc.Get<GameObject>("ChangeNameBtn").GetComponent<Button>();
            realNameBtn = rc.Get<GameObject>("RealNameBtn").GetComponent<Button>();
            bindPhoneBtn = rc.Get<GameObject>("BindPhoneBtn").GetComponent<Button>();
            ChangeAccountBtn = rc.Get<GameObject>("ChangeAccountBtn").GetComponent<Button>();


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
            });
            
            changeNameBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIChangeName);
            });

            ChangeAccountBtn.onClick.Add(() =>
            {
                onClickChangeAccount();
            });

            PlayerInfoComponent pc = Game.Scene.GetComponent<PlayerInfoComponent>();
            PlayerInfo playerInfo = pc.GetPlayerInfo();
            nameTxt.text = playerInfo.Name;
            uIDTxt.text = pc.uid.ToString();
            if (playerInfo.IsRealName)
                realNameTxt.text = "已实名";
            if (!string.IsNullOrEmpty(playerInfo.Phone))
                noBindPhoneTxt.text = "已绑定";
            playerIcon.GetComponent<Image>().sprite = Game.Scene.GetComponent<UIIconComponent>()
                .GetSprite(PlayerInfoComponent.Instance.GetPlayerInfo().Icon);

            playerIcon.onClick.Add(() =>
            {
                CommonUtil.ShowUI(UIType.UIIcon);
            });

            Init();
        }

        private void Init()
        {
            bindPhoneBtn.gameObject.SetActive(!string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone));
            changeNameBtn.gameObject.SetActive(PlayerInfoComponent.Instance.GetPlayerInfo().RestChangeNameCount >= 0);
            realNameBtn.gameObject.SetActive(!PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName);

        }

        public void Update()
        {
            playerIcon.GetComponent<Image>().sprite = Game.Scene.GetComponent<UIIconComponent>()
                .GetSprite(PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
            Debug.Log(PlayerInfoComponent.Instance.GetPlayerInfo().Name);
            nameTxt.text = PlayerInfoComponent.Instance.GetPlayerInfo().Name;
            if (PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName)
                realNameTxt.text = "已实名";
            if (!string.IsNullOrEmpty(PlayerInfoComponent.Instance.GetPlayerInfo().Phone))
                noBindPhoneTxt.text = "已绑定";
        }

        public async void onClickChangeAccount()
        {
            SessionWrap sessionWrap = null;
            try
            {
                IPEndPoint connetEndPoint = NetworkHelper.ToIPEndPoint(GlobalConfigComponent.Instance.GlobalProto.Address);

                Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(connetEndPoint);
                sessionWrap = new SessionWrap(session);
                R2C_ChangeAccount r2CData = (R2C_ChangeAccount)await sessionWrap.Call(new C2R_ChangeAccount() { Uid = PlayerInfoComponent.Instance.uid });
                sessionWrap.Dispose();

                Game.Scene.GetComponent<UIComponent>().RemoveAll();
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UILogin);
            }
            catch (Exception e)
            {
                sessionWrap?.Dispose();
                Log.Error(e);
            }
        }
    }
}
