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
        public bool ISGaming = false;

        public readonly GameObject[] GamersPanel = new GameObject[4];
        public readonly GameObject[] HeadPanel = new GameObject[4];

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

        private Button ChatBtn; //聊天按钮
        
        public GameObject currentItem ;

        private Text restText;
        private GameObject players;
        private CancellationTokenSource tokenSource;
        public Actor_GamerEnterRoom enterRoomMsg;
        private int restCardCount;
        private Button settingBtn;
        private Button changeTableBtn;
        public GamerInfo localGamer;
        private GameObject treasure;
        private bool isTreasureFinish;
        private GameObject cheat;
        private InputField cheatInput;
        private Text roomConfigText;
        private GameObject trustship;
        public int RoomType { get; set; }

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            GamersPanel[0] = rc.Get<GameObject>("Bottom");
            GamersPanel[1] = rc.Get<GameObject>("Right");
            GamersPanel[2] = rc.Get<GameObject>("Top");
            GamersPanel[3] = rc.Get<GameObject>("Left");
            this.players = rc.Get<GameObject>("Players");

            this.treasure = rc.Get<GameObject>("Treasure");
            this.treasure.SetActive(false);

            this.desk = rc.Get<GameObject>("Desk");
            this.head = rc.Get<GameObject>("Head");

            HeadPanel[0] = head.Get<GameObject>("Bottom");
            HeadPanel[1] = head.Get<GameObject>("Right");
            HeadPanel[2] = head.Get<GameObject>("Top");
            HeadPanel[3] = head.Get<GameObject>("Left");

            ChatBtn = rc.Get<GameObject>("ChatBtn").GetComponent<Button>();
            

            this.restText = rc.Get<GameObject>("RestText").GetComponent<Text>();
            this.roomConfigText = rc.Get<GameObject>("RoomConfigText").GetComponent<Text>();

            this.settingBtn = rc.Get<GameObject>("SettingBtn").GetComponent<Button>();
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
            
            this.settingBtn.onClick.Add(OnSetting);
            this.exitBtn.onClick.Add(OnExit);
            this.readyBtn.onClick.Add(OnReady);

            #region 托管

            this.trustship = rc.Get<GameObject>("Trustship");
            trustship.SetActive(false);
            trustship.GetComponent<Button>().onClick.Add(() =>
            {
                SessionWrapComponent.Instance.Session.Send(new Actor_GamerCancelTrusteeship());
                trustship.SetActive(false);
            });
            #endregion


            #region 作弊
            this.cheat = rc.Get<GameObject>("Cheat");
            Button cheatButton = this.cheat.Get<GameObject>("CheatButton").GetComponent<Button>();
            Button sendButton = this.cheat.Get<GameObject>("SendButton").GetComponent<Button>();
            this.cheatInput = this.cheat.Get<GameObject>("InputField").GetComponent<InputField>();

            sendButton.gameObject.SetActive(false);
            cheatInput.gameObject.SetActive(false);

            cheatButton.onClick.Add(() =>
            {
                sendButton.gameObject.SetActive(true);
                cheatInput.gameObject.SetActive(true);
            });

            sendButton.onClick.Add(() =>
            {
                SessionWrapComponent.Instance.Session.Call(new Actor_GamerCheat() { Info = this.cheatInput.text });
                sendButton.gameObject.SetActive(false);
                cheatInput.gameObject.SetActive(false);
            });

            #endregion


            #region 聊天
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIChatShow);
            UI ui = Game.Scene.GetComponent<UIComponent>().Create(UIType.UIChat);
            ui.GameObject.SetActive(false);

            ChatBtn.onClick.Add(() =>
            {
                if (ui != null)
                {
                    if (ui.GetComponent<UIChatComponent>().isOpen)
                    {
                        ui.GetComponent<UIChatComponent>().CloseOrOpenChatUI(false);
                    }
                    else
                    {
                        ui.GetComponent<UIChatComponent>().CloseOrOpenChatUI(true);
                    }
                }
            });
            #endregion

        }

        /// <summary>
        /// 设置剩余牌
        /// </summary>
        public void SetRestCount()
        {
            restCardCount--;
            restText.text = $"剩余牌数：{restCardCount}";
        }

        private async void OnReady()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerReady() { Uid = PlayerInfoComponent.Instance.uid });
        }

        private async void OnExit()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerExitRoom() { IsFromClient = true });
            if (ISGaming)
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRoom);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIReady);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChatShow);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChat);
                Debug.Log("remove");
            }
        }

        private async void OnSetting()
        {
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UISet);
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="gamer"></param>
        /// <param name="index"></param>
        public void AddGamer(Gamer gamer, int index)
        {
            try
            {
                GetParent<UI>().GetComponent<GamerComponent>().Add(gamer, index);
                gamer.GetComponent<GamerUIComponent>().SetPanel(this.GamersPanel[index], this.HeadPanel[index], index);
                HeadManager.setHeadSprite(gamer.GetComponent<GamerUIComponent>().head, gamer.PlayerInfo.Icon);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="id"></param>
        public void RemoveGamer(long id)
        {
            Gamer gamer = GetParent<UI>().GetComponent<GamerComponent>().Remove(id);

//            gamer?.GetComponent<GamerUIComponent>()?.Panel?.SetActive(false);
            gamer?.Dispose();
        }

        public void StartGame(int messageRestCount)
        {
            this.restCardCount = messageRestCount;
            this.changeTableBtn.gameObject.SetActive(false);
            this.readyBtn.gameObject.SetActive(false);
            this.desk.SetActive(true);
            this.head.GetComponentInParent<RectTransform>().gameObject.SetActive(false);
            players.SetActive(true);
            //剩余牌数
            restText.text = $"剩余牌数：{messageRestCount}";
        }

        /// <summary>
        /// 继续游戏
        /// </summary>
        public void ContinueGamer()
        {
            this.changeTableBtn.gameObject.SetActive(true);
            this.readyBtn.gameObject.SetActive(true);
            this.desk.SetActive(false);
            this.trustship.SetActive(false);
            this.head.GetComponentInParent<RectTransform>().gameObject.SetActive(true);
            players.SetActive(false);
            isTreasureFinish = true;
            treasure.SetActive(false);

            //剩余牌数
            restText.text = $"";
            Gamer[] gamers = this.GetParent<UI>().GetComponent<GamerComponent>().GetAll();

            foreach (var gamer in gamers)
            {
                if (gamer == null)
                    continue;
                gamer.GetComponent<HandCardsComponent>().ClearAll();
                gamer.RemoveComponent<HandCardsComponent>();
            }

        }

        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        public async void StartTime(int time = 9)
        {
            try
            {
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                }

                tokenSource = new CancellationTokenSource();
                timeImage.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + time);
                while (time > 0)
                {
                    if (tokenSource.Token.IsCancellationRequested) return;
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000, tokenSource.Token);
                    time--;
                    timeImage.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "time_" + time);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
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

            SessionWrapComponent.Instance.Session.Send(new Actor_GamerOperation() { OperationType = i });

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
                case 4:
                case 5:
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

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            base.Dispose();
            isTreasureFinish = true;
            tokenSource?.Cancel();
            ISGaming = false;
        }

        /// <summary>
        /// 设置宝箱倒计时
        /// </summary>
        /// <param name="onlineSeconds"></param>
        public async void SetTreasureTime(int onlineSeconds)
        {
            try
            {
                Log.Debug("宝箱倒计时:" + onlineSeconds);
                this.treasure.SetActive(true);
                Text timeText = this.treasure.transform.Find("Text").GetComponent<Text>();
                Image image = this.treasure.GetComponent<Image>();
                Button button = this.treasure.GetComponent<Button>();
                image.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "Treasure_Close");
                button.interactable = false;
                isTreasureFinish = false;
                while (onlineSeconds > 0)
                {
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                    if (isTreasureFinish) return;
                    onlineSeconds--;
                    int second = onlineSeconds % 60;
                    int minute = onlineSeconds / 60;
                    if (second < 10)
                    {
                        timeText.text = $"{minute}:0{second}";

                    }
                    else
                    {
                        timeText.text = $"{minute}:{second}";

                    }
                }

                timeText.text = "0:00";
                image.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "Treasure_Open");
                button.interactable = true;
                button.onClick.RemoveAllListeners();
                button.onClick.Add(OnGetTreasure);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
          
        }

        private async void OnGetTreasure()
        {
            Log.Debug("领取宝箱");
            M2C_ActorGamerGetTreasure gamerGetTreasure = (M2C_ActorGamerGetTreasure)await SessionWrapComponent.Instance.Session.Call(new C2M_ActorGamerGetTreasure());
            if (gamerGetTreasure.Error == ErrorCode.ERR_Success)
            {
                SetTreasureTime(gamerGetTreasure.RestSeconds);
                ToastScript.createToast($"恭喜你领取{gamerGetTreasure.Reward}金币");
            }
            else
            {
                ToastScript.createToast(gamerGetTreasure.Message);
            }
        }
        /// <summary>
        /// 设置房间类型
        /// </summary>
        /// <param name="roomType"></param>
        public void SetRoomType(int roomType)
        {
            this.RoomType = roomType;

            if (roomType == 1)
            {
                roomConfigText.text = "100/花";
            }
            else
            {
                roomConfigText.text = "500/花";
            }
        }

        public void ShowTrustship()
        {
            trustship.SetActive(true);
        }
    }
}