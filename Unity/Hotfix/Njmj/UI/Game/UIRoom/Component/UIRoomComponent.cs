﻿using System;
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
        public static bool ISGaming = false;
        public RoomConfig RoomConfig = new RoomConfig();
        public readonly GameObject[] GamersPanel = new GameObject[4];
        public readonly GameObject[] HeadPanel = new GameObject[4];
        public readonly GameObject[] FacePanel = new GameObject[4];

        public Button exitBtn;
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
        public GameObject players;
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
        public GameObject faceCardObj;
        public GameObject dice;
        public GameObject tip;
        private GameObject baoxiang;
        private Text currentJuCountText;
        public long masterUserId; //房主uid

        public int RoomType { get; set; }
        //当前出牌或者抓牌
        public MahjongInfo CurrentMahjong { get; set; }
        public static bool IsFriendRoom;
        private static int CurrentJuCount;
        private Image showAnimImage;
        public int JuCount { get; set; }

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
            this.baoxiang = rc.Get<GameObject>("Baoxiang");

            HeadPanel[0] = head.Get<GameObject>("Bottom");
            HeadPanel[1] = head.Get<GameObject>("Right");
            HeadPanel[2] = head.Get<GameObject>("Top");
            HeadPanel[3] = head.Get<GameObject>("Left");

            ChatBtn = rc.Get<GameObject>("ChatBtn").GetComponent<Button>();
            

            this.restText = rc.Get<GameObject>("RestText").GetComponent<Text>();
            this.roomConfigText = rc.Get<GameObject>("RoomConfigText").GetComponent<Text>();
            this.currentJuCountText = rc.Get<GameObject>("CurrentJuCountText").GetComponent<Text>();

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

            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);

            #region 托管

            this.trustship = rc.Get<GameObject>("Trustship");
            trustship.SetActive(false);
            trustship.GetComponent<Button>().onClick.Add(() =>
            {
                SessionComponent.Instance.Session.Send(new Actor_GamerCancelTrusteeship());
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
                SessionComponent.Instance.Session.Call(new Actor_GamerCheat() { Info = this.cheatInput.text });
                sendButton.gameObject.SetActive(false);
                cheatInput.gameObject.SetActive(false);
            });

            if (NetConfig.getInstance().isFormal)
            {
                this.cheat.SetActive(false);
            }
            else
            {
                this.cheat.SetActive(true);
            }
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

            #region 花牌显示
            this.faceCardObj = rc.Get<GameObject>("FaceCardObj");
            faceCardObj.SetActive(false);
            FacePanel[0] = this.faceCardObj.Get<GameObject>("Bottom");
            FacePanel[1] = this.faceCardObj.Get<GameObject>("Right");
            FacePanel[2] = this.faceCardObj.Get<GameObject>("Top");
            FacePanel[3] = this.faceCardObj.Get<GameObject>("Left");
            Button backBtn = this.faceCardObj.Get<GameObject>("Button_back").GetComponent<Button>();
            backBtn.onClick.Add(() =>
            {
                faceCardObj.gameObject.SetActive(false);
            });

            #endregion

            #region 骰子
            this.dice = rc.Get<GameObject>("Dice");
            #endregion

            #region 提示
            this.tip = rc.Get<GameObject>("Tip");
            #endregion

            #region 四连风等

            this.showAnimImage = rc.Get<GameObject>("ShowAnim").GetComponent<Image>();
            this.showAnimImage.gameObject.SetActive(false);

            #endregion
        }

        /// <summary>
        /// 设置剩余牌
        /// </summary>
        public void SetRestCount()
        {
            restCardCount--;
            restText.text = $"剩余牌数:{restCardCount}";
        }

        private async void OnReady()
        {
            SessionComponent.Instance.Session.Send(new Actor_GamerReady() { Uid = PlayerInfoComponent.Instance.uid });
        }

        public static void OnExit()
        {
            try
            {
                if (!IsFriendRoom)
                {
                    UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("通知", "是否退出房间？");
                    script.setOnClickOkEvent(() =>
                    {
                        SessionComponent.Instance.Session.Send(new Actor_GamerExitRoom() { IsFromClient = true });
                        if (ISGaming)
                        {
                            CommonUtil.ShowUI(UIType.UIMain);
                            GameUtil.Back2Main();
                        }
                    });

                    script.setOnClickCloseEvent(() =>
                    {
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                    });

                    script.getTextObj().alignment = TextAnchor.MiddleCenter;
                }
                //好友房硬件返回屏蔽
                else
                {
                     if (!ISGaming && CurrentJuCount == 0)
                     {
                         UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel("通知", "是否退出房间？");
                         script.setOnClickOkEvent(() =>
                         {
                             SessionComponent.Instance.Session.Send(new Actor_GamerExitRoom() { IsFromClient = true });
                             if (ISGaming)
                             {
                                 CommonUtil.ShowUI(UIType.UIMain);
                                 GameUtil.Back2Main();
                             }
                         });
                    
                         script.setOnClickCloseEvent(() =>
                         {
                             Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                         });
                    
                         script.getTextObj().alignment = TextAnchor.MiddleCenter;
                     }
                }
               
            }
            catch (Exception e)
            {
                Log.Error(e);
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
            players?.SetActive(false);
            this.changeTableBtn.gameObject.SetActive(true);
            this.readyBtn.gameObject.SetActive(true);
            this.desk.SetActive(false);
            this.trustship.SetActive(false);

            //碰刚按钮隐藏
            this.giveUpBtn.gameObject.SetActive(false);
            this.huBtn.gameObject.SetActive(false);
            this.gangBtn.gameObject.SetActive(false);
            this.pengBtn.gameObject.SetActive(false);
           
            this.head.GetComponentInParent<RectTransform>().gameObject.SetActive(true);
            players.SetActive(false);
            isTreasureFinish = true;
            treasure.SetActive(false);
            faceCardObj.SetActive(false);
            //剩余牌数
            restText.text = $"";
            GamerComponent gamerComponent = this.GetParent<UI>().GetComponent<GamerComponent>();
            gamerComponent.IsPlayed = false;
            Gamer[] gamers = gamerComponent.GetAll();

            foreach (var gamer in gamers)
            {
                if (gamer == null)
                    continue;
                gamer?.GetComponent<HandCardsComponent>()?.ClearAll();
                gamer?.GetComponent<GamerUIComponent>()?.zhuang.SetActive(false);
                gamer?.RemoveComponent<HandCardsComponent>();
            }

            foreach (var face in FacePanel)
            {
                GameHelp.DeleteAllItem(face.gameObject);
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

            SessionComponent.Instance.Session.Send(new Actor_GamerOperation() { OperationType = i });

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
            UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            GamerComponent gamerComponent = uiRoom.GetComponent<GamerComponent>();

            Gamer gamer = gamerComponent.Get(PlayerInfoComponent.Instance.uid);
            HandCardsComponent handCardsComponent = gamer.GetComponent<HandCardsComponent>();
            switch (operationType)
            {
                case 0:
                    pengBtn.gameObject.SetActive(true);
                    handCardsComponent.ShowHandCardCanPeng(CurrentMahjong.weight);

                    break;
                case 1:
                case 4:
                case 5:
                    gangBtn.gameObject.SetActive(true);
                    handCardsComponent.ShowHandCardCanPeng(CurrentMahjong.weight);
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

                if (onlineSeconds < 0)
                {
                    this.treasure.SetActive(false);
                    return;
                }

                while (onlineSeconds > 0)
                {
                    await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                    if (isTreasureFinish || this.IsDisposed) return;
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
            M2C_ActorGamerGetTreasure gamerGetTreasure = (M2C_ActorGamerGetTreasure)await SessionComponent.Instance.Session.Call(new C2M_ActorGamerGetTreasure());
            if (gamerGetTreasure.Error == ErrorCode.ERR_Success)
            {
                SetTreasureTime(gamerGetTreasure.RestSeconds);
                baoxiang.SetActive(true);
                baoxiang.GetComponentInChildren<Text>().text = $"恭喜你领取{gamerGetTreasure.Reward}金币";
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(2000);
                if (this.IsDisposed)
                {
                    return;
                }
                baoxiang.SetActive(false);

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
        /// <param name="messageJuCount"></param>
        public void SetRoomType(int roomType, int multiples)
        {
            this.RoomType = roomType;

            if (roomType == 1)
            {
                roomConfigText.text = "100/花";
            }
            else if(roomType == 2)
            {
                roomConfigText.text = "500/花";
            }
            else
            {
                roomConfigText.text = $"{multiples}/花";
            }
        }

        public void ShowTrustship()
        {
            trustship.SetActive(true);
        }

        /// <summary>
        /// 好友房设置
        /// </summary>
        public void SetFriendSetting(long masterUserId)
        {
            this.masterUserId = masterUserId;
            settingBtn.GetComponentInChildren<Text>().text = "设 置";
            if (masterUserId == PlayerInfoComponent.Instance.uid)
            {
                exitBtn.GetComponentInChildren<Text>().text = "解 散";
                exitBtn.onClick.RemoveAllListeners();
                exitBtn.onClick.Add(OnRoomDismiss);
            }
            else
            {
                exitBtn.GetComponentInChildren<Text>().text = "退 出";
            }
          
        }
        /// <summary>
        /// 解散房间事件
        /// </summary>
        private void OnRoomDismiss()
        {
            SessionComponent.Instance.Session.Send(new Actor_GamerApplyRoomDismiss());
        }

        /// <summary>
        /// 设置当前局数
        /// </summary>
        /// <param name="currentJuCount"></param>
        public void SetCurrentJuCount(int currentJuCount)
        {
            CurrentJuCount = currentJuCount;
            currentJuCountText.text = $"{currentJuCount}/{JuCount}局";
        }

        /// <summary>
        /// 显示四连风 2,四连风；3，fafen
        /// </summary>
        /// <param name="type"></param>
        public async void ShowAnim(int type)
        {
            switch (type)
            {
                case 1:
                    break;
                case 2:
                    showAnimImage.sprite = CommonUtil.getSpriteByBundle("Image_GameAnimation", "silianfeng");
                    break;
                case 3:
                    showAnimImage.sprite = CommonUtil.getSpriteByBundle("Image_GameAnimation", "fafen");
                    break;
                case 4:
                    break;
            }

            showAnimImage.gameObject.SetActive(true);

            showAnimImage.SetNativeSize();
            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1500);
            if (this.IsDisposed)
            {
                return;
            }

            showAnimImage.gameObject.SetActive(false);
        }   
    }
}