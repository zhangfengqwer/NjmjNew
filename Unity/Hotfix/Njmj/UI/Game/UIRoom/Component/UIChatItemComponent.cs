﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ETModel;
using Hotfix;
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

        public void Awake()
        {
            ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            UIChatItem = rc.Get<GameObject>("UIChatItem").GetComponent<Button>();
            ChatTxt = rc.Get<GameObject>("ChatTxt").GetComponent<Text>();

            UIChatItem.GetComponent<Button>().onClick.Add(() =>
            {
                RequestChat();
            });
            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);
        }

        private void RequestChat()
        {
            //UINetLoadingComponent.showNetLoading();
            Game.Scene.GetComponent<SessionComponent>()
                .Session.Send(new Actor_Chat { ChatType = 2, Value = ChatTxt.text, UId = PlayerInfoComponent.Instance.uid });
            //UINetLoadingComponent.closeNetLoading();
        }

        public void SetChatItemInfo(ChatInfo chat)
        {
            ChatTxt.text = chat.content;
        }
    }
}
