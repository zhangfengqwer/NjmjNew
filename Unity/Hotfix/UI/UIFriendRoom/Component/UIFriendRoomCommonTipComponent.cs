using ETModel;
using Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendRoomCommonTipASystem : AwakeSystem<UIFriendRoomCommonTipComponent,string>
    {
        public override void Awake(UIFriendRoomCommonTipComponent self,string content)
        {
            self.Awake(content);
        }
    }

    [ObjectSystem]
    public class UIFriendRoomCommonTipSystem: StartSystem<UIFriendRoomCommonTipComponent>
    {
        public override void Start(UIFriendRoomCommonTipComponent self)
        {
            self.Start();
        }
    }

    public class UIFriendRoomCommonTipComponent : Component
    {
        private Button CloseBtn;
        private Button SureBtn;
        private Button CancelBtn;
        private Text TipTxt;

        private Action OnOkComplete = null;
        private Action OnCancelComplete = null;

        public void Awake(string content)
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            CloseBtn = rc.Get<GameObject>("CloseBtn").GetComponent<Button>();
            SureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();
            CancelBtn = rc.Get<GameObject>("CancelBtn").GetComponent<Button>();
            TipTxt = rc.Get<GameObject>("TipTxt").GetComponent<Text>();

            TipTxt.text = content;
        }

        public void Start()
        {
            

            CloseBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIFriendRoomCommonTip);
            });

            CancelBtn.onClick.Add(() =>
            {
                if(OnCancelComplete != null)
                {
                    OnCancelComplete();
                }
                else
                {
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIFriendRoomCommonTip);
                }
            });

            SureBtn.onClick.Add(() =>
            {
                if (OnOkComplete != null)
                {
                    OnOkComplete();
                }
                else
                {
                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIFriendRoomCommonTip);
                }
            });
        }

        public void SetSure(Action onComplete = null)
        {
            OnOkComplete = onComplete;
        }

        public void SetCancel(Action onComplete = null)
        {
            OnCancelComplete = onComplete;
        }
    }
}
