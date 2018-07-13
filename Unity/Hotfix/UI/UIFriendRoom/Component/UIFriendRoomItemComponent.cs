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
        private Text juTxt;
        private Text huaTxt;
        private GameObject FriendGrid;

        private TestRoomInfo info;
        private List<GameObject> icons = new List<GameObject>();
        private GameObject icon = null;
        private List<UI> uis = null;
        private const int iconCount = 4;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            RoomIdTxt = rc.Get<GameObject>("RoomIdTxt").GetComponent<Text>();
            EnterBtn =  rc.Get<GameObject>("EnterBtn").GetComponent<Button>();
            juTxt = rc.Get<GameObject>("juTxt").GetComponent<Text>();
            huaTxt = rc.Get<GameObject>("huaTxt").GetComponent<Text>();
            FriendGrid = rc.Get<GameObject>("FriendGrid");

            icon = CommonUtil.getGameObjByBundle(UIType.UIFriendIcon);

            EnterBtn.onClick.Add(() =>
            {
                //向服务器发送消息

            });
        }

        public void SetItemInfo(TestRoomInfo info)
        {
            this.info = info;
            RoomIdTxt.text = "房间号：" + info.roomId;
            juTxt.text = info.ju + "局";
            huaTxt.text = "每花" + info.hua;
            CreateItems(info.icons);
        }

        private void CreateItems(List<string> iconNames)
        {
            GameObject obj = null;
            for(int i = 0;i< iconNames.Count; ++i)
            {
                if(i < icons.Count)
                {
                    obj = icons[i];
                }
                else
                {
                    obj = GameObject.Instantiate(icon,FriendGrid.transform);
                    icons.Add(obj);
                }
                HeadManager.setHeadSprite(obj.GetComponent<Image>(), iconNames[i]);
            }

            for(int i = iconNames.Count;i < iconCount; ++i)
            {
                if(i < icons.Count)
                {
                    obj = icons[i];
                }
                else
                {
                    obj = obj = GameObject.Instantiate(icon, FriendGrid.transform);
                    icons.Add(obj);
                }
                HeadManager.setHeadSprite(obj.GetComponent<Image>(), "None");
            }
        }
    }
}
