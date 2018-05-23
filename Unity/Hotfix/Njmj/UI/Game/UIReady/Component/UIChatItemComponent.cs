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
    public class UIChatItemSystem : AwakeSystem<UIChatItemComponent>
    {
        public override void Awake(UIChatItemComponent self)
        {
            self.Awake();
        }
    }
    public class UIChatItemComponent : Component
    {
        private Button UIChatItem;
        private Text ChatTxt;
        private int index;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIChatItem = rc.Get<GameObject>("UIChatItem").GetComponent<Button>();
            ChatTxt = rc.Get<GameObject>("ChatTxt").GetComponent<Text>();
            UIChatItem.GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom)
               .GetComponent<UIRoomComponent>().CloseChatUI();
                RequestChat();
            });
        }

        private async void RequestChat()
        {
            string aniName = new StringBuilder().Append("Expression_")
                                                .Append(index).ToString();
            G2C_Chat g2cChat = (G2C_Chat)await Game.Scene.GetComponent<SessionWrapComponent>()
                .Session.Call(new C2G_Chat { ChatType = 2, Value = ChatTxt.text, UId = PlayerInfoComponent.Instance.uid });
        }

        public void SetChatItemInfo(Chat chat,int index)
        {
            this.index = index;
            ChatTxt.text = chat.Content;
        }
    }
}
