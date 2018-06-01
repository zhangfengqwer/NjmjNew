using DG.Tweening;
using ETModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
        private List<string> labaList = new List<string>();

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
       
        #region myRank
        public Text GoldTxt;
        private Text NameTxt;
        private Text RankTxt;
        private Image Icon;
        private GameObject RankImg;
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

            // 转盘
            BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Button>().onClick.Add(() =>
            {
                // 向左
                if (BtnList_Down.transform.localPosition.x > 500)
                {
                    BtnList_Down.GetComponent<RectTransform>().DOAnchorPos(new Vector2(248, -286.4f), 0.5f, false).OnComplete(() =>
                    {
                        PlayerInfoBg.transform.Find("GoldBg").transform.localScale = Vector3.zero;
                    });
                    BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main","btn_you");
                }
                // 向右
                else
                {
                    BtnList_Down.GetComponent<RectTransform>().DOAnchorPos(new Vector2(523, -286.4f), 0.5f, false).OnComplete(() =>
                    {
                        PlayerInfoBg.transform.Find("GoldBg").transform.localScale = new Vector3(1, 1, 1);
                    });

                    BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", "btn_zuo");
                }
            });

            // 喇叭
            LaBa.transform.Find("Btn_laba").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseLaBa);
            });

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
                ToastScript.createToast("暂未开放：比赛场");
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
                OnEnterRoom();
            });

            // 休闲场-精英场
            ChoiceRoomType.transform.Find("Relax/Btn_jingying").GetComponent<Button>().onClick.Add(() =>
            {
                OnEnterRoom();
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
                ShowGoldRank();
                SetMyRank();
            });

            Rank.transform.Find("Btn_game").GetComponent<Button>().onClick.Add(() =>
            {
                ShowGameRank();
                SetMyGameRank();
            });

            //向服务器发送消息请求玩家信息，然后设置玩家基本信息
            await SetPlayerInfo();
            GetRankInfo();

            CommonUtil.ShowUI(UIType.UIDaily);

            checkLaBa();

            HeartBeat.getInstance().startHeartBeat();
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

            isDispose = true;
            wealthRankList.Clear();
            gameRankList.Clear();
            rankItemList.Clear();
            gameItemList.Clear();
            uiList.Clear();
            gameUiList.Clear();
            labaList.Clear();
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
                    obj = rankItemList[i];
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
                    obj = rankItemList[i];
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
        }

        /// <summary>
        /// 设置排行榜信息
        /// </summary>
		public async void GetRankInfo()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_Rank g2cRank = (G2C_Rank)await Game.Scene.GetComponent<SessionWrapComponent>()
                .Session.Call(new C2G_Rank { Uid = PlayerInfoComponent.Instance.uid,RankType = 0 });
            UINetLoadingComponent.closeNetLoading();

            //设置排行榜信息
            wealthRankList = g2cRank.RankList;
            gameRankList = g2cRank.GameRankList;

            isOwnRank = IsInRank(PlayerInfoComponent.Instance.uid);
            isGameRank = IsInGameRank(PlayerInfoComponent.Instance.uid);

            ownRank = GetWealthIndext(PlayerInfoComponent.Instance.uid);
            ownGame = GetGameIndext(PlayerInfoComponent.Instance.uid);
            ShowGoldRank();
            ownWealthRank = g2cRank.OwnWealthRank;
            ownGameRank = g2cRank.OwnGameRank;
            SetMyRank();
        }

        /// <summary>
        /// 判断是否上财富榜
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsInRank(long uid)
        {
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
            for (int i = 0; i < gameRankList.Count; ++i)
            {
                if (gameRankList[i].UId == uid)
                {
                    gameRankList.RemoveAt(i);
                    return i;
                }
                   
            }
            return 30;
        }

        /// <summary>
        /// 获得我的名次（财富榜）
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private int GetWealthIndext(long uid)
        {
            for (int i = 0; i < wealthRankList.Count; ++i)
            {
                if (wealthRankList[i].UId == uid)
                {
                    wealthRankList.RemoveAt(i);
                    return i;
                }
            }
            return 30;
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
            Icon.sprite = CommonUtil.getSpriteByBundle("PlayerIcon", ownWealthRank.Icon);
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
            GoldTxt.text = new StringBuilder().Append("总局数:")
                                              .Append(ownGameRank.TotalCount)
                                              .ToString();
            Icon.sprite = CommonUtil.getSpriteByBundle("PlayerIcon", ownGameRank.Icon);
        }

        private async void OnEnterRoom()
        {
            UINetLoadingComponent.showNetLoading();
            G2C_EnterRoom enterRoom = (G2C_EnterRoom)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(
                new C2G_EnterRoom());
            UINetLoadingComponent.closeNetLoading();
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        private async Task SetPlayerInfo()
        {
            long uid = PlayerInfoComponent.Instance.uid;

            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionWrapComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = uid });

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

            Sprite icon = Game.Scene.GetComponent<UIIconComponent>().GetSprite(info.Icon);
            if (icon != null)
            {
                playerIcon.sprite = icon;
            }
            else
            {
                Log.Warning("icon数据为空，请重新注册");
            }

            playerNameTxt.text = info.Name;
            goldNumTxt.text = info.GoldNum.ToString();
            wingNumTxt.text = info.WingNum.ToString();
            HuaFeiNumTxt.text = info.HuaFeiNum.ToString();

            if (GameUtil.isVIP())
            {
                PlayerInfoBg.transform.Find("HeadKuang").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main", "touxiangkuang_vip");
            }
        }

        public void addLaBaContent(string content)
        {
            labaList.Add(content);

            if (LaBa.transform.Find("Text_content").GetComponent<Text>().text.CompareTo("") == 0)
            {
                LaBa.transform.Find("Text_content").GetComponent<Text>().text = content;
            }
        }

        public async void checkLaBa()
        {
            while (true)
            {
                if (isDispose)
                {
                    return;
                }

                if (labaList.Count > 0)
                {
                    LaBa.transform.Find("Text_content").GetComponent<Text>().text = labaList[0];
                    labaList.RemoveAt(0);
                }
                else
                {
                    LaBa.transform.Find("Text_content").GetComponent<Text>().text = "";
                }

                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(5000);
            }
        }
    }
}