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
        private Text accountTxt;
        private Text nameTxt;
        private Text uid;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            returnBtn = rc.Get<GameObject>("ReturnBtn").GetComponent<Button>();
            accountTxt = rc.Get<GameObject>("AccountTxt").GetComponent<Text>();
            nameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            uid = rc.Get<GameObject>("UID").GetComponent<Text>();

            returnBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIMain).GetComponent<UIMainComponent>().SetUIHideOrOpen(true);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIPlayerInfo);
            });

            PlayerInfoComponent pc = Game.Scene.GetComponent<PlayerInfoComponent>();
            PlayerInfo playerInfo = pc.GetPlayerInfo();
            nameTxt.text = playerInfo.Name;
            accountTxt.text = playerInfo.Name;
            uid.text = pc.uid.ToString();
        }
    }
}
