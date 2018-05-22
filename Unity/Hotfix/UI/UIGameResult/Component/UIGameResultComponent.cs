using System;
using System.Collections.Generic;
using System.Net;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiGameResultComponentSystem : AwakeSystem<UIGameResultComponent>
	{
		public override void Awake(UIGameResultComponent self)
		{
			self.Awake();
		}
	}
	
	public class UIGameResultComponent : Component
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
            startTimer();
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

            playerList.Add(otherPlayer1);
            playerList.Add(otherPlayer2);
            playerList.Add(otherPlayer3);

            Text_daojishi = rc.Get<GameObject>("Text_daojishi").GetComponent<Text>();
            
            Button_close.onClick.Add(onClick_close);
            Button_back.onClick.Add(onClick_close);
            Button_jixu.onClick.Add(onClick_jixu);
        }

        public void setData(Actor_GamerHuPai data, GamerComponent gamerComponent, int BeiLv)
        {
            this.huPaiNeedData = data;

            Gamer[] gamers = gamerComponent.GetAll();
            Gamer huPaiGamer = gamerComponent.Get(data.Uid);

            for (int i = 0; i < gamers.Length; i++)
            {
                if (gamers[i].UserID == data.Uid)
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

                headImage.sprite = Game.Scene.GetComponent<UIIconComponent>().GetSprite(gamer.Head);
                nameText.text = gamer.UserID + "";
                int huaCount = 0;
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

                    huaCount += data.YingHuaCount;
                    huaCount += data.RuanHuaCount;
                    huaCount += 20;

                    List<int> dataHuPaiTypes = data.HuPaiTypes;

                    //胡牌类型
                    for (int j = 0; j < dataHuPaiTypes.Count; j++)
                    {
                        Consts.HuPaiType huPaiType = (Consts.HuPaiType) dataHuPaiTypes[j];
                        int count;
                        Logic_NJMJ.getInstance().HuPaiHuaCount.TryGetValue(huPaiType, out count);

                        Text obj = allhuashutype.transform.GetChild(j + 3).gameObject.GetComponent<Text>();
                        obj.gameObject.SetActive(true);

                        obj.text = $"{huPaiType}{count}";
                 
                        huaCount += count;
                    }

                    Text obj4 = allhuashutype.transform.GetChild(dataHuPaiTypes.Count + 3).GetComponent<Text>();
                    obj4.gameObject.SetActive(true);
                    obj4.text = "砸2";
                    huaCount *= 2;
                    huaCountText.text = huaCount + "";
                }
                goldText.text = BeiLv * huaCount + "";
            }

            if (huPaiNeedData.IsZiMo)
            {
                winPlayer.transform.Find("hupaiType").GetComponent<Image>().sprite = CommonUtil.getSpriteByBundle("image_gameresult", "gameresult_hu");
            }

            SetHuPaiPlayerData();
        }

	    private void SetHuPaiPlayerData()
	    {

	    }

	    public void onClick_close()
        {
            isTimerEnd = true;
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
        }

        public void onClick_jixu()
        {
            ToastScript.createToast("暂未开放");
        }

        public async void startTimer()
        {
            int time = 15;
            while (time >= 0)
            {
                await ETModel.Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
                --time;

                if (isTimerEnd)
                {
                    return;
                }

                Text_daojishi.text = ("准备倒计时 " + time.ToString());
            }

//            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIGameResult);
        }
    }
}
