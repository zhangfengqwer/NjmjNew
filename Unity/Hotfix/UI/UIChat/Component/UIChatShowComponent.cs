using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChatShowSystem : AwakeSystem<UIChatShowComponent>
    {
        public override void Awake(UIChatShowComponent self)
        {
            self.Awake();
        }
    }

    public class UIChatShowComponent : Component
    {
        private GameObject[] chatObjArr = new GameObject[4];
        private GameObject expressItem;//表情
        private GameObject expressionObj = null;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            chatObjArr[0] = rc.Get<GameObject>("ChatB");
            chatObjArr[1] = rc.Get<GameObject>("ChatR");
            chatObjArr[2] = rc.Get<GameObject>("ChatT");
            chatObjArr[3] = rc.Get<GameObject>("ChatL");
            GameUtil.isExit = false;
        }

        /// <summary>
        /// 显示相应的聊天内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="UId"></param>
        public void ShowChatContent(string content, long UId)
        {
            UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            GamerComponent gamerComponent = ui.GetComponent<GamerComponent>();
            int index = gamerComponent.GetGamerSeat(UId);
            //如果多次点击发送文本，隐掉前一个显示当前的
            if (chatObjArr[index].activeInHierarchy)
            {
                Log.Debug("已经存在item");
                isExit = true;
                time = -1;
                chatObjArr[index].SetActive(false);
            }
            isExit = false;
            chatObjArr[index].SetActive(true);
            chatObjArr[index].transform.GetChild(0).GetComponent<Text>().text = content;
            time = 8;
            StartTimer(chatObjArr[index]);
        }

        /// <summary>
        /// 显示表情动画
        /// </summary>
        /// <param name="name"></param>
        public void ShowExpressAni(string name)
        {
            try
            {
                //如果连续发表情 删除上一个，显示当前的表情
                if (expressionObj != null)
                {
                    isExcExit = true;
                    exctime = -1;
                    GameObject.DestroyObject(expressionObj);
                }

                expressItem = CommonUtil.getGameObjByBundle(name);
                expressionObj = GameObject.Instantiate(expressItem);
                expressionObj.transform.SetParent(GameObject.Find("CommonWorld").transform);
                isExcExit = false;
                exctime = 7;
                StartExcTimer();
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }

        }

        /// <summary>
        /// 文本聊天显示定时器
        /// </summary>
        public bool isExit = false;
        public int time = 8;
        public async void StartTimer(GameObject obj)
        {
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(300);
                --time;
            }
            if (isExit)
                return;
            obj.SetActive(false);
        }

        /// <summary>
        /// 表情显示计时器
        /// </summary>
        public static bool isExcExit = false;
        public static int exctime = 7;
        public async void StartExcTimer()
        {
            while (exctime >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(300);
                --exctime;
            }
            if (isExcExit)
                return;
            GameObject.DestroyObject(expressionObj);
        }

        public override void Dispose()
        {
            base.Dispose();
            GameUtil.isExit = true;
            if (expressionObj != null)
                GameObject.DestroyObject(expressionObj);
        }
    }
}
