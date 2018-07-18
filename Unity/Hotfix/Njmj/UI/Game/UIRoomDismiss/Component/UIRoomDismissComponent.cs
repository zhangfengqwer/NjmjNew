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

            SetPlayInfo();
        }
        /// <summary>
        /// 开始时间倒计时
        /// </summary>
        private async void StartTimeDown()
        {
            int time = 60;
            while (time > 0)
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
    }
}