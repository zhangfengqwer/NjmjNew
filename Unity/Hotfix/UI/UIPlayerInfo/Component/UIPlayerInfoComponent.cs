using ETModel;
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
        private Button returnBtn;
        private Button playerIcon;
        private Text nameTxt;
        private Text uIDTxt;
        private Button changeNameBtn;
        private Button realNameBtn;
        private Button bindPhoneBtn;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            uIDTxt = rc.Get<GameObject>("UIDTxt").GetComponent<Text>();
            playerIcon = rc.Get<GameObject>("PlayerIcon").GetComponent<Button>();
            changeNameBtn = rc.Get<GameObject>("ChangeNameBtn").GetComponent<Button>();
            realNameBtn = rc.Get<GameObject>("RealNameBtn").GetComponent<Button>();
            bindPhoneBtn = rc.Get<GameObject>("BindPhoneBtn").GetComponent<Button>();

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

            PlayerInfoComponent pc = Game.Scene.GetComponent<PlayerInfoComponent>();
            PlayerInfo playerInfo = pc.GetPlayerInfo();
            nameTxt.text = playerInfo.Name;
            uIDTxt.text = pc.uid.ToString();
            playerIcon.GetComponent<Image>().sprite = Game.Scene.GetComponent<UIIconComponent>()
                .GetSprite(PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
            playerIcon.onClick.Add(() =>
            {
                CommonUtil.ShowUI(UIType.UIIcon);
            });
        }

        public void UpdateIcon()
        {
            playerIcon.GetComponent<Image>().sprite = Game.Scene.GetComponent<UIIconComponent>()
                .GetSprite(PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
        }
    }
}
