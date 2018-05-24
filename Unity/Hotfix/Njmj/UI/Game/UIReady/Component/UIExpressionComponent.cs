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
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom)
                .GetComponent<UIRoomComponent>().CloseChatUI();
                RequestChat();
            });
        }

        public async void RequestChat()
        {
            Debug.Log("222222");
            string aniName = new StringBuilder().Append("Expression_")
                                                .Append(index).ToString();
            G2C_Chat g2cChat = (G2C_Chat)await Game.Scene.GetComponent<SessionWrapComponent>()
                .Session.Call(new C2G_Chat { ChatType = 1,Value = aniName });
            //播放动画
        }

        public void SetExpression(int index)
        {
            this.index = index;
            string expression = new StringBuilder().Append("ExpressionImg_")
                                                   .Append(index).ToString();
            UIExpression.GetComponent<Image>().sprite =
                CommonUtil.getSpriteByBundle("Expression", expression);
        }
    }
}
