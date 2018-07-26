using System;
using DG.Tweening;
using ETModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Hotfix;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponentSystem: StartSystem<UIMainComponent>
    {
        public override void Start(UIMainComponent self)
        {
            self.Start();
        }
    }

    public class UIMainComponent: Component
    {
        private bool isDispose = false;

        private Text playerNameTxt;
        private Text goldNumTxt;
        private Text wingNumTxt;
        private Text HuaFeiNumTxt;

        private Image playerIcon;

        private GameObject LaBa;
        private GameObject PlayerInfoBg;
        private GameObject BtnList_Down;
        private GameObject BtnList_Up;
        private GameObject Rank;
        private GameObject ChoiceRoomType;
        private GameObject Relax;
        private GameObject RankItem;
        private GameObject Btn_GoldSelect;
        private GameObject Btn_GameSelect;
        private GameObject Grid;
        private Button DetailBtn;

        #region 好友房
        private GameObject FriendRoom;
        private GameObject FriendGrid;
        private GameObject NoRoomTipTxt;
        private Button JoinRoomBtn;
        private Button CreateRoomBtn;
        private Button CloseFrRoomBtn;
        private Text ScoreTxt;
        private Button GameBtn;

        private List<GameObject> roomItems = new List<GameObject>();
        private List<UI> uiFList = new List<UI>();
        private GameObject roomItem = null;
        #endregion

        #region myRank
        public Text GoldTxt;
        private Text NameTxt;
        private Text RankTxt;
        private Image Icon;
        private GameObject RankImg;
        private Button RewardBtn;
        #endregion

        private List<WealthRank> wealthRankList = new List<WealthRank>();
        private List<GameRank> gameRankList = new List<GameRank>();
        private List<GameObject> rankItemList = new List<GameObject>();
        private List<GameObject> gameItemList = new List<GameObject>();
        private List<UI> uiList = new List<UI>();
        private List<UI> gameUiList = new List<UI>();
        private GameRank ownGameRank;
        private WealthRank ownWealthRank;
        private int ownRank = 30;
        private int ownGame = 30;
        private bool isOwnRank = false;
        private bool isGameRank = false;

        GameObject RealNameTip = null;

        public async void Start()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            playerNameTxt = rc.Get<GameObject>("PlayerNameTxt").GetComponent<Text>();
            goldNumTxt = rc.Get<GameObject>("GoldNumTxt").GetComponent<Text>();
            wingNumTxt = rc.Get<GameObject>("WingNumTxt").GetComponent<Text>();
            HuaFeiNumTxt = rc.Get<GameObject>("HuaFeiNumTxt").GetComponent<Text>();
            playerIcon = rc.Get<GameObject>("PlayerIcon").GetComponent<Image>();

            LaBa = rc.Get<GameObject>("LaBa");
            PlayerInfoBg = rc.Get<GameObject>("PlayerInfoBg");
            BtnList_Down = rc.Get<GameObject>("BtnList_Down");
            BtnList_Up = rc.Get<GameObject>("BtnList_Up");
            Rank = rc.Get<GameObject>("Rank");
            ChoiceRoomType = rc.Get<GameObject>("ChoiceRoomType");
            Relax = rc.Get<GameObject>("Relax");
            Btn_GoldSelect = rc.Get<GameObject>("Btn_GoldSelect");
            Btn_GameSelect = rc.Get<GameObject>("Btn_GameSelect");
            Grid = rc.Get<GameObject>("Grid");

            GoldTxt = rc.Get<GameObject>("GoldTxt").GetComponent<Text>();
            NameTxt = rc.Get<GameObject>("NameTxt").GetComponent<Text>();
            RankTxt = rc.Get<GameObject>("RankTxt").GetComponent<Text>();
            Icon = rc.Get<GameObject>("Icon").GetComponent<Image>();
            RankImg = rc.Get<GameObject>("RankImg");
            RewardBtn = rc.Get<GameObject>("RewardBtn").GetComponent<Button>();
            DetailBtn = rc.Get<GameObject>("DetailBtn").GetComponent<Button>();

            #region 好友房
            FriendGrid = rc.Get<GameObject>("FriendGrid");
            FriendRoom  = rc.Get<GameObject>("FriendRoom");
            NoRoomTipTxt = rc.Get<GameObject>("NoRoomTipTxt");
            JoinRoomBtn = rc.Get<GameObject>("JoinRoomBtn").GetComponent<Button>();
            CreateRoomBtn = rc.Get<GameObject>("CreateRoomBtn").GetComponent<Button>();
            CloseFrRoomBtn = rc.Get<GameObject>("CloseFrRoomBtn").GetComponent<Button>();
            ScoreTxt = rc.Get<GameObject>("ScoreTxt").GetComponent<Text>();
            GameBtn = rc.Get<GameObject>("GameBtn").GetComponent<Button>();

            roomItem = CommonUtil.getGameObjByBundle(UIType.UIFriendRoomItem);
            #endregion

            CommonUtil.SetTextFont(FriendRoom);
            CommonUtil.SetTextFont(this.GetParent<UI>().GameObject);

            #region 加入房间
            //打开加入房间
            JoinRoomBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIJoinRoom);
            });

            //我的战绩
            GameBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIFriendRoomRank);
            });

            //关闭好友房界面
            CloseFrRoomBtn.onClick.Add(() =>
            {
                SetUIShow(true);
                isStop = true;
            });

            ////打开创建房间UI
            CreateRoomBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UICreateFriendRoom);
            });

            #endregion

            //周排行规则以及奖励明细
            DetailBtn.onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIWeekRank);
            });

            // 转盘
            BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Button>().onClick.Add(() =>
            {
                // 向左
                if (BtnList_Down.transform.localPosition.x > 400)
                {
                    BtnList_Down.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-392.0f, 73.6f), 0.5f, false).OnComplete(() =>
                    {
                        PlayerInfoBg.transform.Find("HuaFeiBg").transform.localScale = Vector3.zero;
                    });
                    BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main","btn_you");
                }
                // 向右
                else
                {
                    BtnList_Down.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-127.4f, 73.6f), 0.5f, false).OnComplete(() =>
                    {
                        PlayerInfoBg.transform.Find("HuaFeiBg").transform.localScale = new Vector3(1, 1, 1);
                    });

                    BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", "btn_zuo");
                }
            });

            // 喇叭
            {
                LaBa.transform.Find("Btn_laba").GetComponent<Button>().onClick
                        .Add(() => { Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseLaBa); });

                LaBa.transform.Find("Text_content").GetComponent<Text>().text = GameUtil.getTips();
            }

            // 商城
            BtnList_Down.transform.Find("Grid/Btn_Shop").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIShop);
            });

            // 活动
            BtnList_Down.transform.Find("Grid/Btn_Activity").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIActivity);
            });

            // 任务
            BtnList_Down.transform.Find("Grid/Btn_Task").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UITask);
            });

            // 成就
            BtnList_Down.transform.Find("Grid/Btn_ChengJiu").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIChengjiu);
                
            });

            // 背包
            BtnList_Down.transform.Find("Grid/Btn_Bag").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIBag);
            });
            
            // 转盘
            BtnList_Down.transform.Find("Grid/Btn_ZhuanPan").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIZhuanPan);
            });

            // 每日必做
            BtnList_Up.transform.Find("Btn_Daily").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIDaily);
            });

            // 邮箱
            BtnList_Up.transform.Find("Btn_Mail").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIEmail);
            });

            // 帮助
            BtnList_Up.transform.Find("Btn_Help").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIHelp);
            });

            // 休闲场
            ChoiceRoomType.transform.Find("Btn_relax").GetComponent<Button>().onClick.Add(() =>
            {
                ChoiceRoomType.transform.Find("Btn_relax").transform.localScale = Vector3.zero;
                ChoiceRoomType.transform.Find("Btn_pvp").transform.localScale = Vector3.zero;

                ChoiceRoomType.transform.Find("Relax").transform.localScale = new Vector3(1,1,1);
            });

            // 比赛场
            ChoiceRoomType.transform.Find("Btn_pvp").GetComponent<Button>().onClick.Add(() =>
            {
                //ToastScript.createToast("暂未开放：比赛场");
                //return
                ShowFriendRoom();
            });

            // 休闲场返回按钮
            ChoiceRoomType.transform.Find("Relax/Btn_back").GetComponent<Button>().onClick.Add(() =>
            {
                ChoiceRoomType.transform.Find("Btn_relax").transform.localScale = new Vector3(1, 1, 1);
                ChoiceRoomType.transform.Find("Btn_pvp").transform.localScale = new Vector3(1, 1, 1);

                ChoiceRoomType.transform.Find("Relax").transform.localScale = Vector3.zero;
            });

            // 休闲场-新手场
            ChoiceRoomType.transform.Find("Relax/Btn_xinshou").GetComponent<Button>().onClick.Add(() =>
            {
                OnEnterRoom(1);
            });

            // 休闲场-精英场
            ChoiceRoomType.transform.Find("Relax/Btn_jingying").GetComponent<Button>().onClick.Add(() =>
            {
                OnEnterRoom(2);
            });

            PlayerInfoBg.transform.Find("HuaFeiBg/Btn_DuiHuan").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseHuaFei);
            });

            PlayerInfoBg.transform.Find("Btn_set").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                
            });

            playerIcon.GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                
            });

            PlayerInfoBg.transform.Find("HeadKuang").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                
            });

            RankItem = CommonUtil.getGameObjByBundle(UIType.UIRankItem);
            
            
            Rank.transform.Find("Btn_gold").GetComponent<Button>().onClick.Add(() =>
            {
                curType = 1;
                ShowGoldRank();
                SetMyRank();
                RewardBtn.gameObject.SetActive(g2cWeek.IsGetGoldRank);
            });

            Rank.transform.Find("Btn_game").GetComponent<Button>().onClick.Add(() =>
            {
                curType = 2;
                ShowGameRank();
                SetMyGameRank();
                RewardBtn.gameObject.SetActive(g2cWeek.IsGetGameRank);
            });

            //可以领取周排行榜奖励
            RewardBtn.onClick.Add(() =>
            {
                GetWeekReward();
            });

            //PlayerPrefs.DeleteAll();
            ShowNotice();

            WeekRankReq();

            //向服务器发送消息请求玩家信息，然后设置玩家基本信息
            await SetPlayerInfo();
            GetRankInfo();

            if (!PlayerInfoComponent.Instance.GetPlayerInfo().IsSign)
            {
                CommonUtil.ShowUI(UIType.UIDaily);
            }
            SetRedTip();
            HeartBeat.getInstance().startHeartBeat();
            // 实名认证提示
            try
            {
                RealNameTip = PlayerInfoBg.transform.Find("RealNameTip").gameObject;
                RealNameTip.transform.Find("Button_close").GetComponent<Button>().onClick.Add(() =>
                {
                    RealNameTip.transform.localScale = Vector3.zero;
                });

                if (OtherData.getIsShiedRealName())
                {
                    RealNameTip.transform.localScale = Vector3.zero;
                }
                else
                {
                    if (PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName)
                    {
                        RealNameTip.transform.localScale = Vector3.zero;
                    }
                    else
                    {
                        DOTween.Sequence().Append(RealNameTip.GetComponent<RectTransform>().DOAnchorPos(new Vector2(117.3f, 114.38f), 0.8f, false))
                            .Append(RealNameTip.GetComponent<RectTransform>().DOAnchorPos(new Vector2(117.3f, 97.1f), 0.8f, false)).SetLoops(-1).Play();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// 打开好友房时需要隐藏大厅一些东西
        /// </summary>
        /// <param name="isActive"></param>
        public void SetUIShow(bool isActive)
        {
            LaBa.SetActive(isActive);
            ChoiceRoomType.SetActive(isActive);
            Rank.SetActive(isActive);
            FriendRoom.SetActive(!isActive);
        }

        long m_durTime = 5000;
        bool isStop = false;

        public async void ShowFriendRoom()
        {
            SetUIShow(false);
            GetRoomInfoReq();
            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(m_durTime);
            StartFriendReq();
        }

        public async void StartFriendReq()
        {
            isStop = false;

            while (!isStop)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(m_durTime);
                GetRoomReqX();
            }
        }

        #region 好友房
        private async void GetRoomReqX()
        {
            G2C_FriendRoomInfo m2cFriend = (G2C_FriendRoomInfo)await SessionComponent.Instance.Session.Call(new C2G_FriendRoomInfo { UId = PlayerInfoComponent.Instance.uid });
            CreateUI(m2cFriend);
        }

        private async void GetRoomInfoReq()
        {
            #region 向服务器请求信息
            UINetLoadingComponent.showNetLoading();

            G2C_FriendRoomInfo m2cFriend = (G2C_FriendRoomInfo)await SessionComponent.Instance.Session.Call(new C2G_FriendRoomInfo { UId = PlayerInfoComponent.Instance.uid });

            UINetLoadingComponent.closeNetLoading();

            CreateUI(m2cFriend);
        }

        private void CreateUI(G2C_FriendRoomInfo m2cFriend)
        {
            ScoreTxt.text = m2cFriend.Score.ToString();

            //今天沒有贈送好友房钥匙
            if (!PlayerInfoComponent.Instance.GetPlayerInfo().IsGiveFriendKey)
            {
                //显示赠送界面
                ToastScript.createToast("每日赠送3把钥匙，仅限当日使用");
                PlayerInfoComponent.Instance.GetPlayerInfo().IsGiveFriendKey = m2cFriend.IsGiveFriendKey;
            }

            PlayerInfoComponent.Instance.GetPlayerInfo().FriendKeyCount = m2cFriend.KeyCount;
            #endregion

            if (m2cFriend.Info.Count <= 0)
            {
                NoRoomTipTxt.SetActive(true);
            }
            else
            {
                NoRoomTipTxt.SetActive(false);
            }
            CreateRoomItemss(m2cFriend.Info);
        }

        /// <summary>
        /// 创建房间Item
        /// </summary>
        private void CreateRoomItemss(List<FriendRoomInfo> roomInfos)
        {
            if(roomInfos.Count <= 0)
            {
                HideMore(0);
            }

            GameObject obj = null;
            for (int i = 0; i < roomInfos.Count; ++i)
            {
                if (i < roomItems.Count)
                {
                    roomItems[i].SetActive(true);
                    obj = roomItems[i];
                }
                else
                {
                    obj = GameObject.Instantiate(roomItem, FriendGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    roomItems.Add(obj);
                    UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                    ui.AddComponent<UIFriendRoomItemComponent>();
                    uiFList.Add(ui);
                }
                uiFList[i].GetComponent<UIFriendRoomItemComponent>().SetItemInfo(roomInfos[i]);
            }
            HideMore(roomInfos.Count);
        }

        private void HideMore(int index)
        {
            for(int i = index;i< roomItems.Count;++i)
            {
                roomItems[i].SetActive(false);
            }
        }
        #endregion

        private int curType = 2;
        private async void GetWeekReward()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_GetWeekReward g2cWR = (G2C_GetWeekReward)await Game.Scene.GetComponent<SessionComponent>()
                .Session.Call(new C2G_GetWeekReward { UId = PlayerInfoComponent.Instance.uid, type = curType });
            UINetLoadingComponent.closeNetLoading();

            if (g2cWR.Error != ErrorCode.ERR_Success)
            {
                ToastScript.createToast(g2cWR.Message);
                return;
            }
            RewardBtn.gameObject.SetActive(false);
            g2cWeek.IsGetGameRank = g2cWR.IsGetGameRank;
            g2cWeek.IsGetGoldRank = g2cWR.IsGetGoldRank;
            string str = "";
            if (curType == 1)
            {
                str = "2:" + g2cWR.GoldReward;
                //ToastScript.createToast($"领取成功,获得元宝{g2cWR.GoldReward}");
            }
            else if(curType == 2)
            {
                str = "111:" + g2cWR.GameReward;
               /* ToastScript.createToast($"领取成功,获得话费礼包{g2cWR.GameReward}");*/
            }
            ShowRewardUtil.Show(str);

        }

        private void ShowNotice()
        {
            for (int i = 0; i < NoticeConfig.getInstance().getDataList().Count; i++)
            {
                NoticeInfo config = NoticeConfig.getInstance().getDataList()[i];
               
                string key = $"{PlayerInfoComponent.Instance.uid}{config.id}";
                if (PlayerPrefs.GetInt(key) != 1)
                {
                    PlayerPrefs.SetInt(key, 1);
                    UICommonPanelComponent script = UICommonPanelComponent.showCommonPanel(config.title, config.content);
                    script.setOnClickOkEvent(() =>
                    {
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                        ShowNotice();
                    });

                    script.setOnClickCloseEvent(() =>
                    {
                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.UICommonPanel);
                        ShowNotice();
                    });

                    break;
                }
            }
        }

        private G2C_WeekRank g2cWeek;
        private async void WeekRankReq()
        {
            UINetLoadingComponent.showNetLoading();
            g2cWeek = (G2C_WeekRank)await Game.Scene.GetComponent<SessionComponent>()
                .Session.Call(new C2G_WeekRank { UId = PlayerInfoComponent.Instance.uid});
            UINetLoadingComponent.closeNetLoading();
        }

        /// <summary>
        /// 设置?点提示
        /// </summary>
        private async void SetRedTip()
        {
            G2C_Tip g2c = (G2C_Tip)await Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2G_Tip
            {
                UId = PlayerInfoComponent.Instance.uid
            });

            //设置?点
            SetRedTip(1, g2c.IsTaskComplete,g2c.TaskCompleteCount);
            SetRedTip(2, g2c.IsChengjiuComplete, g2c.ChengjiuCompleteCount);
            SetRedTip(3, g2c.IsInActivity, g2c.ActivityCompleteCount);
            SetRedTip(4, g2c.IsZhuanpan, g2c.ZhuanpanCount);
            SetRedTip(5, g2c.IsEmail, g2c.EmailCount);
        }

        /// <summary>
        /// 设置红点提示状态 1,任务 2,成就 3,活动 4,转盘 5,邮件
        /// </summary>
        /// <param name="state"></param>
        public void SetRedTip(int state, bool isHide, int count = 0)
        {
            switch (state)
            {
                case 1:
                    BtnList_Down.transform.Find("Grid/Btn_Task/Tip").gameObject.SetActive(isHide);
                    BtnList_Down.transform.Find("Grid/Btn_Task/Tip/Count").GetComponent<Text>().text = count.ToString();
                    break;
                case 2:
                    BtnList_Down.transform.Find("Grid/Btn_ChengJiu/Tip").gameObject.SetActive(isHide);
                    BtnList_Down.transform.Find("Grid/Btn_ChengJiu/Tip/Count").GetComponent<Text>().text = count.ToString();
                    break;
                case 3:
                    BtnList_Down.transform.Find("Grid/Btn_Activity/Tip").gameObject.SetActive(isHide);
                    BtnList_Down.transform.Find("Grid/Btn_Activity/Tip/Count").GetComponent<Text>().text = count.ToString();
                    break;
                case 4:
                    BtnList_Down.transform.Find("Grid/Btn_ZhuanPan/Tip").gameObject.SetActive(isHide);
                    BtnList_Down.transform.Find("Grid/Btn_ZhuanPan/Tip/Count").GetComponent<Text>().text = count.ToString();
                    break;
                case 5:
                    BtnList_Up.transform.Find("Btn_Mail/Tip").gameObject.SetActive(isHide);
                    BtnList_Up.transform.Find("Btn_Mail/Tip/Count").GetComponent<Text>().text = count.ToString();
                    break;
            }

            if (int.Parse(BtnList_Down.transform.Find("Grid/Btn_Task/Tip/Count").GetComponent<Text>().text) > 0)
            {
                FingerAnimation.Show(BtnList_Down.transform.Find("Grid/Btn_Task").gameObject);
            }
            else if (int.Parse(BtnList_Down.transform.Find("Grid/Btn_ChengJiu/Tip/Count").GetComponent<Text>().text) > 0)
            {
                FingerAnimation.Show(BtnList_Down.transform.Find("Grid/Btn_ChengJiu").gameObject);
            }
            else if (int.Parse(BtnList_Down.transform.Find("Grid/Btn_ZhuanPan/Tip/Count").GetComponent<Text>().text) > 0)
            {
                FingerAnimation.Show(BtnList_Down.transform.Find("Grid/Btn_ZhuanPan").gameObject);
            }
            else if (int.Parse(BtnList_Up.transform.Find("Btn_Mail/Tip/Count").GetComponent<Text>().text) > 0)
            {
                FingerAnimation.Show(BtnList_Up.transform.Find("Btn_Mail").gameObject);
            }
            else
            {
                FingerAnimation.Hide();
            }
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            isStop = true;
            isDispose = true;
            wealthRankList.Clear();
            gameRankList.Clear();
            rankItemList.Clear();
            gameItemList.Clear();
            uiList.Clear();
            gameUiList.Clear();
            uiFList.Clear();
            roomItems.Clear();
        }

        /// <summary>
        /// 显示财富榜
        /// </summary>
        private void ShowGoldRank()
        {
            GameObject obj = null;
            Btn_GoldSelect.gameObject.SetActive(true);
            Btn_GameSelect.gameObject.SetActive(false);
            //Grid.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
            for (int i = 0; i < wealthRankList.Count; ++i)
            {
                if (i < rankItemList.Count)
                {
                    rankItemList[i].SetActive(true);
                    obj = rankItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(RankItem);
                    obj.transform.SetParent(Grid.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    rankItemList.Add(obj);
                }
                UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                ui.AddComponent<UIRankItemComponent>();
                uiList.Add(ui);

                if (i >= ownRank)
                {   
                    uiList[i].GetComponent<UIRankItemComponent>().SetGoldItem(wealthRankList[i], i + 1);
                }
                else
                {
                    uiList[i].GetComponent<UIRankItemComponent>().SetGoldItem(wealthRankList[i], i);
                }
            }
            SetMoreHide(wealthRankList.Count, rankItemList);
        }

        private void SetMoreHide(int index,List<GameObject> list)
        {
            for(int i = index;i < list.Count; ++i)
            {
                list[i].SetActive(false);
            }
        }

        /// <summary>
        /// 显示战绩榜
        /// </summary>
        private void ShowGameRank()
        {
            Btn_GoldSelect.gameObject.SetActive(false);
            Btn_GameSelect.gameObject.SetActive(true);
            //Grid.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
            GameObject obj = null;
            for (int i = 0; i < gameRankList.Count; ++i)
            {
                if (i < rankItemList.Count)
                {
                    rankItemList[i].SetActive(true);
                    obj = rankItemList[i];
                }
                else
                {
                    obj = GameObject.Instantiate(RankItem);
                    obj.transform.SetParent(Grid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    rankItemList.Add(obj);
                }

                UI ui = ComponentFactory.Create<UI, GameObject>(obj);
                ui.AddComponent<UIRankItemComponent>();
                gameUiList.Add(ui);

                if (i >= ownGame)
                {
                    gameUiList[i].GetComponent<UIRankItemComponent>().SetGameItem(gameRankList[i], i + 1);
                }
                else
                {
                    gameUiList[i].GetComponent<UIRankItemComponent>().SetGameItem(gameRankList[i], i);
                }
            }
            SetMoreHide(gameRankList.Count, rankItemList);
        }

        /// <summary>
        /// 设置排行榜信息
        /// </summary>
		public async void GetRankInfo()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_Rank g2cRank = (G2C_Rank)await Game.Scene.GetComponent<SessionComponent>()
                .Session.Call(new C2G_Rank { Uid = PlayerInfoComponent.Instance.uid,RankType = 0 });
            UINetLoadingComponent.closeNetLoading();

            //设置排行榜信息
            wealthRankList = g2cRank.RankList;
            gameRankList = g2cRank.GameRankList;

            isOwnRank = IsInRank(PlayerInfoComponent.Instance.uid);
            isGameRank = IsInGameRank(PlayerInfoComponent.Instance.uid);

            ownRank = GetWealthIndext(PlayerInfoComponent.Instance.uid);
            ownGame = GetGameIndext(PlayerInfoComponent.Instance.uid);
            ShowGameRank();
            ownWealthRank = g2cRank.OwnWealthRank;
            ownGameRank = g2cRank.OwnGameRank;
            SetMyGameRank();
        }

        /// <summary>
        /// 判断是否上财富榜
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsInRank(long uid)
        {
            if (wealthRankList.Count <= 0) return true;
            for (int i = 0; i < wealthRankList.Count; ++i)
            {
                if (wealthRankList[i].UId == uid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否上战绩榜
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsInGameRank(long uid)
        {
            if (gameRankList.Count <= 0) return true;
            for(int i = 0;i< gameRankList.Count; ++i)
            {
                if (gameRankList[i].UId == uid)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获得我的名次（战绩榜）
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private int GetGameIndext(long uid)
        {
            if (gameRankList.Count <= 0) return 0;
            for (int i = 0; i < gameRankList.Count; ++i)
            {
                if (gameRankList[i].UId == uid)
                {
                    gameRankList.RemoveAt(i);
                    return i;
                }
                   
            }
            return 50;
        }

        /// <summary>
        /// 获得我的名次（财富榜）
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private int GetWealthIndext(long uid)
        {
            if (wealthRankList.Count <= 0) return 0;
            for (int i = 0; i < wealthRankList.Count; ++i)
            {
                if (wealthRankList[i].UId == uid)
                {
                    wealthRankList.RemoveAt(i);
                    return i;
                }
            }
            return 50;
        }

        /// <summary>
        /// 设置我的财富榜信息
        /// </summary>
        private void SetMyRank()
        {
            string str = "";

            if (!isOwnRank)
            {
                RankTxt.gameObject.SetActive(true);
                RankImg.SetActive(false);
                str = "未上榜";
            }
            else
            {
                if (ownRank < 3)
                {
                    RankTxt.gameObject.SetActive(false);
                    RankImg.SetActive(true);
                    string iconName = new StringBuilder().Append("Rank_")
                                                         .Append(ownRank + 1).ToString();
                    RankImg.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", iconName);
                }
                else
                {
                    RankTxt.gameObject.SetActive(true);
                    RankImg.SetActive(false);
                    str = (ownRank + 1).ToString();
                }
            }
            RankTxt.text = str;
            NameTxt.text = ownWealthRank.PlayerName;
            GoldTxt.text = new StringBuilder().Append("金币:")
                                              .Append(ownWealthRank.GoldNum)
                                              .ToString();
            RewardBtn.gameObject.SetActive(g2cWeek.IsGetGoldRank);
            HeadManager.setHeadSprite(Icon, PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
        }

        /// <summary>
        /// 设置我的战绩榜信息
        /// </summary>
        private void SetMyGameRank()
        {
            string str = "";
            if (!isGameRank)
            {
                RankTxt.gameObject.SetActive(true);
                RankImg.SetActive(false);
                str = "未上榜";
            }
            else
            {
                if (ownGame < 3)
                {
                    RankTxt.gameObject.SetActive(false);
                    RankImg.SetActive(true);
                    string iconName = new StringBuilder().Append("Rank_")
                                                         .Append(ownGame + 1).ToString();
                    RankImg.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", iconName);
                }
                else
                {
                    RankTxt.gameObject.SetActive(true);
                    RankImg.SetActive(false);
                    str = (ownGame + 1).ToString();
                }
            }
            RankTxt.text = str;
            NameTxt.text = ownGameRank.PlayerName;
            GoldTxt.text = new StringBuilder().Append("获胜局数:")
                                              .Append(ownGameRank.WinCount)
                                              .ToString();
            RewardBtn.gameObject.SetActive(g2cWeek.IsGetGameRank);
            HeadManager.setHeadSprite(Icon, PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
        }

        private async void OnEnterRoom(int i)
        {
            try
            {
                UINetLoadingComponent.showNetLoading();

                RoomConfig roomConfig = ConfigHelp.Get<RoomConfig>(i);
                if (PlayerInfoComponent.Instance.GetPlayerInfo().GoldNum < roomConfig.MinThreshold)
                {
                    ToastScript.createToast("金币不足：" + roomConfig.MinThreshold);
                    UINetLoadingComponent.closeNetLoading();
                    return;
                }
                G2C_EnterRoom enterRoom = (G2C_EnterRoom)await Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2G_EnterRoom() { RoomType = i });

                PlayerInfoComponent.Instance.RoomType = i;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }   
            
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        private async Task SetPlayerInfo()
        {
            long uid = PlayerInfoComponent.Instance.uid;

            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = uid });

            if (g2CPlayerInfo == null)
            {
                Debug.Log("用户信息错误");
                return;
            }
            PlayerInfoComponent.Instance.SetPlayerInfo(g2CPlayerInfo.PlayerInfo);
            refreshUI();
            
            // 设置头像
            {
                HeadManager.setHeadSprite(playerIcon, PlayerInfoComponent.Instance.GetPlayerInfo().Icon);
            }
        }

        /// <summary>
        /// 预留接口(关闭或打开主界面)
        /// </summary>
        /// <param name="isHide"></param>
        public void SetUIHideOrOpen(bool isHide)
        {
            GetParent<UI>().GameObject.SetActive(isHide);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void refreshUI()
        {
            PlayerInfo info = PlayerInfoComponent.Instance.GetPlayerInfo();

            HeadManager.setHeadSprite(playerIcon, info.Icon);
            HeadManager.setHeadSprite(Icon, info.Icon);

            playerNameTxt.text = info.Name;
            goldNumTxt.text = info.GoldNum.ToString();
            wingNumTxt.text = info.WingNum.ToString();
            HuaFeiNumTxt.text = (info.HuaFeiNum / 100.0f).ToString();
            ScoreTxt.text = info.Score.ToString();

            if (GameUtil.isVIP())
            {
                PlayerInfoBg.transform.Find("HeadKuang").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", "touxiangkuang_vip");
            }

            if (PlayerInfoComponent.Instance.GetPlayerInfo().IsRealName)
            {
                if (RealNameTip != null)
                {
                    RealNameTip.transform.localScale = Vector3.zero;
                }
            }
        }

        public async void addLaBaContent(string content)
        {
            LaBa.transform.Find("Text_content").GetComponent<Text>().text = content;

            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(5000);

            if (isDispose)
            {
                return;
            }

            LaBa.transform.Find("Text_content").GetComponent<Text>().text = GameUtil.getTips();
        }
    }
}