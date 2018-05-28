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
        private GameObject expressionObj;

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
            chatObjArr[index].SetActive(true);
            chatObjArr[index].transform.GetChild(0).GetComponent<Text>().text = content;
            GameUtil.StartTimer(8, chatObjArr[index]);
        }

        /// <summary>
        /// 显示表情动画
        /// </summary>
        /// <param name="name"></param>
        public void ShowExpressAni(string name)
        {
            expressItem = CommonUtil.getGameObjByBundle(name);
            expressionObj = GameObject.Instantiate(expressItem);
            expressionObj.transform.SetParent(GameObject.Find("CommonWorld").transform);
            GameUtil.StartTimer(7, expressionObj,true);
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
