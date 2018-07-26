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

        private FriendRoomInfo info;
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

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);

            icon = CommonUtil.getGameObjByBundle(UIType.UIFriendIcon);

            EnterBtn.onClick.Add(() =>
            {
                //向服务器发送消息
                if (info.IsPublic == 1)
                {
                    //公开房间
                    JoinRoom();
                }
                else if (info.IsPublic == 2)
                {
                    //私密房间
                    Game.Scene.GetComponent<UIComponent>().Create(UIType.UIJoinRoom);
                }
            });
        }

        private async void JoinRoom()
        {
            await UIJoinRoomComponent.EnterFriendRoom(info.RoomId.ToString());
        }

        public void SetItemInfo(FriendRoomInfo info)
        {
            this.info = info;
            if(info.IsPublic == 1)
            {
                RoomIdTxt.text = "房号:" + info.RoomId;
            }
            else if(info.IsPublic == 2)
            {
                RoomIdTxt.text = "私密房间";
            }
            juTxt.text = info.Ju + "局";
            huaTxt.text = info.Hua + "/花";
            if (info.Icons.Count >= 4)
            {
                EnterBtn.gameObject.SetActive(false);
            }
            else
            {
                EnterBtn.gameObject.SetActive(true);
            }
            CreateItems(info.Icons);
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
