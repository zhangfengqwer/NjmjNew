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

        public GameObject currentItem = new GameObject();
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

            this.changeTableBtn.onClick.Add(OnChangeTable);
            this.exitBtn.onClick.Add(OnExit);
            this.readyBtn.onClick.Add(OnReady);
        }

        private async void OnReady()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerReady() { Uid = PlayerInfoComponent.Instance.uid });
        }

        private async void OnExit()
        {
            SessionWrapComponent.Instance.Session.Send(new Actor_GamerExitRoom() { IsFromClient = true });

            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIMain);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRoom);
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
    }
}