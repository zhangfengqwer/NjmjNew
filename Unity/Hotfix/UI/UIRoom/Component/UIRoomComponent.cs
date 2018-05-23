using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRoomComponentSystem: AwakeSystem<UIRoomComponent>
    {
        public override void Awake(UIRoomComponent self)
        {
            self.Awake();
        }
    }

    public class UIRoomComponent: Component
    {
        public readonly GameObject[] GamersPanel = new GameObject[4];
        public readonly GameObject[] HeadPanel = new GameObject[4];

        private Button changeTableBtn;
        private Button exitBtn;
        public Button readyBtn;
        private Image timeImage;

        private GameObject desk;
        //控制时间
        private GameObject head;
        private Button giveUpBtn;
        private Button huBtn;
        private Button gangBtn;
        private Button pengBtn;

        private Button ChatBtn;//聊天按钮
        private GameObject Chat;//聊天框
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
        public GameObject currentItem = new GameObject();
        private GameObject[] chatObjArr = new GameObject[4];

        private Text restText;
        private GameObject players;
        private CancellationTokenSource tokenSource;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            GamersPanel[0] = rc.Get<GameObject>("Bottom");
            GamersPanel[1] = rc.Get<GameObject>("Right");
            GamersPanel[2] = rc.Get<GameObject>("Top");
            GamersPanel[3] = rc.Get<GameObject>("Left");
            this.players = rc.Get<GameObject>("Players");

            this.desk = rc.Get<GameObject>("Desk");
            this.head = rc.Get<GameObject>("Head");

            HeadPanel[0] = head.Get<GameObject>("Bottom");
            HeadPanel[1] = head.Get<GameObject>("Right");
            HeadPanel[2] = head.Get<GameObject>("Top");
            HeadPanel[3] = head.Get<GameObject>("Left");

            ChatBtn = rc.Get<GameObject>("ChatBtn").GetComponent<Button>();
            Chat = rc.Get<GameObject>("Chat");
            ExpressionBtn = Chat.transform.Find("ExpressionBtn").GetComponent<Button>();
            ShortBtn = Chat.transform.Find("ShortBtn").GetComponent<Button>();
            ExpressionGrid = ExpressionBtn.transform.Find("Select_Btn/Scroll/ExpressionGrid").gameObject;
            ShortGrid = ShortBtn.transform.Find("Select_Btn/Scroll/ShortGrid").gameObject;

            chatObjArr[0] = rc.Get<GameObject>("ChatB");
            chatObjArr[1] = rc.Get<GameObject>("ChatL");
            chatObjArr[2] = rc.Get<GameObject>("ChatB");
            chatObjArr[3] = rc.Get<GameObject>("ChatR");

            this.restText = rc.Get<GameObject>("RestText").GetComponent<Text>();

            this.changeTableBtn = rc.Get<GameObject>("ChangeTableBtn").GetComponent<Button>();
            this.readyBtn = rc.Get<GameObject>("ReadyBtn").GetComponent<Button>();
            this.exitBtn = rc.Get<GameObject>("ExitBtn").GetComponent<Button>();
            this.timeImage = rc.Get<GameObject>("Time").GetComponent<Image>();

            this.giveUpBtn = rc.Get<GameObject>("ImageGiveUp").GetComponent<Button>();
            this.huBtn = rc.Get<GameObject>("ImageHu").GetComponent<Button>();
            this.gangBtn = rc.Get<GameObject>("ImageGang").GetComponent<Button>();
            this.pengBtn = rc.Get<GameObject>("ImagePeng").GetComponent<Button>();

            pengBtn.onClick.Add(() => OnOperate(0));
            gangBtn.onClick.Add(() => OnOperate(1));
            huBtn.onClick.Add(() => OnOperate(2));
            giveUpBtn.onClick.Add(() => OnOperate(3));
            ExpressionItem = CommonUtil.getGameObjByBundle(UIType.UIExpression);
            ChatItem = CommonUtil.getGameObjByBundle(UIType.UIChatItem);
            this.changeTableBtn.onClick.Add(OnChangeTable);
            this.exitBtn.onClick.Add(OnExit);
            this.readyBtn.onClick.Add(OnReady);
            ChatBtn.onClick.Add(() =>
            {
                Chat.gameObject.SetActive(true);
                //选中表情包界面
                CreatExpressions();
            });

            ExpressionBtn.onClick.Add(() =>
            {
                CreatExpressions();
            });

            ShortBtn.onClick.Add(() =>
            {
                CreateChatItems();
            });
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
                chatUiList[i].GetComponent<UIChatItemComponent>().SetChatItemInfo(PlayerInfoComponent.Instance.GetChatList()[i], i + 1);
            }
        }

        private void CreatExpressions()
        {
            ExpressionBtn.transform.GetChild(0).gameObject.SetActive(true);
            ShortBtn.transform.GetChild(0).gameObject.SetActive(false);
            GameObject obj = null;
            for(int i = 0;i< 18; ++i)
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

        public void ShowChatContent(string content,long UId)
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

        public void CloseChatUI()
        {
            Chat.SetActive(false);
        }

        private async void OnReady()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerReady() { Uid = PlayerInfoComponent.Instance.uid });
        }

        private async void OnExit()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerExitRoom() { IsFromClient = true });
        }

        private async void OnChangeTable()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_ChangeTable());
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="gamer"></param>
        /// <param name="index"></param>
        public void AddGamer(Gamer gamer, int index)
        {
            GetParent<UI>().GetComponent<GamerComponent>().Add(gamer, index);
            gamer.GetComponent<GamerUIComponent>().SetPanel(this.GamersPanel[index],this.HeadPanel[index],  index);
        }

        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="id"></param>
        public void RemoveGamer(long id)
        {
            Gamer gamer = GetParent<UI>().GetComponent<GamerComponent>().Remove(id);
            gamer.GetComponent<GamerUIComponent>().Panel.SetActive(false);
            gamer.Dispose();
        }

        public void StartGame(int messageRestCount)
        {
            this.changeTableBtn.gameObject.SetActive(false);
            this.readyBtn.gameObject.SetActive(false);
            this.desk.SetActive(true);
            this.head.GetComponentInParent<RectTransform>().gameObject.SetActive(false);
            players.SetActive(true);
            //剩余牌数
            restText.text = $"剩余牌数：{messageRestCount}";
        }



        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        public async void StartTime(int time = 9)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            tokenSource = new CancellationTokenSource();
            timeImage.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + time);
            while (time > 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000, tokenSource.Token);
                time--;
                timeImage.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + time);
            }
        }

        public bool CanPeng(List<MahjongInfo> list, MahjongInfo mahjongInfo)
        {
            if (Logic_NJMJ.getInstance().isCanPeng(mahjongInfo, list))
            {
                pengBtn.gameObject.SetActive(true);
                return true;
            }

            return false;
        }

        public bool CanGang(List<MahjongInfo> list, MahjongInfo mahjongInfo)
        {
            if (Logic_NJMJ.getInstance().isCanGang(mahjongInfo, list))
            {
                gangBtn.gameObject.SetActive(true);
                return true;
            }

            return false;
        }

        public bool CanHu(List<MahjongInfo> list, MahjongInfo mahjongInfo)
        {
            List<MahjongInfo> temp = new List<MahjongInfo>(list);
            temp.Add(mahjongInfo);
            if (Logic_NJMJ.getInstance().isHuPai(temp))
            {
                gangBtn.gameObject.SetActive(true);
                return true;
            }

            return false;
        }

        public void CanTing(List<MahjongInfo> list)
        {
            List<MahjongInfo> checkTingPaiList = Logic_NJMJ.getInstance().checkTingPaiList(list);
            if (checkTingPaiList?.Count != 0)
            {
                Log.Debug("玩家停牌");
            }
        }

        private void OnOperate(int i)
        {
            switch (i)
            {
                //碰
                case 0:
                    break;
                //杠
                case 1:
                    break;
                //胡
                case 2:
                    break;
                //放弃
                case 3:
                    break;
            }

            SessionWrapComponent.Instance.Session.Send(new Actor_GamerOperation()
            {
                OperationType = i
            });

            this.ClosePropmtBtn();
        }

        public void ClosePropmtBtn()
        {
            this.pengBtn.gameObject.SetActive(false);
            this.gangBtn.gameObject.SetActive(false);
            this.huBtn.gameObject.SetActive(false);
            this.giveUpBtn.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示中间的指针
        /// </summary>
        /// <param name="userId"></param>
        public void ShowTurn(long userId)
        {
            GamerComponent gamerComponent = this.GetParent<UI>().GetComponent<GamerComponent>();
            foreach (var _gamer in gamerComponent.GetAll())
            {
                HandCardsComponent cardsComponent = _gamer.GetComponent<HandCardsComponent>();
                if (_gamer.UserID == userId)
                {
                    cardsComponent.ShowBg();
                }
                else
                {
                    cardsComponent.CloseBg();
                }
            }

            //时间重新开始
            StartTime();
        }

        public bool Operate(List<MahjongInfo> allCards, MahjongInfo mahjongInfo)
        {
            if (CanPeng(allCards, mahjongInfo) || CanGang(allCards, mahjongInfo) || CanHu(allCards, mahjongInfo))
            {
                giveUpBtn.gameObject.SetActive(true);
                return true;
            }

            return false;
        }

        public void ShowOperation(int operationType)
        {
            switch (operationType)
            {
                case 0:
                    pengBtn.gameObject.SetActive(true);
                    break;
                case 1:
                    gangBtn.gameObject.SetActive(true);
                    break;
                case 2:
                    huBtn.gameObject.SetActive(true);
                    break;
                case 3:
                    break;
            }

            giveUpBtn.gameObject.SetActive(true);
        }

        public void AddReadyGamer(Gamer gamer, int index)
        {

        }
    }
}