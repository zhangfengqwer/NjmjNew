using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIReadyComponentSystem: AwakeSystem<UIReadyComponent>
    {
        public override void Awake(UIReadyComponent self)
        {
            self.Awake();
        }
    }

    public class UIReadyComponent: Component
    {
        private Button changeTableBtn;
        public Button readyBtn;
        private GameObject head;

        private Image readyHead;
        private Text readyName;
        private Text readyText;
        public readonly GameObject[] HeadPanel = new GameObject[4];
        public readonly Dictionary<long, GameObject> userReady = new Dictionary<long, GameObject>();
        private GameObject readyTimeout;
        private Text timeText;
        private CancellationTokenSource tokenSource;
        private int timeOut = 20;
        private Button weChatBtn;
        private Text roomIdText;
        private string roomId;
        private UIRoomComponent uiRoomComponent;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            this.changeTableBtn = rc.Get<GameObject>("ChangeTableBtn").GetComponent<Button>();
            this.readyBtn = rc.Get<GameObject>("ReadyBtn").GetComponent<Button>();
            this.weChatBtn = rc.Get<GameObject>("WeChat").GetComponent<Button>();
            this.roomIdText = rc.Get<GameObject>("RoomId").GetComponent<Text>();

            this.head = rc.Get<GameObject>("Head");
            this.readyTimeout = rc.Get<GameObject>("ReadyTimeout");
            this.timeText = this.readyTimeout.transform.Find("timeText").GetComponent<Text>();

            HeadPanel[0] = head.Get<GameObject>("Bottom");
            HeadPanel[1] = head.Get<GameObject>("Right");
            HeadPanel[2] = head.Get<GameObject>("Top");
            HeadPanel[3] = head.Get<GameObject>("Left");

            this.changeTableBtn.onClick.Add(OnChangeTable);
            this.readyBtn.onClick.Add(OnReady);
            weChatBtn.onClick.Add(OnInviteWeChat);

            timeOut = 20;
            SetTimeOut();

            UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            this.uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
        }

        private void OnInviteWeChat()
        {
            PlatformHelper.WXShareFriends("", "", $"{OtherData.ShareUrl}|南京麻将好友房,房间号:{roomId},局数:{uiRoomComponent.JuCount}" + 
                                                  $"|玩法:南京麻将好友房,房主开房");
        }

        /// <summary>
        /// 设置超时界面
        /// </summary>
        private async void SetTimeOut()
        {
            try
            {
                tokenSource = new CancellationTokenSource();
                while (timeOut > 0)
                {
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000, tokenSource.Token);
                    timeOut--;
                    timeText.text = $"{timeOut}秒";
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private async void OnReady()
        {
            SessionComponent.Instance.Session.Send(new Actor_GamerReady() { Uid = PlayerInfoComponent.Instance.uid });
        }

        private async void OnChangeTable()
        {
            UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            UIRoomComponent roomComponent = uiRoom.GetComponent<UIRoomComponent>();
            Log.Debug("换桌:" + roomComponent.RoomType);
            SessionComponent.Instance.Session.Send(new Actor_ChangeTable(){RoomType = PlayerInfoComponent.Instance.RoomType});
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIReady);
        }

        /// <summary>
        /// 设置准备界面的头像
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="readyPanel"></param>
        /// <param name="index"></param>
        public async void SetPanel(Gamer gamer, int index)
        {
            try
            {
                if (gamer.UserID == PlayerInfoComponent.Instance.uid)
                {
                    if (gamer.IsReady)
                    {
                        readyBtn.interactable = false;
                    }
                    else
                    {
                        readyBtn.interactable = true;
                    }
                }
                  
            }
            catch (Exception e)
            {
                Log.Error(e);

            }
        }

        /// <summary>
        /// 玩家准备
        /// </summary>
        /// <param name="messageUid"></param>
        public void SetReady(long uid)
        {
            if (uid == PlayerInfoComponent.Instance.uid)
            {
                readyBtn.interactable = false;
                readyTimeout.SetActive(false);
            }
        }

        public void ClosePropt()
        {
            readyBtn.gameObject.SetActive(false);
            readyTimeout.gameObject.SetActive(false);
            changeTableBtn.gameObject.SetActive(false);
        }

        public void ShowWeChat(string roomId)
        {
            this.roomId = roomId;
            roomIdText.text = $"房间号：{roomId}";
            weChatBtn.gameObject.SetActive(true);
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            base.Dispose();
            tokenSource.Cancel();
            timeOut = 20;
        }

       
    }
}