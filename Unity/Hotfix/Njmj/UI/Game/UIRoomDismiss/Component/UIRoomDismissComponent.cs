using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRoomDismissComponentSystem : AwakeSystem<UIRoomDismissComponent>
    {
        public override void Awake(UIRoomDismissComponent self)
        {
            self.Awake();
        }
    }

    public class UIRoomDismissComponent: Component
    {
        private Text timeText;
        private List<Text> plays;
        private Gamer[] gamers;
        private Button closeBtn;
        private Button cancelBtn;
        private Button sureBtn;
        private int cancelCount;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
           
            //倒计时
            this.timeText = rc.Get<GameObject>("TimeText").GetComponent<Text>();
            StartTimeDown();

            //4个玩家
            UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();
            this.gamers = gamerComponent.GetAll();
            plays = new List<Text>();
            for (int i = 1; i < 5; i++)
            {
                plays.Add(rc.Get<GameObject>("Play" + i).GetComponent<Text>());
            }

            //退出
            this.cancelBtn = rc.Get<GameObject>("CancelBtn").GetComponent<Button>();
            this.sureBtn = rc.Get<GameObject>("SureBtn").GetComponent<Button>();

            cancelBtn.onClick.Add(OnCancel);
            sureBtn.onClick.Add(OnSure);

            cancelCount = 0;
            SetPlayInfo();
        }

        /// <summary>
        /// 同意解散房间
        /// </summary>
        private void OnSure()
        {
            SessionComponent.Instance.Session.Send(new Actor_GamerAgreeRoomDismiss());
        }

        /// <summary>
        /// 不同意解散房间
        /// </summary>
        private void OnCancel()
        {
            SessionComponent.Instance.Session.Send(new Actor_GamerCancelRoomDismiss());
        }

        /// <summary>
        /// 开始时间倒计时
        /// </summary>
        private async void StartTimeDown(int time = 60)
        {
            while (time >= 0)
            {
                timeText.text = $"{time}秒后将自动解散房间";
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                if (IsDisposed)
                {
                    return;
                }
                time--;
            }
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        private void SetPlayInfo()
        {
            for (int i = 0; i < 4; i++)
            {
                plays[i].text = $"【待确定】" + gamers[i].PlayerInfo.Name;
            }
        }

        /// <summary>
        /// 玩家同意解散
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserAgree(long userId)
        {
            int userIndex = this.GetUserIndex(userId);
            if (userIndex != -1)
            {
                plays[userIndex].text = $"【已确定】" + gamers[userIndex].PlayerInfo.Name;
            }

            if (userId == PlayerInfoComponent.Instance.uid)
            {
                cancelBtn.gameObject.SetActive(false);
                sureBtn.gameObject.SetActive(false);
            }
        }
    
        /// <summary>
        /// 玩家不同意解散
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserCancel(long userId)
        {
            int userIndex = this.GetUserIndex(userId);
            if (userIndex != -1)
            {
                plays[userIndex].text = $"【已取消】" + gamers[userIndex].PlayerInfo.Name;
            }

            if (userId == PlayerInfoComponent.Instance.uid)
            {
                cancelBtn.gameObject.SetActive(false);
                sureBtn.gameObject.SetActive(false);
            }

            cancelCount++;

            if (cancelCount == 2)
            {
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRoomDismiss);
            }
        }

        private int GetUserIndex(long userId)
        {
            for (int i = 0; i < gamers.Length; i++)
            {
                if (userId == gamers[i].UserID)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}