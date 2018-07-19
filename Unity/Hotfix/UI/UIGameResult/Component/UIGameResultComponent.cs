using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UiGameResultComponentSystem: AwakeSystem<UIGameResultComponent>
    {
        public override void Awake(UIGameResultComponent self)
        {
            self.Awake();
        }
    }

    public class UIGameResultComponent: Component
    {
        private Button Button_close;
        private Button Button_back;
        private Button Button_jixu;

        private GameObject winPlayer;
        private GameObject otherPlayer1;
        private GameObject otherPlayer2;
        private GameObject otherPlayer3;
        private List<GameObject> playerList = new List<GameObject>();

        bool isTimerEnd = false;

        private Text Text_daojishi;
        private Actor_GamerHuPai huPaiNeedData;

        public void Awake()
        {
            ToastScript.clear();

            initData();
        }

        public void initData()
        {
            isTimerEnd = false;

            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            Button_close = rc.Get<GameObject>("Button_close").GetComponent<Button>();
            Button_back = rc.Get<GameObject>("Button_back").GetComponent<Button>();
            Button_jixu = rc.Get<GameObject>("Button_jixu").GetComponent<Button>();

            winPlayer = rc.Get<GameObject>("winPlayer");
            otherPlayer1 = rc.Get<GameObject>("otherPlayer1");
            otherPlayer2 = rc.Get<GameObject>("otherPlayer2");
            otherPlayer3 = rc.Get<GameObject>("otherPlayer3");

            playerList.Clear();
            playerList.Add(otherPlayer1);
            playerList.Add(otherPlayer2);
            playerList.Add(otherPlayer3);

            Text_daojishi = rc.Get<GameObject>("Text_daojishi").GetComponent<Text>();

            Button_close.onClick.Add(onClick_close);
            Button_back.onClick.Add(onClick_Back);
            Button_jixu.onClick.Add(onClick_jixu);

            CommonUtil.SetTextFont(Button_close.transform.parent.gameObject);
        }


        public void setData(Actor_GamerHuPai data, GamerComponent gamerComponent, int BeiLv)
        {
            this.huPaiNeedData = data;

            Gamer[] gamers = gamerComponent.GetAll();
            Gamer huPaiGamer = gamerComponent.Get(data.Uid);

            for (int i = 0; i < gamers.Length; i++)
            {
                if (gamers[i] == null)
                    continue;
                if (gamers[i].UserID == data.Uid)
                {
                    playerList.Insert(i, winPlayer);
                }
            }

            int huaCount = 0;
            //设置胡牌的花数
            List<int> dataHuPaiTypes = data.HuPaiTypes;
            for (int j = 0; j < dataHuPaiTypes.Count; j++)
            {
                Consts.HuPaiType huPaiType = (Consts.HuPaiType) dataHuPaiTypes[j];
                int count;
                Logic_NJMJ.getInstance().HuPaiHuaCount.TryGetValue(huPaiType, out count);
                //胡牌花
                huaCount += count;
            }

            //硬花
            huaCount += data.YingHuaCount;
            //软花
            huaCount += data.RuanHuaCount;

            huaCount += data.BixiaHuCount;
            //基数
            huaCount += 20;
            //砸2
            huaCount *= 2;
            //设置胡牌的人
            for (int i = 0; i < gamers.Length; i++)
            {
                Gamer gamer = gamers[i];
                GameObject gameObject = this.playerList[i];
                Image headImage = gameObject.transform.Find("head").GetComponent<Image>();
                Text nameText = gameObject.transform.Find("name").GetComponent<Text>();
                Text goldText = gameObject.transform.Find("Text_gold").GetComponent<Text>();

                //headImage.sprite = CommonUtil.getSpriteByBundle("playericon", gamer.PlayerInfo.Icon);
                HeadManager.setHeadSprite(headImage, gamer.PlayerInfo.Icon);
                nameText.text = gamer.PlayerInfo.Name + "";
                //胡牌的ui
                if (gamer.UserID == data.Uid)
                {
                    Text huaCountText = gameObject.Get<GameObject>("Text").GetComponent<Text>();
                    GameObject allhuashutype = gameObject.Get<GameObject>("Allhuashutype");

                    Text obj1 = allhuashutype.transform.GetChild(0).GetComponent<Text>();
                    Text obj2 = allhuashutype.transform.GetChild(1).GetComponent<Text>();
                    Text obj3 = allhuashutype.transform.GetChild(2).GetComponent<Text>();
                    obj1.gameObject.SetActive(true);
                    obj2.gameObject.SetActive(true);
                    obj3.gameObject.SetActive(true);

                    obj1.text = $"硬花{data.YingHuaCount}";
                    obj2.text = $"软花{data.RuanHuaCount}";
                    obj3.text = $"基数{20}";

                    //胡牌类型
                    for (int j = 0; j < dataHuPaiTypes.Count; j++)
                    {
                        Consts.HuPaiType huPaiType = (Consts.HuPaiType) dataHuPaiTypes[j];
                        int count;
                        string name;
                        Logic_NJMJ.getInstance().HuPaiHuaCount.TryGetValue(huPaiType, out count);
                        Logic_NJMJ.getInstance().HuPaiHuaName.TryGetValue(huPaiType, out name);

                        Text obj = allhuashutype.transform.GetChild(j + 3).gameObject.GetComponent<Text>();
                        obj.gameObject.SetActive(true);
                        obj.text = $"{name}{count}";
                    }

                    if (data.BixiaHuCount > 0)
                    {
                        Text obj4 = allhuashutype.transform.GetChild(dataHuPaiTypes.Count + 3).GetComponent<Text>();
                        obj4.gameObject.SetActive(true);
                        obj4.text = $"比下胡{data.BixiaHuCount}";

                        Text obj5 = allhuashutype.transform.GetChild(dataHuPaiTypes.Count + 4).GetComponent<Text>();
                        obj5.gameObject.SetActive(true);
                        obj5.text = "砸2";
                    }
                    else
                    {
                        Text obj4 = allhuashutype.transform.GetChild(dataHuPaiTypes.Count + 3).GetComponent<Text>();
                        obj4.gameObject.SetActive(true);
                        obj4.text = "砸2";
                    }
                    huaCountText.text = huaCount + "";
                    if (data.IsZiMo)
                    {
                        goldText.text = BeiLv * huaCount * 3 + "";
                    }
                    else
                    {
                        goldText.text = BeiLv * huaCount + "";
                    }
                }
                else
                {
                    if (data.IsZiMo)
                    {
                        if (BeiLv * huaCount == 0)
                        {
                            goldText.text = "0";
                        }
                        else
                        {
                            goldText.text = "-" + BeiLv * huaCount + "";
                        }
                    }
                    else
                    {
                        if (gamer.UserID == data.FangPaoUid)
                        {
                            if (BeiLv * huaCount == 0)
                            {
                                goldText.text = "0";
                            }
                            else
                            {
                                goldText.text = "-" + BeiLv * huaCount + "";
                            }
                            
                            nameText.color = Color.yellow;
                        }
                    }
                }
            }

            if (huPaiNeedData.IsZiMo)
            {
                winPlayer.transform.Find("hupaiType").GetComponent<Image>().sprite =
                        CommonUtil.getSpriteByBundle("image_gameresult", "gameresult_hu");
            }

            SetHuPaiPlayerData();
        }

        private void SetHuPaiPlayerData()
        {
        }

        public void onClick_close()
        {
            onClick_jixu();
        }

        private void onClick_Back()
        {
            SessionComponent.Instance.Session.Send(new Actor_GamerExitRoom() { IsFromClient = true });
//            CommonUtil.ShowUI(UIType.UIMain);
//            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIRoom);
//            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIReady);
//            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
//            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChatShow);
//            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChat);
        }

        public void onClick_jixu()
        {
            isTimerEnd = true;
            //删除弹窗
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);

            UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIType.UIRoom);
            //重新开始
            ui.GetComponent<UIRoomComponent>().ContinueGamer();
          
            //设置准备
