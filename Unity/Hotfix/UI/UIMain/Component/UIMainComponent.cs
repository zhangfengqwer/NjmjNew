using DG.Tweening;
using ETModel;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class MyRankStruct
    {
        public bool isRank;
        public int numb;
    }

    [ObjectSystem]
    public class UIMainComponentSystem: AwakeSystem<UIMainComponent>
    {
        public override void Awake(UIMainComponent self)
        {
            self.Awake();
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
        public GameObject Btn_GoldSelect;
        public GameObject Btn_GameSelect;
        public GameObject Grid;
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

        public void Awake()
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
                    BtnList_Down.GetComponent<RectTransform>().DOAnchorPos(new Vector2(248, -286.4f), 0.5f, false);
                    BtnList_Down.transform.Find("Btn_JianTou").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_main","btn_you");
                }
                // 向右
                else
                {
                    BtnList_Down.GetComponent<RectTransform>().DOAnchorPos(new Vector2(523, -286.4f), 0.5f, false);
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
                ToastScript.createToast("暂未开放：活动");
            });

            // 任务
            BtnList_Down.transform.Find("Grid/Btn_Task").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UITask);
            });

            // 成就
            BtnList_Down.transform.Find("Grid/Btn_ChengJiu").GetComponent<Button>().onClick.Add(() =>
            {
                //ToastScript.createToast("暂未开放：成就");
                //Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUseHuaFei);
                //Game.Scene.GetComponent<UIComponent>().Create(UIType.UISet);
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
                ToastScript.createToast("暂未开放：转盘");
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

            PlayerInfoBg.transform.Find("Btn_set").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                SetUIHideOrOpen(false);
            });

            playerIcon.GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                SetUIHideOrOpen(false);
            });

            PlayerInfoBg.transform.Find("HeadKuang").GetComponent<Button>().onClick.Add(() =>
            {
                Game.Scene.GetComponent<UIComponent>().Create(UIType.UIPlayerInfo);
                SetUIHideOrOpen(false);
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
            SetPlayerInfo();
            GetRankInfo();

            CommonUtil.ShowUI(UIType.UIDaily);

            checkLaBa();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            isDispose = true;
        }

        private void ShowGoldRank()
        {
            GameObject obj = null;
            Btn_GoldSelect.gameObject.SetActive(true);
            Btn_GameSelect.gameObject.SetActive(false);
            //Grid.transform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
            
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
                uiList[i].GetComponent<UIRankItemComponent>().SetGoldItem(wealthRankList[i], i);
            }
        }

        private void ShowGameRank()
        {
            Btn_GoldSelect.gameObject.SetActive(false);
            Btn_GameSelect.gameObject.SetActive(true);
            //Grid.transform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
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
                gameUiList[i].GetComponent<UIRankItemComponent>().SetGameItem(gameRankList[i], i);
            }
        }

		public async void GetRankInfo()
        {
            G2C_Rank g2cRank = (G2C_Rank)await Game.Scene.GetComponent<SessionWrapComponent>()
                .Session.Call(new C2G_Rank { Uid = PlayerInfoComponent.Instance.uid,RankType = 0 });
            //设置排行榜信息
            wealthRankList = g2cRank.RankList;
            gameRankList = g2cRank.GameRankList;
            Debug.Log(JsonHelper.ToJson(g2cRank.RankList));
            ShowGoldRank();
            ownWealthRank = g2cRank.OwnWealthRank;
            ownGameRank = g2cRank.OwnGameRank;

            SetMyRank();
        }

        public bool IsInRank(long uid)
        {
            for (int i = 0; i < wealthRankList.Count; ++i)
            {
                if (wealthRankList[i].UId == uid)
                    return true;
            }
            return false;
        }

        public bool IsInGameRank(long uid)
        {
            for(int i = 0;i< gameRankList.Count; ++i)
            {
                if (gameRankList[i].UId == uid)
                    return true;
            }
            return false;
        }

        private int GetGameIndext(long uid)
        {
            for (int i = 0; i < gameRankList.Count; ++i)
            {
                if (gameRankList[i].UId == uid)
                    return i;
            }
            return -1;
        }

        private int GetWealthIndext(long uid)
        {
            for (int i = 0; i < wealthRankList.Count; ++i)
            {
                if (wealthRankList[i].UId == uid)
                    return i;
            }
            return -1;
        }

        private void SetMyRank()
        {
            string str = "";
            if (!IsInRank(ownWealthRank.UId))
            {
                RankTxt.gameObject.SetActive(true);
                RankImg.SetActive(false);
                str = "未上榜";
            }
            else
            {
                int index = GetWealthIndext(ownWealthRank.UId);
                if (index < 3 && index != -1)
                {
                    RankTxt.gameObject.SetActive(false);
                    RankImg.SetActive(true);
                    string iconName = new StringBuilder().Append("Rank_")
                                                         .Append(index + 1).ToString();
                    RankImg.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("uirankicon", iconName);
                }
                else
                {
                    RankTxt.gameObject.SetActive(true);
                    RankImg.SetActive(false);
                    str = (index + 1).ToString();
                }
            }
            RankTxt.text = str;
            NameTxt.text = ownWealthRank.PlayerName;
            GoldTxt.text = new StringBuilder().Append("金币:")
                                              .Append(ownWealthRank.GoldNum)
                                              .ToString();
            Icon.sprite = CommonUtil.getSpriteByBundle("PlayerIcon", ownWealthRank.Icon);
        }

        private void SetMyGameRank()
        {
            string str = "";
            if (!IsInGameRank(ownGameRank.UId))
            {
                RankTxt.gameObject.SetActive(true);
                RankImg.SetActive(false);
                str = "未上榜";
            }
            else
            {
                int index = GetGameIndext(ownGameRank.UId);
                if (index < 3 && index != -1)
                {
                    RankTxt.gameObject.SetActive(false);
                    RankImg.SetActive(true);
                    string iconName = new StringBuilder().Append("Rank_")
                                                         .Append(index + 1).ToString();
                    RankImg.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("uirankicon", iconName);
                }
                else
                {
                    RankTxt.gameObject.SetActive(true);
                    RankImg.SetActive(false);
                    str = (index + 1).ToString();
                }
                RankTxt.text = str;
            }
            NameTxt.text = ownGameRank.PlayerName;
            GoldTxt.text = new StringBuilder().Append("总局数:")
                                              .Append(ownGameRank.TotalCount)
                                              .ToString();
            Icon.sprite = CommonUtil.getSpriteByBundle("PlayerIcon", ownGameRank.Icon);
        }

        private async void RequestTaskInfo()
        {
            G2C_Task g2cTask = (G2C_Task)await SessionWrapComponent.Instance.Session.Call(new C2G_Task { uid = PlayerInfoComponent.Instance.uid });
            PlayerInfoComponent.Instance.SetTaskInfoList(g2cTask.TaskProgressList);
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UITask);
        }

        //public async void UpDatePlayerInfo()
        //{
        //    PlayerInfoComponent playerInfoComponent = Game.Scene.GetComponent<PlayerInfoComponent>();
        //    long uid = playerInfoComponent.uid;
        //    playerInfoComponent.GetPlayerInfo().WingNum = 1000;
        //    G2C_UpdatePlayerInfo g2cUpdatePlayerInfo = (G2C_UpdatePlayerInfo)await SessionWrapComponent.Instance.Session.Call(new C2G_UpdatePlayerInfo() { Uid = uid, playerInfo = playerInfoComponent.GetPlayerInfo() });
        //    UpDatePlayerInfo(g2cUpdatePlayerInfo.playerInfo);
        //}

        private async void OnEnterRoom()
        {
            G2C_EnterRoom enterRoom = (G2C_EnterRoom)await Game.Scene.GetComponent<SessionWrapComponent>().Session.Call(
                new C2G_EnterRoom());

          

        }

        private async void SetPlayerInfo()
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
        }

        public void SetUIHideOrOpen(bool isHide)
        {
            GetParent<UI>().GameObject.SetActive(isHide);
        }

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
                if (labaList.Count > 0)
                {
                    LaBa.transform.Find("Text_content").GetComponent<Text>().text = labaList[0];
                    labaList.RemoveAt(0);
                }
                else
                {
                    LaBa.transform.Find("Text_content").GetComponent<Text>().text = "";
                }

                if (isDispose)
                {
                    return;
                }

                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(5000);
            }
        }
    }
}