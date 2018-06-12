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
        private Text ExceptionTxt;
        private Button Mask;
        private List<GameObject> ExpressionItemList = new List<GameObject>();
        private GameObject ChatItem;
        private List<GameObject> ChatItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<UI> chatUiList = new List<UI>();
        public bool isOpen = false;

        public async void Awake()
        {
            await HttpReqUtil.Req(NetConfig.getInstance().getWebUrl() + "files/chat.json", ChatConfig.getInstance().init);
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            ExpressionBtn = rc.Get<GameObject>("ExpressionBtn").GetComponent<Button>();
            ShortBtn = rc.Get<GameObject>("ShortBtn").GetComponent<Button>();
            Mask = rc.Get<GameObject>("Mask").GetComponent<Button>();
            ExceptionTxt = rc.Get<GameObject>("ExceptionTxt").GetComponent<Text>();
            ExpressionGrid = ExpressionBtn.transform.Find("Select_Btn/Scroll/ExpressionGrid").gameObject;
            ShortGrid = ShortBtn.transform.Find("Select_Btn/Scroll/ShortGrid").gameObject;

            ExpressionBtn.onClick.Add(() => { CreatExpressions(); });

            Mask.onClick.Add(() =>
            {
                CloseOrOpenChatUI(false);
            });

            ShortBtn.onClick.Add(() => { CreateChatItems(); });

            ExpressionItem = CommonUtil.getGameObjByBundle(UIType.UIExpression);
            ChatItem = CommonUtil.getGameObjByBundle(UIType.UIChatItem);
            isOpen = false;

            //选中表情包界面
            CreatExpressions();
        }

        private void CreateChatItems()
        {
            ExpressionBtn.transform.GetChild(0).gameObject.SetActive(false);
            ShortBtn.transform.GetChild(0).gameObject.SetActive(true);
            GameObject obj = null;
            for (int i = 0; i < ChatConfig.getInstance().m_chatInfoList.Count; ++i)
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
                chatUiList[i].GetComponent<UIChatItemComponent>().SetChatItemInfo(ChatConfig.getInstance().m_chatInfoList[i]);
            }
        }

        private void CreatExpressions()
        {
            if (GameUtil.isCanUseEmoji())
            {
                ExceptionTxt.text = "表情包到期时间:" + CommonUtil.splitStr_Start_str(PlayerInfoComponent.Instance.GetPlayerInfo().EmogiTime, ' ');
            }
            else
            {
                ExceptionTxt.text = "";
            }

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
                uiList[i].GetComponent<UIExpressionComponent>().SetExpression(i + 1);
            }
        }

        public void CloseOrOpenChatUI(bool isOpen)
        {
            this.isOpen = isOpen;
            GetParent<UI>().GameObject.SetActive(isOpen);
        }

        public override void Dispose()
        {
            base.Dispose();
            ExpressionItemList.Clear();
            ChatItemList.Clear();
            uiList.Clear();
            chatUiList.Clear();
        }
    }
}
