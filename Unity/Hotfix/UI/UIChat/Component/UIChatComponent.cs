using System.Collections.Generic;
using System.Net;
using ETModel;
using Hotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChatSystem:AwakeSystem<UIChatComponent>
    {
        public override void Awake(UIChatComponent self)
        {
            self.Awake();
        }
    }

    public class UIChatComponent : Component 
    {
        private Button ExpressionBtn;
        private Button ShortBtn;
        private GameObject ExpressionGrid;
        private GameObject ShortGrid;
        private GameObject ExpressionItem;
        private List<GameObject> ExpressionItemList = new List<GameObject>();
        private GameObject ChatItem;
        private List<GameObject> ChatItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<UI> chatUiList = new List<UI>();
        private GameObject[] chatObjArr = new GameObject[4];

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            chatObjArr[0] = rc.Get<GameObject>("ChatB");
            chatObjArr[1] = rc.Get<GameObject>("ChatL");
            chatObjArr[2] = rc.Get<GameObject>("ChatT");
            chatObjArr[3] = rc.Get<GameObject>("ChatR");

            ExpressionBtn = rc.Get<GameObject>("ExpressionBtn").GetComponent<Button>();
            ShortBtn = rc.Get<GameObject>("ShortBtn").GetComponent<Button>();
            ExpressionGrid = ExpressionBtn.transform.Find("Select_Btn/Scroll/ExpressionGrid").gameObject;
            ShortGrid = ShortBtn.transform.Find("Select_Btn/Scroll/ShortGrid").gameObject;

            ExpressionBtn.onClick.Add(() => { CreatExpressions(); });

            ShortBtn.onClick.Add(() => { CreateChatItems(); });

            ExpressionItem = CommonUtil.getGameObjByBundle(UIType.UIExpression);
            ChatItem = CommonUtil.getGameObjByBundle(UIType.UIChatItem);

            //选中表情包界面
            CreatExpressions();
        }

        private void CreateChatItems()
        {
            ExpressionBtn.transform.GetChild(0).gameObject.SetActive(false);
            ShortBtn.transform.GetChild(0).gameObject.SetActive(true);
            GameObject obj = null;
            for (int i = 0; i < PlayerInfoComponent.Instance.GetChatList().Count; ++i)
            {
                if (i < ChatItemList.Count)
                    obj = ChatItemList[i];
                else
                {
                    obj = GameObject.Instantiate(ChatItem);
                    obj.transform.SetParent(ShortGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    ChatItemList.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIChatItemComponent>();
                    chatUiList.Add(ui);
                }

                //                chatUiList[i].GetComponent<UIChatItemComponent>().SetChatItemInfo(PlayerInfoComponent.Instance.GetChatList()[i], i + 1);
            }
        }

        private void CreatExpressions()
        {
            ExpressionBtn.transform.GetChild(0).gameObject.SetActive(true);
            ShortBtn.transform.GetChild(0).gameObject.SetActive(false);
            GameObject obj = null;
            for (int i = 0; i < 18; ++i)
            {
                if (i < ExpressionItemList.Count)
                    obj = ExpressionItemList[i];
                else
                {
                    obj = GameObject.Instantiate(ExpressionItem);
                    obj.transform.SetParent(ExpressionGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    ExpressionItemList.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIExpressionComponent>();
                    uiList.Add(ui);
                }

                //                uiList[i].GetComponent<UIExpressionComponent>().SetExpression(i + 1);
            }
        }

        public void ShowChatContent(string content, long UId)
        {
            int index = this.GetParent<UI>().GetComponent<GamerComponent>().GetGamerSeat(UId);
            chatObjArr[index].SetActive(true);
            chatObjArr[index].transform.GetChild(0).GetComponent<Text>().text = content;
            StartTimer(index);
        }

        private async void StartTimer(int index)
        {
            int time = 8;
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(300);
                --time;
            }

            chatObjArr[index].SetActive(false);
        }
    }
}
