using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIExpressSystem : AwakeSystem<UIExpressionComponent>
    {
        public override void Awake(UIExpressionComponent self)
        {
            self.Awake();
        }
    }
    public class UIExpressionComponent : Component
    {
        private GameObject UIExpression;
        private int index;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIExpression = rc.Get<GameObject>("UIExpression");
            UIExpression.GetComponent<Button>().onClick.Add(() =>
            {
                RequestChat();
            });
        }

        private async void RequestChat()
        {
            string aniStr = new StringBuilder().Append("Expression_")
                                             .Append(index).ToString();
            G2C_Chat g2cChat = (G2C_Chat)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(new C2G_Chat { UId = PlayerInfoComponent.Instance.uid,ChatType = 1, Value = aniStr });
        }

        public void SetExpression(int index)
        {
            this.index = index;
            string icon = new StringBuilder().Append("ExpressionImg_")
                                             .Append(index).ToString();
            UIExpression.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("expression", icon);
        }
    }
}