//            UI uiReady = Game.Scene.GetComponent<UIComponent>().Create(UIType.UIReady);

            //重新请求数据
            SessionComponent.Instance.Session.Send(new Actor_GamerContinueGame());

        }

        public async void startTimer(int time = 20)
        {
            Text_daojishi.text = ("准备倒计时 " + time.ToString());
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                --time;
                if (time < 0) time = 0;
                if (isTimerEnd)
                {
                    return;
                }

                if (this.IsDisposed)
                {
                    return;
                }
                Text_daojishi.text = ("准备倒计时 " + time.ToString());
            }

            //            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
        }

        public override void Dispose()
        {
            base.Dispose();

            isTimerEnd = true;
        }

        public void SetFlowGame(Actor_GameFlow message, GamerComponent gamerComponent)
        {
            Gamer[] gamers = gamerComponent.GetAll();

            for (int i = 0; i < gamers.Length; i++)
            {
                if (gamers[i] == null)
                    continue;
                if (gamers[i].UserID == PlayerInfoComponent.Instance.uid)
                {
                    playerList.Insert(i, winPlayer);
                }
            }

            for (int i = 0; i < gamers.Length; i++)
            {
                Gamer gamer = gamers[i];
                GameObject gameObject = this.playerList[i];
                Image headImage = gameObject.transform.Find("head").GetComponent<Image>();
                Text nameText = gameObject.transform.Find("name").GetComponent<Text>();
                Text goldText = gameObject.transform.Find("Text_gold").GetComponent<Text>();

                HeadManager.setHeadSprite(headImage, gamer.PlayerInfo.Icon);
                nameText.text = gamer.PlayerInfo.Name;
                goldText.text = 0 + "";
            }

            winPlayer.transform.Find("hupaiType").GetComponent<Image>().sprite =
                    CommonUtil.getSpriteByBundle("image_gameresult", "gameresult_liuju");
        }

        public async void SetFriendRoom()
        {
            Button_back.gameObject.SetActive(false);
            Button_close.gameObject.SetActive(false);
            await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(5000);
            if (IsDisposed)
            {
                return;
            }

            onClick_jixu();
        }
    }
}