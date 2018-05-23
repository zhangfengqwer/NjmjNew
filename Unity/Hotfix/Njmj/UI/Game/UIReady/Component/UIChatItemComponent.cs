using System;
using System.Collections.Generic;
using System.Net;
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
            
        }
    }
    public class UIChatItemComponent : Component
    {
        private Button UIChatItem;
        private Text ChatTxt;

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIChatItem = rc.Get<GameObject>("UIChatItem").GetComponent<Button>();
            ChatTxt = rc.Get<GameObject>("ChatTxt").GetComponent<Text>();
        }

        public void SetChatItemInfo(Chat chat)
        {
            ChatTxt.text = chat.Content;
        }
    }
}
