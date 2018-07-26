using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class GamerUIComponentStartSystem : StartSystem<GamerUIComponent>
    {
        public override void Start(GamerUIComponent self)
        {
            self.Start();
        }
    }

    /// <summary>
    /// 玩家UI组件
    /// </summary>
    public class GamerUIComponent : Component
    {
        //UI面板
        public GameObject Panel { get; private set; }

        //玩家昵称
        public string NickName { get { return name.text; } }

        public Image head;
        private Text prompt;
        private Text name;

        private Image readyHead;
        private Text readyName;
        private Text readyText;
        private Text shengLvText;
        private Text jinbiText;
        private Text uidText;
        private GameObject headInfo;
        private GameObject changeMoney;
        private GameObject vip;
        public GameObject FaceObj { get; set; }
        private GameObject zhuang;
        private CancellationTokenSource tokenSource;
        private GameObject buHua;
        private GameObject headReadyInfo;
        private UIRoomComponent uiRoomComponent;

        public int Index { get; set; }

        public void Start()
        {
           
        }

        /// <summary>
        /// 重置面板
        /// </summary>
        public void ResetPanel()
        {
//            ResetPrompt();
        
            this.name.text = "空位";

            this.Panel = null;
            this.name = null;
        }

        /// <summary>
        /// 设置面板
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="gameObject"></param>
        /// <param name="index"></param>
        public void SetPanel(GameObject panel, GameObject readyPanel, int index)
        {
            panel.SetActive(true);
            this.Panel = panel;
            this.Index = index;
            //绑定关联
            this.head = this.Panel.Get<GameObject>("Head").GetComponent<Image>();
            this.name = this.Panel.Get<GameObject>("Name").GetComponent<Text>();
            this.prompt = this.Panel.Get<GameObject>("Prompt").GetComponent<Text>();
            this.changeMoney = this.Panel.Get<GameObject>("ChangeMoney");
            this.zhuang = this.Panel.Get<GameObject>("Zhuang");
            this.buHua = this.Panel.Get<GameObject>("BuHua");


            if (index != 0)
            {
                this.headInfo = this.head.transform.Find("HeadInfo").gameObject;
                this.shengLvText = this.headInfo.Get<GameObject>("Shenglv").GetComponent<Text>();
                this.jinbiText = this.headInfo.Get<GameObject>("Jinbi").GetComponent<Text>();
                this.uidText = this.headInfo.Get<GameObject>("Uid").GetComponent<Text>();
                this.headInfo.SetActive(false);
                head.GetComponent<Button>().onClick.RemoveAllListeners();
                head.GetComponent<Button>().onClick.Add(OnShowHeadInfo);
            }

//            this.readyHead = readyPanel.Get<GameObject>("Image").GetComponent<Image>();
//            this.readyName = readyPanel.Get<GameObject>("Name").GetComponent<Text>();
//            this.readyText = readyPanel.Get<GameObject>("Text").GetComponent<Text>();

            UpdatePanel();
        }

        /// <summary>
        /// 设置花牌
        /// </summary>
        /// <param name="obj"></param>
        public void SetFace(GameObject obj)
        {
            this.FaceObj = obj;
        }

        /// <summary>
        /// 补花
        /// </summary>
        /// <param name="weight"></param>
        public void SetBuHua(int weight)
        {
            try
            {
                GameObject obj = CommonUtil.getGameObjByBundle("Item_Top_Card");
                obj.GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("Image_Top_Card", "card_" + weight);
                GameObject.Instantiate(obj, FaceObj.transform);
              
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async void ShowBuHua()
        {
            buHua.gameObject.SetActive(true);
            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1500);
            if (this.IsDisposed)
            {
                return;
            }
            buHua.gameObject.SetActive(false);
        }

        public void SetGoldChange(int num)
        {
            GameHelp.ShowPlusGoldChange(changeMoney, num);
        }

        private void OnShowHeadInfo()
        {
            if (this.headInfo.activeSelf)
            {
                headInfo.SetActive(false);
            }
            else
            {
                headInfo.SetActive(true);
            }
        }

        public void ResetReadyPanel()
        {
            if (readyName == null)
            {
                return;
            }
            readyName.text = "";
            readyHead.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "icon_default");
            readyText.text = "";
            vip.transform.localScale = Vector3.zero;
            headReadyInfo.gameObject.SetActive(false);
            this.headReadyInfo.GetComponentInParent<Button>().onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 更新面板
        /// </summary>
        public void UpdatePanel()
        {
            if (this.Panel != null)
            {
                SetUserInfo();
            }
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        public void GameStart()
        {

//            ResetPrompt();
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="id"></param>
        private async void SetUserInfo()
        {
            PlayerInfo playerInfo = this.GetParent<Gamer>().PlayerInfo;

            if (this.Panel != null || playerInfo == null)
            {
                name.text = playerInfo.Name;
                head.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(playerInfo.Icon);

                if (Index != 0)
                {
                    uidText.text = playerInfo.Name;
                    if (UIRoomComponent.IsFriendRoom)
                    {
                        jinbiText.text = $"积 分:<color=#FFF089FF>{playerInfo.Score}</color>";
                    }
                    else
                    {
                        jinbiText.text = $"金 币:<color=#FFF089FF>{playerInfo.GoldNum}</color>";
                    }

                    float i;
                    if (playerInfo.TotalGameCount == 0)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = GameUtil.GetWinRate(playerInfo.TotalGameCount, playerInfo.WinGameCount);
                    }
                    shengLvText.text = $"胜 率:<color=#FFF089FF>{i}%</color>";

                    if (GameUtil.isVIP(playerInfo))
                    {
                        head.transform.Find("vip").transform.localScale = Vector3.one;
                    }
                    else
                    {
                        head.transform.Find("vip").transform.localScale = Vector3.zero;
                    }

                }

            }
        }

        public void SetZhuang()
        {
            Gamer gamer = this.GetParent<Gamer>();
            if (gamer.IsBanker)
            {
                zhuang.SetActive(true);
            }
            else
            {
                zhuang.SetActive(false);
            }
        }

        //        public async Task GetPlayerInfo()
//        {
//            tokenSource = new CancellationTokenSource();
//            try
//            {
//                Gamer gamer = this.GetParent<Gamer>();
//                Log.Debug("请求gamer信息:" + gamer.UserID);
//                G2C_PlayerInfo playerInfo = (G2C_PlayerInfo)await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo() { uid = gamer.UserID }, tokenSource.Token);
//                gamer.PlayerInfo = playerInfo.PlayerInfo;
//            }
//            catch (Exception e)
//            {
//                Log.Error(e);
//                tokenSource.Cancel();
//            }
//        }

        /// <summary>
        /// 设置准备界面
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetHeadPanel(GameObject gameObject, int index)
        {
            try
            {
                if (gameObject == null)
                {
                    return;
                }

                Index = index;
                Gamer gamer = this.GetParent<Gamer>();
                this.readyHead = gameObject.Get<GameObject>("Image").GetComponent<Image>();
                this.readyName = gameObject.Get<GameObject>("Name").GetComponent<Text>();
                this.readyText = gameObject.Get<GameObject>("Text").GetComponent<Text>();
                this.vip = gameObject.Get<GameObject>("vip");
                PlayerInfo playerInfo = gamer.PlayerInfo;

                if (readyName == null) return;
                readyName.text = playerInfo.Name + "";
                HeadManager.setHeadSprite(readyHead, playerInfo.Icon);

                if (gamer.IsReady)
                {
                    gameObject.transform.Find("Text").GetComponent<Text>().text = "已准备";
                }
                else
                {
                    readyText.text = "";
                }

                if (GameUtil.isVIP(playerInfo))
                {
                    vip.transform.localScale = Vector3.one;
                }
                else
                {
                    vip.transform.localScale = Vector3.zero;
                }

                #region 准备界面信息框
                // if(Index != 0)
                {
                    this.headReadyInfo = gameObject.Get<GameObject>("HeadInfo");
                    Image kickOffImage = this.headReadyInfo.Get<GameObject>("KickOffImage").GetComponent<Image>();
                    Text uidReadyText = this.headReadyInfo.Get<GameObject>("Uid").GetComponent<Text>();
                    Text shenglvReadyText = this.headReadyInfo.Get<GameObject>("Shenglv").GetComponent<Text>();
                    Text jinbiReadyText = this.headReadyInfo.Get<GameObject>("Jinbi").GetComponent<Text>();
                    this.headReadyInfo.SetActive(false);
                    this.headReadyInfo.GetComponentInParent<Button>().onClick.RemoveAllListeners();
                    this.headReadyInfo.GetComponentInParent<Button>().onClick.Add(() =>
                    {
                        if (this.headReadyInfo.activeSelf)
                        {
                            this.headReadyInfo.SetActive(false);
                        }
                        else
                        {
                            this.headReadyInfo.SetActive(true);
                        }
                    });
                    uidReadyText.text = playerInfo.Name;


                    UI uiRoom = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
                    this.uiRoomComponent = uiRoom.GetComponent<UIRoomComponent>();
                    if (UIRoomComponent.IsFriendRoom)
                    {
                        shenglvReadyText.text = $"积 分:<color=#FFF089FF>{playerInfo.Score}</color>";
                    }
                    else
                    {
                        shenglvReadyText.text = $"金 币:<color=#FFF089FF>{playerInfo.GoldNum}</color>";
                    }

                    float i;
                    if (playerInfo.TotalGameCount == 0)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = GameUtil.GetWinRate(playerInfo.TotalGameCount, playerInfo.WinGameCount);
                    }
                    jinbiReadyText.text = $"胜 率:<color=#FFF089FF>{i}%</color>";

                    //踢人
                   
                    if (uiRoomComponent.masterUserId != 0 && uiRoomComponent.masterUserId == PlayerInfoComponent.Instance.uid && Index != 0)
                    {
                        kickOffImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        kickOffImage.gameObject.SetActive(false);
                    }

                    kickOffImage.gameObject.GetComponent<Button>().onClick.Add(() =>
                    {
                        SessionComponent.Instance.Session.Send(new Actor_GamerKickOff()
                        {
                            KickedUserId = gamer.UserID
                        });
                    });

                }

                

                #endregion
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            GameHelp.DeleteAllItem(FaceObj);
            FaceObj = null;
        }

        public void SetReady()
        {
            if(readyText == null) return;
            readyText.text = "已准备";
        }

        public void ShowTrust()
        {
            head.sprite = CommonUtil.getSpriteByBundle("Image_Desk_Card", "tuoguan_icon");
        }

        public void ShowPlayerIcon()
        {
            PlayerInfo playerInfo = this.GetParent<Gamer>().PlayerInfo;

            HeadManager.setHeadSprite(head, playerInfo.Icon);
        }
    }
}
