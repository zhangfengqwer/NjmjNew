using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendRoomItemSystem : AwakeSystem<UIFriendRoomItemComponent>
    {
        public override void Awake(UIFriendRoomItemComponent self)
        {
            self.Awake();
        }
    }

    public class UIFriendRoomItemComponent : Component
    {
        private Text RoomIdTxt;
        private Button EnterBtn;

        private TestRoomInfo info;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            RoomIdTxt = rc.Get<GameObject>("RoomIdTxt").GetComponent<Text>();
            EnterBtn =  rc.Get<GameObject>("EnterBtn").GetComponent<Button>();

            EnterBtn.onClick.Add(() =>
            {
                //向服务器发送消息

            });
        }

        public void SetItemInfo(TestRoomInfo info)
        {
            this.info = info;
            RoomIdTxt.text = "房间号：" + info.roomId;
        }
    }
}
